import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Product } from '../../models/product.model';
import { HasRoleDirective } from '../../../../shared/directives/has-role.directive';
import { EditButtonComponent } from '../../../../shared/components/edit-button/edit-button.component';
import { DeleteButtonComponent } from '../../../../shared/components/delete-button/delete-button.component';

@Component({
  selector: 'app-product-card',
  imports: [CommonModule, HasRoleDirective, EditButtonComponent, DeleteButtonComponent],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.css'
})
export class ProductCardComponent {
  @Input() product!: Product;
  @Output() edit = new EventEmitter<Product>();
  @Output() delete = new EventEmitter<Product>();

  onEdit() {
    this.edit.emit(this.product);
  }

  onDelete() {
    this.delete.emit(this.product);
  }
}
