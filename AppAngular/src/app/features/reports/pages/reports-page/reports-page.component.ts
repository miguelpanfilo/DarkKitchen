import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReportService } from '../../services/report.service';
import { TopProduct, SalesReport } from '../../models/report.model';
import { FilterButtonComponent } from
  '../../../../shared/components/filter-button/filter-button.component';
import { ClearButtonComponent } from
  '../../../../shared/components/clear-button/clear-button.component';

@Component({
  selector: 'app-reports-page',
  imports: [CommonModule, FormsModule, FilterButtonComponent, ClearButtonComponent],
  templateUrl: './reports-page.component.html',
  styleUrl: './reports-page.component.css'
})
export class ReportsPageComponent {
  dateFrom = '';
  dateTo = '';
  topProducts: TopProduct[] = [];
  salesReport: SalesReport | null = null;
  errorMessage = '';

  constructor(private reportService: ReportService) { }

  onFiltrar() {
    this.errorMessage = '';

    if (!this.dateFrom || !this.dateTo) {
      this.errorMessage = 'Debés ingresar una fecha desde y una fecha hasta para filtrar.';
      return;
    }

    this.reportService.getTopProducts(this.dateFrom,
      this.dateTo).subscribe({
        next: (response) => { this.topProducts = response; },
        error: (err) => {
          this.errorMessage = err.error?.message || 'Error al cargar el reporte.'; }
        });
    this.reportService.getSalesReport(this.dateFrom,
      this.dateTo).subscribe({
        next: (response) => { this.salesReport = response; },
        error: (err) => {
          this.errorMessage = err.error?.message || 'Error al cargar el reporte.'; }
        });
  }

  onLimpiar() {
    this.dateFrom = '';
    this.dateTo = '';
    this.topProducts = [];
    this.salesReport = null;
    this.errorMessage = '';
  }
}