import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-status-messages',
  imports: [CommonModule],
  templateUrl: './status-messages.component.html',
  styleUrl: './status-messages.component.css'
})
export class StatusMessagesComponent implements OnChanges {
  @Input() errorMessage = '';
  @Input() successMessage = '';
  @Input() isLoading = false;
  @Input() loadingText = 'Cargando...';

  showSuccess = false;
  showError = false;

  ngOnChanges(changes: SimpleChanges) {
    if (changes['successMessage']?.currentValue) {
      this.showSuccess = true;
      setTimeout(() => this.showSuccess = false, 3000);
    }
    if (changes['errorMessage']?.currentValue) {
      this.showError = true;
      setTimeout(() => this.showError = false, 3000);
    }
  }
}
