import { Component, Output, EventEmitter } from '@angular/core';

  @Component({
    selector: 'app-clear-button',
    imports: [],
    templateUrl: './clear-button.component.html',
    styleUrl: './clear-button.component.css'
  })
  export class ClearButtonComponent {
    @Output() cleared = new EventEmitter<void>();

    onClick() {
      this.cleared.emit();
    }
  }