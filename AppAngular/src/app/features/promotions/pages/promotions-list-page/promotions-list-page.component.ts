import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { PromotionService } from '../../services/promotion.service';
import { Promotion } from '../../models/promotion.model';
import { PromotionFilters } from '../../models/promotion-filters.model';
import { PromotionCardComponent } from '../../components/promotion-card/promotion-card.component';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';
import { AddButtonComponent } from '../../../../shared/components/add-button/add-button.component';
import { FilterButtonComponent } from '../../../../shared/components/filter-button/filter-button.component';
import { ClearButtonComponent } from '../../../../shared/components/clear-button/clear-button.component';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';
import { HasRoleDirective } from '../../../../shared/directives/has-role.directive';

@Component({
  selector: 'app-promotions-list-page',
  imports: [
    CommonModule, FormsModule, PromotionCardComponent, StatusMessagesComponent,
    AddButtonComponent, FilterButtonComponent, ClearButtonComponent,
    ConfirmDialogComponent, HasRoleDirective
  ],
  templateUrl: './promotions-list-page.component.html',
  styleUrl: './promotions-list-page.component.css'
})
export class PromotionsListPageComponent implements OnInit, OnDestroy {
  promotions: Promotion[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  filterDate = new Date().toISOString().split('T')[0];
  filterCommercialLine = '';
  filterProductId = '';

  showConfirmDialog = false;
  promotionToDelete: Promotion | null = null;

  private subs = new Subscription();

  constructor(
    private promotionService: PromotionService,
    private router: Router
  ) {}

  ngOnInit() {
      const nav = history.state;

      if (nav?.successMessage) {
        this.successMessage = nav.successMessage;

        history.replaceState({}, '');

        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      }

      this.loadPromotions();
    }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  loadPromotions() {
    this.isLoading = true;
    this.errorMessage = '';
    const filters: PromotionFilters = {
      date: this.filterDate,
      commercialLine: this.filterCommercialLine || undefined,
      productId: this.filterProductId ? Number(this.filterProductId) : undefined
    };
    this.subs.add(this.promotionService.getPromotions(filters).subscribe({
      next: (promotions) => {
        this.isLoading = false;
        this.promotions = promotions;
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Error al cargar las promociones.';
      }
    }));
  }

  onFiltrar() { this.loadPromotions(); }

  onLimpiar() {
    this.filterDate = new Date().toISOString().split('T')[0];
    this.filterCommercialLine = '';
    this.filterProductId = '';
    this.loadPromotions();
  }

  onAdd() { this.router.navigate(['/home/promotions/create']); }

  onEdit(promotion: Promotion) {
    this.router.navigate(['/home/promotions/edit', promotion.id], { state: { promotion } });
  }

  onDelete(promotion: Promotion) {
    this.promotionToDelete = promotion;
    this.showConfirmDialog = true;
  }

  onConfirmDelete() {
    if (!this.promotionToDelete) return;
    const id = this.promotionToDelete.id;
    this.subs.add(this.promotionService.delete(id).subscribe({
      next: () => {
        this.promotions = this.promotions.filter(p => p.id !== id);
        this.showConfirmDialog = false;
        this.promotionToDelete = null;

        this.successMessage = 'Promoción eliminada correctamente.';

        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        this.showConfirmDialog = false;
        this.errorMessage = error.error?.message || 'Error al eliminar la promoción.';
      }
    }));
  }

  onCancelDelete() {
    this.showConfirmDialog = false;
    this.promotionToDelete = null;
  }
}
