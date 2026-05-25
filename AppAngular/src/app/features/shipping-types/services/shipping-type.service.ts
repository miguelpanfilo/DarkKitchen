import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ShippingType } from '../models/shipping-type.model';

@Injectable({
  providedIn: 'root'
})
export class ShippingTypeService {
  private apiUrl = 'http://localhost:5254';

  constructor(private http: HttpClient) { }

  getAll(): Observable<ShippingType[]> {
    return this.http.get<ShippingType[]>(`${this.apiUrl}/api/v1/shipping-types`);
  }

  create(name: string, cost: number): Observable<ShippingType> {
    return this.http.post<ShippingType>(`${this.apiUrl}/api/v1/shipping-types`, { name, cost });
  }

  update(id: number, name?: string, cost?: number): Observable<ShippingType> {
    const body: { name?: string; cost?: number } = {};
    if (name !== undefined) body['name'] = name;
    if (cost !== undefined) body['cost'] = cost;
    return this.http.patch<ShippingType>(`${this.apiUrl}/api/v1/shipping-types/${id}`, body);
  }
}
