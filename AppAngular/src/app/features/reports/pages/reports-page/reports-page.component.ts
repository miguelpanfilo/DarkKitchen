import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReportService } from '../../services/report.service';
import { TopProduct, SalesReport } from '../../models/report.model';
import { FilterButtonComponent } from '../../../../shared/components/filter-button/filter-button.component';
import { ClearButtonComponent } from '../../../../shared/components/clear-button/clear-button.component';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';
import { SafeImagePipe } from '../../../../shared/pipes/safe-image.pipe';

@Component({
  selector: 'app-reports-page',
  imports: [CommonModule, FormsModule, FilterButtonComponent, ClearButtonComponent, StatusMessagesComponent, SafeImagePipe],
  templateUrl: './reports-page.component.html',
  styleUrl: './reports-page.component.css'
})
export class ReportsPageComponent implements OnDestroy {
  dateFrom = '';
  dateTo = '';
  isLoading: boolean = false;
  topProducts: TopProduct[] = [];
  salesReport: SalesReport | null = null;
  errorMessage = '';

  constructor(private reportService: ReportService) { }

  private subs = new Subscription();

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  onFiltrar() {
    this.errorMessage = '';

    if (!this.dateFrom || !this.dateTo) {
      this.errorMessage = 'Debés ingresar una fecha desde y una fecha hasta para filtrar.';
      return;
    }
    this.isLoading = true;

    this.subs.add(this.reportService.getTopProducts(this.dateFrom,
      this.dateTo).subscribe({
        next: (response) => {
          this.topProducts = response;
          this.isLoading = false;
        },
        error: (err) => {
          this.isLoading = false;
          this.errorMessage = err.error?.message || 'Error al cargar el reporte.';
        }
      }));
    this.subs.add(this.reportService.getSalesReport(this.dateFrom,
      this.dateTo).subscribe({
        next: (response) => {
          this.salesReport = response;
          this.isLoading = false;
        },
        error: (err) => {
          this.isLoading = false;
          this.errorMessage = err.error?.message || 'Error al cargar el reporte.';
        }
      }));
  }

  onLimpiar() {
    this.dateFrom = '';
    this.dateTo = '';
    this.topProducts = [];
    this.salesReport = null;
    this.errorMessage = '';
  }
}