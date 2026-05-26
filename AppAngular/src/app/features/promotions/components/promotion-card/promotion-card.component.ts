import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Promotion } from '../../models/promotion.model';
import { HasRoleDirective } from '../../../../shared/directives/has-role.directive';
import { EditButtonComponent } from '../../../../shared/components/edit-button/edit-button.component';
import { DeleteButtonComponent } from '../../../../shared/components/delete-button/delete-button.component';

@Component({
  selector: 'app-promotion-card',
  imports: [CommonModule, HasRoleDirective, EditButtonComponent, DeleteButtonComponent],
  templateUrl: './promotion-card.component.html',
  styleUrl: './promotion-card.component.css'
})
export class PromotionCardComponent {
  @Input() promotion!: Promotion;
  @Output() edit = new EventEmitter<Promotion>();
  @Output() delete = new EventEmitter<Promotion>();

  onEdit() { this.edit.emit(this.promotion); }
  onDelete() { this.delete.emit(this.promotion); }
}
