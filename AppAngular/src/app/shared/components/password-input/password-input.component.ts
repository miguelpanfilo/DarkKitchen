import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-password-input',
  imports: [CommonModule],
  templateUrl: './password-input.component.html',
  styleUrl: './password-input.component.css'
})
export class PasswordInputComponent {
  @Input() inputId = '';
  @Input() name = '';
  @Input() placeholder = '••••••••';
  @Input() hasError = false;
  @Input() value = '';
  @Output() valueChange = new EventEmitter<string>();

  show = false;

  onInput(event: Event) {
    const input = event.target as HTMLInputElement;
    this.valueChange.emit(input.value);
  }
}
