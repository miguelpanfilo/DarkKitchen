import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-status-messages',
  imports: [CommonModule],
  templateUrl: './status-messages.component.html',
  styleUrl: './status-messages.component.css'
})
export class StatusMessagesComponent {
  @Input() errorMessage = '';
  @Input() successMessage = '';
  @Input() isLoading = false;
  @Input() loadingText = 'Cargando...';
}
