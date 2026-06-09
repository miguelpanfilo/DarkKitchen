import { Component, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { PromotionService } from '../../services/promotion.service';
import { FormCardComponent } from '../../../../shared/components/form-card/form-card.component';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';
import { SubmitButtonComponent } from '../../../../shared/components/submit-button/submit-button.component';
import { CancelButtonComponent } from '../../../../shared/components/cancel-button/cancel-button.component';
import { HasUnsavedChanges } from '../../../../core/guards/pending-changes.guard';

@Component({
  selector: 'app-create-promotion-page',
  imports: [CommonModule, FormsModule, FormCardComponent, StatusMessagesComponent,
    SubmitButtonComponent, CancelButtonComponent],
  templateUrl: './create-promotion-page.component.html',
  styleUrl: './create-promotion-page.component.css'
})
export class CreatePromotionPageComponent implements OnDestroy, HasUnsavedChanges {
  name = '';
  discountPercentage: number | null = null;
  startDate = '';
  endDate = '';

  submitted = false;
  isSubmitting = false;
  errorMessage = '';
  private saveSucceeded = false;

  private subs = new Subscription();

  constructor(private promotionService: PromotionService, private router: Router) {}

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  hasUnsavedChanges(): boolean {
    return !this.saveSucceeded && !!(this.name || this.discountPercentage !== null || this.startDate || this.endDate);
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
    this.subs.add(this.promotionService.create({
      name: this.name.trim(),
      discountPercentage: this.discountPercentage,
      startDate: this.startDate,
      endDate: this.endDate
    }).subscribe({
      next: (created) => {
        this.isSubmitting = false;
        this.saveSucceeded = true;
        this.router.navigate(['/home/promotions/edit', created.id]);
      },
      error: (error) => {
        this.isSubmitting = false;
        this.errorMessage = error.error?.message || 'Error al crear la promoción.';
      }
    }));
  }

  onCancel() {
    this.router.navigate(['/home/promotions']);
  }
}
