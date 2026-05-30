import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FilterButtonComponent } from
  '../../../../shared/components/filter-button/filter-button.component';
import { AuditLogService } from '../../services/audit-log.service';
import { AuditLog } from '../../models/audit-log.model';
import { ClearButtonComponent } from
  '../../../../shared/components/clear-button/clear-button.component';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';

@Component({
  selector: 'app-audit-logs-page',
  imports: [CommonModule, FormsModule, FilterButtonComponent, ClearButtonComponent, StatusMessagesComponent],
  templateUrl: './audit-logs-page.component.html',
  styleUrl: './audit-logs-page.component.css'
})
export class AuditLogsPageComponent implements OnDestroy {
  fechaDesde = '';
  fechaHasta = '';
  entidad = '';
  entityId = '';
  isLoading: boolean = false;
  auditLogs: AuditLog[] = [];
  errorMessage = '';

  constructor(private auditLogService: AuditLogService) { }

  private subs = new Subscription();

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  onFiltrar() {
    this.errorMessage = '';
    this.isLoading = true;
    this.subs.add(this.auditLogService.getAuditLogs(this.entidad, this.fechaDesde, this.fechaHasta, this.entityId).subscribe({
      next: (response) => {
        this.auditLogs = response;
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message ||
          'Error al cargar las auditorías.';
      }
    }));
  }
  onLimpiar() {
    this.fechaDesde = '';
    this.fechaHasta = '';
    this.entidad = '';
    this.entityId = '';
    this.auditLogs = [];
    this.errorMessage = '';
  }
}
