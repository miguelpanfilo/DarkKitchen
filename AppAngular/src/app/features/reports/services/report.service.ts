import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReportService {

  private apiUrl = 'http://localhost:5254';

  constructor(private http: HttpClient) { }

  getTopProducts(dateFrom: string, dateTo: string): Observable<any[]> {
    let params = new HttpParams();
    if (dateFrom) params = params.set('dateFrom', dateFrom);
    if (dateTo) params = params.set('dateTo', dateTo);
    return this.http.get<any[]>(`${this.apiUrl}/api/v1/reports/top-products`, {
      headers: { 'X-Session-Id': localStorage.getItem('sessionId') || '' },
      params
    });
  }

  getSalesReport(dateFrom: string, dateTo: string): Observable<any> {
    let params = new HttpParams();
    if (dateFrom) params = params.set('dateFrom', dateFrom);
    if (dateTo) params = params.set('dateTo', dateTo);

    return this.http.get<any>(`${this.apiUrl}/api/v1/reports/sales`, {
      headers: {
        'X-Session-Id': localStorage.getItem('sessionId') ||
          ''
      },
      params
    });
  }
}