import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { Order } from '../models/order.model';
import { OrderFilters } from '../models/order-filters.model';
import { CreateOrderRequest } from '../models/create-order-request.model';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = 'http://localhost:5254';

  constructor(private http: HttpClient) { }

  getOrders(filters?: OrderFilters): Observable<Order[]> {
    let params = new HttpParams();
    if (filters?.dateFrom) params = params.set('dateFrom', filters.dateFrom);
    if (filters?.dateTo) params = params.set('dateTo', filters.dateTo);
    if (filters?.status) params = params.set('status', filters.status);
    if (filters?.street) params = params.set('street', filters.street);
    return this.http.get<Order[]>(`${this.apiUrl}/api/v1/orders`, { params });
  }

  getById(id: number): Observable<Order> {
    return this.http.get<Order>(`${this.apiUrl}/api/v1/orders/${id}`);
  }

  create(request: CreateOrderRequest): Observable<Order> {
    return this.http.post<Order>(`${this.apiUrl}/api/v1/orders`, request);
  }

  prepare(id: number): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/api/v1/orders/${id}/prepare`, {});
  }

  cancel(id: number): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/api/v1/orders/${id}/cancel`, {});
  }

  dispatch(id: number): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/api/v1/orders/${id}/dispatch`, {});
  }

  deliver(id: number): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/api/v1/orders/${id}/deliver`, {});
  }

  markNotDelivered(id: number): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/api/v1/orders/${id}/not-delivered`, {});
  }

  delay(id: number): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/api/v1/orders/${id}/delay`, {});
  }
}
