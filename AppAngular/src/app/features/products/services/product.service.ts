import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { Product } from '../models/product.model';
import { ProductFilters } from '../models/product-filters.model';
import { CreateProductRequest } from
  '../models/create-product-request.model';
import { UpdateProductRequest } from
  '../models/update-product-request.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = 'http://localhost:5254';

  constructor(private http: HttpClient) { }

  private mapProduct(item: any): Product {
    return {
      id: item.id,
      code: item.code,
      name: item.name,
      description: item.description,
      commercialLine: item.commercialLine,
      category: item.category,
      price: item.price,
      isActive: item.isActive,
      photos: (item.photos ?? []).map((p: any) => ({ path: this.apiUrl + p.path }))
    };
  }

  getProducts(filters?: ProductFilters): Observable<Product[]> {
    let params = new HttpParams();
    if (filters?.name) params = params.set('Name', filters.name);
    if (filters?.commercialLine) params = params.set('CommercialLine',
      filters.commercialLine);
    if (filters?.category) params = params.set('Category',
      filters.category);
    return this.http.get<any[]>(`${this.apiUrl}/api/v1/products`, {
      params
    }).pipe(
      map(response => response.map(item => this.mapProduct(item)))
    );
  }

  create(request: CreateProductRequest): Observable<Product> {
    return this.http.post<any>(`${this.apiUrl}/api/v1/products`,
      request).pipe(
        map(item => this.mapProduct(item))
      );
  }

  update(id: number, request: UpdateProductRequest):
    Observable<Product> {
    return this.http.patch<any>(`${this.apiUrl}/api/v1/products/${id}`,
      request).pipe(
        map(item => this.mapProduct(item))
      );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/v1/products/${id}`);
  }
}
