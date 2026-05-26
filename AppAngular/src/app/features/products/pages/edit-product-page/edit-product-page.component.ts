import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';
import { SubmitButtonComponent } from '../../../../shared/components/submit-button/submit-button.component';
import { CancelButtonComponent } from '../../../../shared/components/cancel-button/cancel-button.component';
import { FormCardComponent } from '../../../../shared/components/form-card/form-card.component';
import { PatchPhotoRequest } from '../../models/product-photo.model';

@Component({
  selector: 'app-edit-product-page',
  imports: [CommonModule, FormsModule, StatusMessagesComponent, SubmitButtonComponent, CancelButtonComponent, FormCardComponent],
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
  photos: PatchPhotoRequest[] = [];

  submitted = false;
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
        this.photos = product.photos.map(p => ({ path: p.path }));
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Error al cargar el producto.';
      }
    });
  }

  getPhotoLabel(photo: PatchPhotoRequest): string {
    return photo.path ? photo.path.split('/').pop() ?? photo.path : `${photo.fileName}.${photo.extension}`;
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

  onSubmit() {
    this.submitted = true;
    this.errorMessage = '';
    if (!this.code.trim() || !this.name.trim() || !this.description.trim() ||
        !this.commercialLine.trim() || !this.category.trim() ||
        this.price === null || this.price <= 0) {
      return;
    }
    if (this.photos.length === 0) {
      this.errorMessage = 'El producto debe tener al menos una foto.';
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
      isActive: this.isActive,
      photos: this.photos
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
