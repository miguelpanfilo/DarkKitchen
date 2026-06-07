import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { Promotion } from '../models/promotion.model';
import { PromotionFilters } from '../models/promotion-filters.model';
import { PromotionPatchRequest } from '../models/promotion-patch-request.model';
import { environment } from '../../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PromotionService {
  private apiUrl = environment.webApiBaseUrl;

  constructor(private http: HttpClient) {}

  private mapPromotion(item: any): Promotion {
    return {
      id: item.id,
      name: item.name,
      discountPercentage: item.discountPercentage,
      startDate: item.startDate,
      endDate: item.endDate,
      products: (item.products ?? []).map((p: any) => ({ id: p.id, name: p.name }))
    };
  }

  getPromotions(filters: PromotionFilters): Observable<Promotion[]> {
    let params = new HttpParams();
    params = params.set('Date', filters.date);
    if (filters.commercialLine) params = params.set('CommercialLine', filters.commercialLine);
    if (filters.productId) params = params.set('ProductId', filters.productId);
    return this.http.get<any[]>(`${this.apiUrl}/api/v1/promotions`, { params }).pipe(
      map(response => response.map(item => this.mapPromotion(item)))
    );
  }

  create(request: { name: string; discountPercentage: number; startDate: string; endDate: string }): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(`${this.apiUrl}/api/v1/promotions`, request);
  }

  associateProduct(promotionId: number, productId: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/api/v1/promotions/${promotionId}/products/${productId}`, {});
  }

  disassociateProduct(promotionId: number, productId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/v1/promotions/${promotionId}/products/${productId}`);
  }

  update(id: number, patch: PromotionPatchRequest): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/api/v1/promotions/${id}`, patch);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/v1/promotions/${id}`);
  }
}
