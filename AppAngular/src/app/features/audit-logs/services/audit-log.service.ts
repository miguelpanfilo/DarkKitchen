import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { AuditLog } from '../models/audit-log.model';

@Injectable({
  providedIn: 'root'
})
export class AuditLogService {

  private apiUrl = 'http://localhost:5254';

  constructor(private http: HttpClient) { }

  getAuditLogs(entityName: string, dateFrom: string, dateTo: string,
    entityId: string): Observable<AuditLog[]> {
    let params = new HttpParams();

    if (entityName) params = params.set('EntityName', entityName);
    if (dateFrom) params = params.set('DateTimeFrom', dateFrom);
    if (dateTo) params = params.set('DateTimeTo', dateTo);
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