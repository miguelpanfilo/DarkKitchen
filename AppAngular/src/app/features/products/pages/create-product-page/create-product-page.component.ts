import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';
import { SubmitButtonComponent } from '../../../../shared/components/submit-button/submit-button.component';
import { CancelButtonComponent } from '../../../../shared/components/cancel-button/cancel-button.component';
import { FormCardComponent } from '../../../../shared/components/form-card/form-card.component';
import { PhotoRequest } from '../../models/product-photo.model';
import { HasUnsavedChanges } from '../../../../core/guards/pending-changes.guard';

@Component({
  selector: 'app-create-product-page',
  imports: [CommonModule, FormsModule, StatusMessagesComponent, SubmitButtonComponent, CancelButtonComponent, FormCardComponent],
  templateUrl: './create-product-page.component.html',
  styleUrl: './create-product-page.component.css'
})
export class CreateProductPageComponent implements OnDestroy, HasUnsavedChanges {
  code = '';
  name = '';
  description = '';
  commercialLine = '';
  category = '';
  price: number | null = null;
  photos: PhotoRequest[] = [];

  submitted = false;
  isSubmitting = false;
  errorMessage = '';
  private saveSucceeded = false;

  private subs = new Subscription();

  constructor(
    private productService: ProductService,
    private router: Router
  ) { }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  hasUnsavedChanges(): boolean {
    return !this.saveSucceeded && !!(this.code || this.name || this.description || this.commercialLine || this.category || this.price !== null || this.photos.length > 0);
  }

  onPhotosSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files) return;

    const files = Array.from(input.files);

    if (this.photos.length + files.length > 3) {
      this.errorMessage = 'Podés subir un máximo de 3 fotos.';
      input.value = '';
      return;
    }

    files.forEach(file => {
      if (!file.name.toLowerCase().endsWith('.jpg')) {
        this.errorMessage = 'Solo se permiten fotos en formato JPG.';
        return;
      }
      if (file.size > 500 * 1024) {
        this.errorMessage = 'Cada foto no puede superar los 500 KB.';
        return;
      }

      const reader = new FileReader();
      reader.onload = () => {
        const base64 = (reader.result as string).split(',')[1];
        this.photos.push({
          fileName: file.name.replace(/\.[^.]+$/, ''),
          extension: 'jpg',
          sizeInBytes: file.size,
          base64
        });
      };
      reader.readAsDataURL(file);
    });

    input.value = '';
  }

  removePhoto(index: number) {
    this.photos.splice(index, 1);
  }

  getPhotoPreview(photo: PhotoRequest): string {
    return `data:image/jpg;base64,${photo.base64}`;
  }

  onSubmit() {
    this.submitted = true;
    this.errorMessage = '';
    if (!this.code.trim() || !this.name.trim() || !this.description.trim() ||
        !this.commercialLine.trim() || !this.category.trim() ||
        this.price === null || this.price <= 0) {
      return;
    }
    if (this.photos.length === 0) {
      this.errorMessage = 'Agregá al menos una foto.';
      return;
    }
    this.isSubmitting = true;
    this.subs.add(this.productService.create({
      code: this.code.trim(),
      name: this.name.trim(),
      description: this.description.trim(),
      commercialLine: this.commercialLine.trim(),
      category: this.category.trim(),
      price: this.price,
      photos: this.photos
    }).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.saveSucceeded = true;
        this.router.navigate(['/home/products'], { state: { successMessage: 'Producto creado correctamente.' } });
      },
      error: (error) => {
        this.isSubmitting = false;
        this.errorMessage = error.error?.message || 'Error al crear el producto.';
      }
    }));
  }

  onCancel() {
    this.router.navigate(['/home/products']);
  }
}
