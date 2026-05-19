import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReportService } from '../../services/report.service';
import { TopProduct, SalesReport } from '../../models/report.model';
import { FilterButtonComponent } from
  '../../../../shared/components/filter-button/filter-button.component';
import { ClearButtonComponent } from
  '../../../../shared/components/clear-button/clear-button.component';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';

@Component({
  selector: 'app-reports-page',
  imports: [CommonModule, FormsModule, FilterButtonComponent, ClearButtonComponent, StatusMessagesComponent],
  templateUrl: './reports-page.component.html',
  styleUrl: './reports-page.component.css'
})
export class ReportsPageComponent {
  dateFrom = '';
  dateTo = '';
  isLoading: boolean = false;
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
    this.isLoading = true;

    this.reportService.getTopProducts(this.dateFrom,
      this.dateTo).subscribe({
        next: (response) => { 
          this.topProducts = response;
          this.isLoading = false;
        },
        error: (err) => {
          this.isLoading = false;
          this.errorMessage = err.error?.message || 'Error al cargar el reporte.'; }
        });
    this.reportService.getSalesReport(this.dateFrom,
      this.dateTo).subscribe({
        next: (response) => { 
          this.salesReport = response;
          this.isLoading = false;
        },
        error: (err) => {
          this.isLoading = false;
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