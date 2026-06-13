import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-filter-button',
  imports: [],
  templateUrl: './filter-button.component.html',
  styleUrl: './filter-button.component.css'
})
export class FilterButtonComponent {
  @Output() filtered = new EventEmitter<void>();

  onFilter() {
    this.filtered.emit();
  }
}
