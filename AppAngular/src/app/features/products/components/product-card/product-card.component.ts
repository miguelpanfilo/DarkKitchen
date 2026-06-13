import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Product } from '../../models/product.model';
import { HasRoleDirective } from '../../../../shared/directives/has-role.directive';
import { EditButtonComponent } from '../../../../shared/components/edit-button/edit-button.component';
import { DeleteButtonComponent } from '../../../../shared/components/delete-button/delete-button.component';
import { SafeImagePipe } from '../../../../shared/pipes/safe-image.pipe';

@Component({
  selector: 'app-product-card',
  imports: [CommonModule, HasRoleDirective, EditButtonComponent, DeleteButtonComponent, SafeImagePipe],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.css'
})
export class ProductCardComponent {
  @Input() product!: Product;
  @Output() edit = new EventEmitter<Product>();
  @Output() delete = new EventEmitter<Product>();

  currentPhotoIndex = 0;

  get currentPhotoPath(): string {
    return this.product.photos[this.currentPhotoIndex]?.path;
  }

  previousPhoto(event: Event) {
    event.stopPropagation();

    if (this.currentPhotoIndex === 0) {
      this.currentPhotoIndex = this.product.photos.length - 1;
    } else {
      this.currentPhotoIndex--;
    }
  }

  nextPhoto(event: Event) {
    event.stopPropagation();

    if (this.currentPhotoIndex === this.product.photos.length - 1) {
      this.currentPhotoIndex = 0;
    } else {
      this.currentPhotoIndex++;
    }
  }

  onEdit() {
    this.edit.emit(this.product);
  }

  onDelete() {
    this.delete.emit(this.product);
  }
}
