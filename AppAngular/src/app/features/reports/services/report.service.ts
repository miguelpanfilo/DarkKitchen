import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { TopProduct, SalesReport, SalesPeriod, SalesCustomer } from '../models/report.model';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ReportService {

  private apiUrl = environment.webApiBaseUrl;

  constructor(private http: HttpClient) { }

  getTopProducts(dateFrom: string, dateTo: string): Observable<TopProduct[]> {
    let params = new HttpParams();
    if (dateFrom) params = params.set('dateFrom', dateFrom + 'T00:00:00');
    if (dateTo) params = params.set('dateTo', dateTo + 'T23:59:59');
    return this.http.get<any[]>(`${this.apiUrl}/api/v1/reports/top-products`, { params }).pipe(
      map(response => response.map(item => ({
        code: item.code,
        name: item.name,
        quantitySold: item.quantitySold,
        images: (item.images ?? []).map((img: string) => this.apiUrl + img)
      } as TopProduct)))
    );
  }

  getSalesReport(dateFrom: string, dateTo: string): Observable<SalesReport> {
    let params = new HttpParams();
    if (dateFrom) params = params.set('dateFrom', dateFrom + 'T00:00:00');
    if (dateTo) params = params.set('dateTo', dateTo + 'T23:59:59');
    return this.http.get<any>(`${this.apiUrl}/api/v1/reports/sales`, { params }).pipe(
      map(item => ({
        periods: item.periods.map((p: any) => ({
          year: p.year,
          month: p.month,
          monthTotal: p.monthTotal,
          customers: p.customers.map((c: any) => ({
            customer: c.customer,
            totalSales: c.totalSales
          } as SalesCustomer))
        } as SalesPeriod)),
        grandTotal: item.grandTotal
      } as SalesReport))
    );
  }
}