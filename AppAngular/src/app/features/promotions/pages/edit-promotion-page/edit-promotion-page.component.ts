import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { PromotionService } from '../../services/promotion.service';
import { ProductService } from '../../../products/services/product.service';
import { Product } from '../../../products/models/product.model';
import { FormCardComponent } from '../../../../shared/components/form-card/form-card.component';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';
import { SubmitButtonComponent } from '../../../../shared/components/submit-button/submit-button.component';
import { CancelButtonComponent } from '../../../../shared/components/cancel-button/cancel-button.component';

@Component({
  selector: 'app-edit-promotion-page',
  imports: [CommonModule, FormsModule, FormCardComponent, StatusMessagesComponent,
    SubmitButtonComponent, CancelButtonComponent],
  templateUrl: './edit-promotion-page.component.html',
  styleUrl: './edit-promotion-page.component.css'
})
export class EditPromotionPageComponent implements OnInit, OnDestroy {
  private promotionId = 0;
  name = '';
  discountPercentage: number | null = null;
  startDate = '';
  endDate = '';

  allProducts: Product[] = [];
  associatedIds = new Set<number>();
  isLoadingProducts = false;

  submitted = false;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';

  private subs = new Subscription();

  constructor(
    private promotionService: PromotionService,
    private productService: ProductService,
    private router: Router
  ) {}

  ngOnInit() {
    const promotion = this.router.getCurrentNavigation()?.extras.state?.['promotion']
      ?? history.state?.promotion;

    if (promotion) {
      this.promotionId = promotion.id;
      this.name = promotion.name;
      this.discountPercentage = promotion.discountPercentage;
      this.startDate = promotion.startDate;
      this.endDate = promotion.endDate;
      this.associatedIds = new Set((promotion.products ?? []).map((p: any) => p.id));
    }

    this.isLoadingProducts = true;
    this.subs.add(this.productService.getProducts().subscribe({
      next: (products) => {
        this.isLoadingProducts = false;
        this.allProducts = products.filter(p => p.isActive);
      },
      error: () => { this.isLoadingProducts = false; }
    }));
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  isAssociated(id: number): boolean {
    return this.associatedIds.has(id);
  }

  onToggleProduct(id: number, checked: boolean) {
    this.errorMessage = '';
    if (checked) {
      this.subs.add(this.promotionService.associateProduct(this.promotionId, id).subscribe({
        next: () => this.associatedIds.add(id),
        error: (err) => { this.errorMessage = err.error?.message || 'Error al asociar el producto.'; }
      }));
    } else {
      this.subs.add(this.promotionService.disassociateProduct(this.promotionId, id).subscribe({
        next: () => this.associatedIds.delete(id),
        error: (err) => { this.errorMessage = err.error?.message || 'Error al desasociar el producto.'; }
      }));
    }
  }

  onSubmit() {
    this.submitted = true;
    this.errorMessage = '';
    if (!this.name.trim() || this.discountPercentage === null ||
        this.discountPercentage <= 0 || this.discountPercentage > 100 ||
        !this.startDate || !this.endDate) {
      return;
    }
    this.isSubmitting = true;
    this.subs.add(this.promotionService.update(this.promotionId, {
      name: this.name.trim(),
      discountPercentage: this.discountPercentage,
      startDate: this.startDate,
      endDate: this.endDate
    }).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['/home/promotions'], { state: { successMessage: 'Promoción actualizada correctamente.' } });
      },
      error: (error) => {
        this.isSubmitting = false;
        this.errorMessage = error.error?.message || 'Error al actualizar la promoción.';
      }
    }));
  }

  onCancel() {
    this.router.navigate(['/home/promotions']);
  }
}
