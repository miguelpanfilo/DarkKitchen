import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { ProductService } from '../../../products/services/product.service';
import { ShippingTypeService } from '../../../shipping-types/services/shipping-type.service';
import { ShippingType } from '../../../shipping-types/models/shipping-type.model';
import { Product } from '../../../products/models/product.model';
import { CreateOrderItemRequest } from '../../models/create-order-request.model';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';

interface CartItem {
  product: Product;
  quantity: number;
}

@Component({
  selector: 'app-create-order-page',
  imports: [CommonModule, FormsModule, StatusMessagesComponent],
  templateUrl: './create-order-page.component.html',
  styleUrl: './create-order-page.component.css'
})
export class CreateOrderPageComponent implements OnInit {
  products: Product[] = [];
  shippingTypes: ShippingType[] = [];
  cartItems: CartItem[] = [];

  deliveryType = '';
  street = '';
  doorNumber = '';
  apartment = '';

  isLoadingProducts = false;
  isSubmitting = false;
  errorMessage = '';

  constructor(
    private orderService: OrderService,
    private productService: ProductService,
    private shippingTypeService: ShippingTypeService,
    private router: Router
  ) { }

  ngOnInit() {
    this.loadProducts();
    this.shippingTypeService.getAll().subscribe({
      next: (types) => {
        this.shippingTypes = types;
        if (types.length > 0) this.deliveryType = types[0].name;
      },
      error: () => {
        this.errorMessage = 'Error al cargar los tipos de envío.';
      }
    });
  }

  loadProducts() {
    this.isLoadingProducts = true;
    this.productService.getProducts().subscribe({
      next: (products) => {
        this.isLoadingProducts = false;
        this.products = products.filter(p => p.isActive);
      },
      error: () => {
        this.isLoadingProducts = false;
        this.errorMessage = 'Error al cargar los productos.';
      }
    });
  }

  getCartQty(productId: number): number {
    return this.cartItems.find(i => i.product.id === productId)?.quantity ?? 0;
  }

  get cartSubtotal(): number {
    return this.cartItems.reduce((sum, i) => sum + i.product.price * i.quantity, 0);
  }

  get cartShippingCost(): number {
    return this.shippingTypes.find(s => s.name === this.deliveryType)?.cost ?? 0;
  }

  get cartTotal(): number {
    return this.cartSubtotal + this.cartShippingCost;
  }

  onIncrement(product: Product) {
    const existing = this.cartItems.find(i => i.product.id === product.id);
    if (existing) {
      existing.quantity++;
    } else {
      this.cartItems.push({ product, quantity: 1 });
    }
  }

  onDecrement(product: Product) {
    const idx = this.cartItems.findIndex(i => i.product.id === product.id);
    if (idx === -1) return;
    this.cartItems[idx].quantity--;
    if (this.cartItems[idx].quantity <= 0) {
      this.cartItems.splice(idx, 1);
    }
  }

  onSubmit() {
    this.errorMessage = '';

    if (this.cartItems.length === 0) {
      this.errorMessage = 'Agregá al menos un producto al carrito.';
      return;
    }
    if (!this.street.trim() || !this.doorNumber.trim()) {
      this.errorMessage = 'La calle y el número de puerta son obligatorios.';
      return;
    }

    const items: CreateOrderItemRequest[] = this.cartItems.map(i => ({
      productId: i.product.id,
      quantity: i.quantity
    }));

    this.isSubmitting = true;
    this.orderService.create({
      deliveryType: this.deliveryType,
      address: {
        street: this.street.trim(),
        doorNumber: this.doorNumber.trim(),
        apartment: this.apartment.trim() || undefined
      },
      items
    }).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['/app/orders'], { state: { successMessage: 'Orden creada exitosamente.' } });
      },
      error: (error) => {
        this.isSubmitting = false;
        this.errorMessage = error.error?.message || 'Error al crear la orden.';
      }
    });
  }

  onCancel() {
    this.router.navigate(['/app/orders']);
  }
}
