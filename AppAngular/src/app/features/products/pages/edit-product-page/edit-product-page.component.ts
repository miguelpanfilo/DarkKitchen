import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';
import { SubmitButtonComponent } from '../../../../shared/components/submit-button/submit-button.component';
import { CancelButtonComponent } from '../../../../shared/components/cancel-button/cancel-button.component';

@Component({
  selector: 'app-edit-product-page',
  imports: [CommonModule, FormsModule, StatusMessagesComponent, SubmitButtonComponent, CancelButtonComponent],
  templateUrl: './edit-product-page.component.html',
  styleUrl: './edit-product-page.component.css'
})
export class EditProductPageComponent implements OnInit {
  code = '';
  name = '';
  description = '';
  commercialLine = '';
  category = '';
  price: number | null = null;
  isActive = true;

  isLoading = false;
  isSubmitting = false;
  errorMessage = '';

  private productId = 0;

  constructor(
    private productService: ProductService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {
    this.productId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadProduct();
  }

  loadProduct() {
    this.isLoading = true;
    this.errorMessage = '';
    this.productService.getProducts().subscribe({
      next: (products) => {
        this.isLoading = false;
        const product = products.find(p => p.id === this.productId);
        if (!product) {
          this.errorMessage = 'Producto no encontrado.';
          return;
        }
        this.code = product.code;
        this.name = product.name;
        this.description = product.description;
        this.commercialLine = product.commercialLine;
        this.category = product.category;
        this.price = product.price;
        this.isActive = product.isActive;
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Error al cargar el producto.';
      }
    });
  }

  onSubmit() {
    this.errorMessage = '';
    if (!this.code.trim() || !this.name.trim() || !this.description.trim() ||
        !this.commercialLine.trim() || !this.category.trim()) {
      this.errorMessage = 'Todos los campos son obligatorios.';
      return;
    }
    if (this.price === null || this.price <= 0) {
      this.errorMessage = 'El precio debe ser mayor a cero.';
      return;
    }
    this.isSubmitting = true;
    this.productService.update(this.productId, {
      code: this.code.trim(),
      name: this.name.trim(),
      description: this.description.trim(),
      commercialLine: this.commercialLine.trim(),
      category: this.category.trim(),
      price: this.price,
      isActive: this.isActive
    }).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['/app/products'], { state: { successMessage: 'Producto actualizado correctamente.' } });
      },
      error: (error) => {
        this.isSubmitting = false;
        this.errorMessage = error.error?.message || 'Error al actualizar el producto.';
      }
    });
  }

  onCancel() {
    this.router.navigate(['/app/products']);
  }
}
