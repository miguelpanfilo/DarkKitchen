import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ProductCardComponent } from '../../components/product-card/product-card.component';
import { Product } from '../../models/product.model';
import { ProductFilters } from '../../models/product-filters.model';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';
import { AddButtonComponent } from '../../../../shared/components/add-button/add-button.component';
import { FilterButtonComponent } from '../../../../shared/components/filter-button/filter-button.component';
import { ClearButtonComponent } from '../../../../shared/components/clear-button/clear-button.component';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';
import { HasRoleDirective } from '../../../../shared/directives/has-role.directive';

@Component({
  selector: 'app-products-list-page',
  imports: [
    CommonModule, FormsModule, ProductCardComponent, StatusMessagesComponent,
    AddButtonComponent, FilterButtonComponent, ClearButtonComponent,
    ConfirmDialogComponent, HasRoleDirective
  ],
  templateUrl: './products-list-page.component.html',
  styleUrl: './products-list-page.component.css'
})
export class ProductsListPageComponent implements OnInit, OnDestroy {
  products: Product[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  filterName = '';
  filterCommercialLine = '';
  filterCategory = '';

  showConfirmDialog = false;
  productToDelete: Product | null = null;

  private subs = new Subscription();

  constructor(
    private productService: ProductService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit() {
    const nav = this.router.getCurrentNavigation()?.extras.state ?? history.state;
    if (nav?.successMessage) this.successMessage = nav.successMessage;
    this.loadProducts();
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  loadProducts() {
    this.isLoading = true;
    this.errorMessage = '';
    const filters: ProductFilters = {
      name: this.filterName || undefined,
      commercialLine: this.filterCommercialLine || undefined,
      category: this.filterCategory || undefined
    };
    this.subs.add(this.productService.getProducts(filters).subscribe({
      next: (products) => {
        this.isLoading = false;
        this.products = products;
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Error al cargar los productos.';
      }
    }));
  }

  onFiltrar() {
    this.loadProducts();
  }

  onLimpiar() {
    this.filterName = '';
    this.filterCommercialLine = '';
    this.filterCategory = '';
    this.loadProducts();
  }

  onAdd() {
    this.router.navigate(['/home/products/create']);
  }

  onEdit(product: Product) {
    this.router.navigate(['/home/products/edit', product.id]);
  }

  onDelete(product: Product) {
    this.productToDelete = product;
    this.showConfirmDialog = true;
  }

  onConfirmDelete() {
    if (!this.productToDelete) return;
    const id = this.productToDelete.id;
    this.subs.add(this.productService.delete(id).subscribe({
      next: () => {
        this.products = this.products.filter(p => p.id !== id);
        this.showConfirmDialog = false;
        this.productToDelete = null;
        this.successMessage = 'Producto eliminado correctamente.';
      },
      error: (error) => {
        this.showConfirmDialog = false;
        this.errorMessage = error.error?.message || 'Error al eliminar el producto.';
      }
    }));
  }

  onCancelDelete() {
    this.showConfirmDialog = false;
    this.productToDelete = null;
  }
}
