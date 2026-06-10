import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../../../core/services/auth.service';
import { OrderCardComponent } from '../../components/order-card/order-card.component';
import { Order } from '../../models/order.model';
import { OrderFilters } from '../../models/order-filters.model';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';
import { AddButtonComponent } from '../../../../shared/components/add-button/add-button.component';
import { FilterButtonComponent } from '../../../../shared/components/filter-button/filter-button.component';
import { ClearButtonComponent } from '../../../../shared/components/clear-button/clear-button.component';
import { HasRoleDirective } from '../../../../shared/directives/has-role.directive';

@Component({
  selector: 'app-orders-list-page',
  imports: [
    CommonModule, FormsModule, OrderCardComponent, StatusMessagesComponent,
    AddButtonComponent, FilterButtonComponent, ClearButtonComponent, HasRoleDirective
  ],
  templateUrl: './orders-list-page.component.html',
  styleUrl: './orders-list-page.component.css'
})
export class OrdersListPageComponent implements OnInit {
  orders: Order[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  filterDateFrom = '';
  filterDateTo = '';
  filterStatus = '';
  filterStreet = '';

  statuses = ['Pending', 'Prepared', 'OnTheWay', 'Delivered', 'NotDelivered', 'Cancelled', 'Delayed'];

  constructor(
    private orderService: OrderService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit() {
    const nav = history.state;

    if (nav?.successMessage) {
      this.successMessage = nav.successMessage;

      history.replaceState({}, '');

      setTimeout(() => {
        this.successMessage = '';
      }, 3000);
    }

    if (this.authService.getRole() !== 'Client') {
      this.setDefaultDateRange();
    }

    this.loadOrders();
  }

  private setDefaultDateRange() {
    const pad = (n: number) => n.toString().padStart(2, '0');
    const toDateStr = (d: Date) =>
      `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
    const now = new Date();
    const monthAgo = new Date(now);
    monthAgo.setDate(monthAgo.getDate() - 30);
    this.filterDateTo = toDateStr(now);
    this.filterDateFrom = toDateStr(monthAgo);
  }

  loadOrders() {
    this.isLoading = true;
    this.errorMessage = '';
    const filters: OrderFilters = {
      dateFrom: this.filterDateFrom || undefined,
      dateTo: this.filterDateTo || undefined,
      status: this.filterStatus || undefined,
      street: this.filterStreet || undefined
    };
    this.orderService.getOrders(filters).subscribe({
      next: (orders) => {
        this.isLoading = false;
        this.orders = orders;
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Error al cargar las órdenes.';
      }
    });
  }

  onFiltrar() {
    const role = this.authService.getRole();
    if (role !== 'Client' && (!this.filterDateFrom || !this.filterDateTo)) {
      this.errorMessage = 'El rango de fechas es obligatorio.';
      return;
    }
    if (this.filterDateFrom && this.filterDateTo && this.filterDateFrom > this.filterDateTo) {
      this.errorMessage = 'La fecha de inicio no puede ser posterior a la fecha fin.';
      return;
    }
    this.loadOrders();
  }

  onLimpiar() {
    this.filterStatus = '';
    this.filterStreet = '';
    if (this.authService.getRole() !== 'Client') {
      this.setDefaultDateRange();
    } else {
      this.filterDateFrom = '';
      this.filterDateTo = '';
    }
    this.loadOrders();
  }

  onAdd() {
    this.router.navigate(['/home/orders/create']);
  }

  onViewDetail(order: Order) {
    this.router.navigate(['/home/orders', order.id]);
  }

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      Pending: 'Pendiente', Prepared: 'Preparado', OnTheWay: 'En camino',
      Delivered: 'Entregado', NotDelivered: 'No entregado',
      Cancelled: 'Cancelado', Delayed: 'Demorado'
    };
    return labels[status] ?? status;
  }
}
