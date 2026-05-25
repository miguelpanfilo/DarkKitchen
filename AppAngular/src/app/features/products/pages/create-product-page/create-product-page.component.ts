import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';
import { SubmitButtonComponent } from '../../../../shared/components/submit-button/submit-button.component';
import { CancelButtonComponent } from '../../../../shared/components/cancel-button/cancel-button.component';

@Component({
  selector: 'app-create-product-page',
  imports: [CommonModule, FormsModule, StatusMessagesComponent, SubmitButtonComponent, CancelButtonComponent],
  templateUrl: './create-product-page.component.html',
  styleUrl: './create-product-page.component.css'
})
export class CreateProductPageComponent {
  code = '';
  name = '';
  description = '';
  commercialLine = '';
  category = '';
  price: number | null = null;

  isSubmitting = false;
  errorMessage = '';

  constructor(
    private productService: ProductService,
    private router: Router
  ) { }

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
    this.productService.create({
      code: this.code.trim(),
      name: this.name.trim(),
      description: this.description.trim(),
      commercialLine: this.commercialLine.trim(),
      category: this.category.trim(),
      price: this.price,
      photos: []
    }).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['/app/products'], { state: { successMessage: 'Producto creado correctamente.' } });
      },
      error: (error) => {
        this.isSubmitting = false;
        this.errorMessage = error.error?.message || 'Error al crear el producto.';
      }
    });
  }

  onCancel() {
    this.router.navigate(['/app/products']);
  }
}
