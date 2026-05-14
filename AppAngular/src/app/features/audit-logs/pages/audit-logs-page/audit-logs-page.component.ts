import { Component } from '@angular/core';
  import { FormsModule } from '@angular/forms';
  import { FilterButtonComponent } from
  '../../../../shared/components/filter-button/filter-button.component';

  @Component({
    selector: 'app-audit-logs-page',
    imports: [FormsModule, FilterButtonComponent],
    templateUrl: './audit-logs-page.component.html',
    styleUrl: './audit-logs-page.component.css'
  })
  export class AuditLogsPageComponent {
    fechaDesde = '';
    fechaHasta = '';
    entidad = '';

    onFiltrar() {
      console.log('Filtrando...', this.fechaDesde, this.fechaHasta,
  this.entidad);
    }
  }