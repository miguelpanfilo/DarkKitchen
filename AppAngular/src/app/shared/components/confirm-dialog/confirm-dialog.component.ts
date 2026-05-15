 import { Component, Input, Output, EventEmitter } from
  '@angular/core';

  @Component({
    selector: 'app-confirm-dialog',
    imports: [],
    templateUrl: './confirm-dialog.component.html',
    styleUrl: './confirm-dialog.component.css'
  })
  export class ConfirmDialogComponent {
    @Input() message = '¿Estás seguro?';
    @Input() visible = false;
    @Output() confirmed = new EventEmitter<void>();
    @Output() cancelled = new EventEmitter<void>();

    onConfirm() {
      this.confirmed.emit();
    }

    onCancel() {
      this.cancelled.emit();
    }
  }