import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FilterButtonComponent } from
  '../../../../shared/components/filter-button/filter-button.component';
import { AuditLogService } from '../../services/audit-log.service';
import { AuditLog } from '../../models/audit-log.model';
import { ClearButtonComponent } from
  '../../../../shared/components/clear-button/clear-button.component';
@Component({
  selector: 'app-audit-logs-page',
  imports: [CommonModule, FormsModule, FilterButtonComponent, ClearButtonComponent],
  templateUrl: './audit-logs-page.component.html',
  styleUrl: './audit-logs-page.component.css'
})
export class AuditLogsPageComponent {
  fechaDesde = '';
  fechaHasta = '';
  entidad = '';
  entityId = '';
  auditLogs: AuditLog[] = [];
  errorMessage = '';

  constructor(private auditLogService: AuditLogService) { }

  onFiltrar() {
    this.errorMessage = '';
    this.auditLogService.getAuditLogs(this.entidad, this.fechaDesde, this.fechaHasta, this.entityId).subscribe({
        next: (response) => { this.auditLogs = response; },
        error: (err) => {
          this.errorMessage = err.error?.message ||
            'Error al cargar las auditorías.';
        }
      });
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
