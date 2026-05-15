import { Component, Output, EventEmitter } from '@angular/core';

  @Component({
    selector: 'app-edit-button',
    imports: [],
    templateUrl: './edit-button.component.html',
    styleUrl: './edit-button.component.css'
  })
  export class EditButtonComponent {
    @Output() clicked = new EventEmitter<void>();

    onClick() {
      this.clicked.emit();
    }
  }