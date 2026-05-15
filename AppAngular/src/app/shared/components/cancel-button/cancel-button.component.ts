import { Component, EventEmitter, Output } from '@angular/core';

  @Component({
    selector: 'app-cancel-button',
    imports: [],
    templateUrl: './cancel-button.component.html',
    styleUrl: './cancel-button.component.css'
  })
  export class CancelButtonComponent {
    @Output() cancelled = new EventEmitter<void>();

    onCancel() {
      this.cancelled.emit();
    }
  }