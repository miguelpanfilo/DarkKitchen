import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { AuditLog } from '../models/audit-log.model';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuditLogService {

  private apiUrl = environment.webApiBaseUrl;

  constructor(private http: HttpClient) { }

  getAuditLogs(entityName: string, dateFrom: string, dateTo: string,
    entityId: string): Observable<AuditLog[]> {
    let params = new HttpParams();

    if (entityName) params = params.set('EntityName', entityName);
    if (dateFrom) params = params.set('DateTimeFrom', dateFrom + 'T00:00:00');
    if (dateTo) params = params.set('DateTimeTo', dateTo + 'T23:59:59');
    if (entityId) params = params.set('EntityId', entityId);

    return this.http.get<any[]>(`${this.apiUrl}/api/v1/audits`, { params }).pipe(
      map(response => response.map(item => ({
        dateTime: item.dateTime,
        entityName: item.entityName,
        entityId: item.entityId,
        changeDescription: item.changeDescription,
        responsibleUser: item.responsibleUser
      } as AuditLog)))
    );
  }
}