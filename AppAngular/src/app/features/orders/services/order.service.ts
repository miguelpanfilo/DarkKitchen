import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { Order, OrderClient, OrderAddress } from '../models/order.model';
import { OrderItem } from '../models/order-item.model';
import { OrderFilters } from '../models/order-filters.model';
import { CreateOrderRequest } from '../models/create-order-request.model';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = environment.webApiBaseUrl;

  constructor(private http: HttpClient) { }

  private mapOrder(item: any): Order {
    return {
      id: item.id,
      orderNumber: item.orderNumber,
      client: {
        id: item.client.id,
        name: item.client.name,
        lastName: item.client.lastName,
        email: item.client.email
      } as OrderClient,
      deliveryType: item.deliveryType,
      address: {
        street: item.address.street,
        doorNumber: item.address.doorNumber,
        apartment: item.address.apartment
      } as OrderAddress,
      items: item.items.map((i: any) => ({
        productId: i.productId,
        productName: i.productName,
        quantity: i.quantity,
        unitPrice: i.unitPrice,
        discountApplied: i.discountApplied,
        lineTotal: i.lineTotal
      } as OrderItem)),
      itemsCount: item.itemsCount,
      status: item.status,
      createdAt: item.createdAt + 'Z',
      statusUpdatedAt: item.statusUpdatedAt + 'Z',
      subtotal: item.subtotal,
      shippingCost: item.shippingCost,
      iva: item.iva,
      ivaPercentage: item.ivaPercentage,
      total: item.total
    };
  }

  getOrders(filters?: OrderFilters): Observable<Order[]> {
    let params = new HttpParams();
    if (filters?.dateFrom) params = params.set('dateFrom', filters.dateFrom + 'T00:00:00');
    if (filters?.dateTo) params = params.set('dateTo', filters.dateTo + 'T23:59:59');
    if (filters?.status) params = params.set('status', filters.status);
    if (filters?.street) params = params.set('street', filters.street);
    return this.http.get<any[]>(`${this.apiUrl}/api/v1/orders`, { params }).pipe(
      map(response => response.map(item => this.mapOrder(item)))
    );
  }

  getById(id: number): Observable<Order> {
    return this.http.get<any>(`${this.apiUrl}/api/v1/orders/${id}`).pipe(
      map(item => this.mapOrder(item))
    );
  }

  create(request: CreateOrderRequest): Observable<Order> {
    return this.http.post<any>(`${this.apiUrl}/api/v1/orders`, request).pipe(
      map(item => this.mapOrder(item))
    );
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
