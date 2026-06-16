import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { ShippingType } from '../models/shipping-type.model';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ShippingTypeService {
  private apiUrl = environment.webApiBaseUrl;

  constructor(private http: HttpClient) { }

  getAll(): Observable<ShippingType[]> {
    return this.http.get<any[]>(`${this.apiUrl}/api/v1/shipping-types`).pipe(
      map(response => response.map(item => ({
        id: item.id,
        name: item.name,
        cost: item.cost
      } as ShippingType)))
    );
  }

  create(name: string, cost: number): Observable<ShippingType> {
    return this.http.post<any>(`${this.apiUrl}/api/v1/shipping-types`, { name, cost }).pipe(
      map(item => ({
        id: item.id,
        name: item.name,
        cost: item.cost
      } as ShippingType))
    );
  }

  update(id: number, name?: string, cost?: number): Observable<ShippingType> {
    const body: { name?: string; cost?: number } = {};
    if (name !== undefined) body['name'] = name;
    if (cost !== undefined) body['cost'] = cost;
    return this.http.patch<any>(`${this.apiUrl}/api/v1/shipping-types/${id}`, body).pipe(
      map(item => ({
        id: item.id,
        name: item.name,
        cost: item.cost
      } as ShippingType))
    );
  }
}
