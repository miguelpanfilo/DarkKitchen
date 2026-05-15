import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuditLogService {

  private apiUrl = 'http://localhost:5254';

  constructor(private http: HttpClient) { }

  getAuditLogs(entityName: string, dateFrom: string, dateTo: string,
  entityId: string): Observable<any[]> {
    let params = new HttpParams();

    if (entityName) params = params.set('EntityName', entityName);
    if (dateFrom) params = params.set('DateTimeFrom', dateFrom);
    if (dateTo) params = params.set('DateTimeTo', dateTo);
    if (entityId) params = params.set('EntityId', entityId);

    return this.http.get<any[]>(`${this.apiUrl}/api/v1/audits`, {
      headers: { 'X-Session-Id': localStorage.getItem('sessionId') || ''
   },
      params
    });
  }
}