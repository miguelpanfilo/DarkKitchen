import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ShippingTypeService } from '../../services/shipping-type.service';
import { ShippingType } from '../../models/shipping-type.model';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';
import { EditButtonComponent } from '../../../../shared/components/edit-button/edit-button.component';

@Component({
  selector: 'app-shipping-types-page',
  imports: [CommonModule, FormsModule, StatusMessagesComponent, EditButtonComponent],
  templateUrl: './shipping-types-page.component.html',
  styleUrl: './shipping-types-page.component.css'
})
export class ShippingTypesPageComponent implements OnInit {
  shippingTypes: ShippingType[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  newName = '';
  newCost: number | null = null;
  isCreating = false;

  editingId: number | null = null;
  editName = '';
  editCost: number | null = null;
  isSaving = false;

  constructor(private shippingTypeService: ShippingTypeService) { }

  ngOnInit() {
    this.load();
  }

  load() {
    this.isLoading = true;
    this.errorMessage = '';
    this.shippingTypeService.getAll().subscribe({
      next: (types) => {
        this.isLoading = false;
        this.shippingTypes = types;
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Error al cargar los tipos de envío.';
      }
    });
  }

  onCreate() {
    this.errorMessage = '';
    this.successMessage = '';
    if (!this.newName.trim()) {
      this.errorMessage = 'El nombre es obligatorio.';
      return;
    }
    if (this.newCost === null || this.newCost < 0) {
      this.errorMessage = 'El costo debe ser un valor positivo.';
      return;
    }
    this.isCreating = true;
    this.shippingTypeService.create(this.newName.trim(), this.newCost).subscribe({
      next: (created) => {
        this.isCreating = false;
        this.shippingTypes.push(created);
        this.newName = '';
        this.newCost = null;
        this.showSuccessMessage('Tipo de envío creado correctamente.');
      },
      error: (error) => {
        this.isCreating = false;
        this.errorMessage = error.error?.message || 'Error al crear el tipo de envío.';
      }
    });
  }

  onEdit(type: ShippingType) {
    this.editingId = type.id;
    this.editName = type.name;
    this.editCost = type.cost;
    this.errorMessage = '';
    this.successMessage = '';
  }

  onCancelEdit() {
    this.editingId = null;
  }

  onSaveEdit(type: ShippingType) {
    this.errorMessage = '';
    if (!this.editName.trim()) {
      this.errorMessage = 'El nombre no puede estar vacío.';
      return;
    }
    if (this.editCost === null || this.editCost < 0) {
      this.errorMessage = 'El costo debe ser un valor positivo.';
      return;
    }
    this.isSaving = true;
    this.shippingTypeService.update(type.id, this.editName.trim(), this.editCost).subscribe({
      next: (updated) => {
        this.isSaving = false;
        const index = this.shippingTypes.findIndex(t => t.id === type.id);
        if (index !== -1) this.shippingTypes[index] = updated;
        this.editingId = null;
        this.showSuccessMessage('Tipo de envío actualizado.');
      },
      error: (error) => {
        this.isSaving = false;
        this.errorMessage = error.error?.message || 'Error al actualizar el tipo de envío.';
      }
    });
  }

  private showSuccessMessage(message: string): void {
    this.successMessage = message;

    setTimeout(() => {
      this.successMessage = '';
    }, 3000);
  }
}
