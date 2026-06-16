import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable, Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../../../core/services/auth.service';
import { Order } from '../../models/order.model';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-order-detail-page',
  imports: [CommonModule, StatusMessagesComponent, ConfirmDialogComponent],
  templateUrl: './order-detail-page.component.html',
  styleUrl: './order-detail-page.component.css'
})
export class OrderDetailPageComponent implements OnInit, OnDestroy {
  order: Order | null = null;
  isLoading = false;
  errorMessage = '';
  actionLoading = false;

  showConfirmDialog = false;
  confirmMessage = '';
  pendingAction: (() => void) | null = null;

  private subs = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private orderService: OrderService,
    private authService: AuthService
  ) { }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadOrder(id);
  }

  loadOrder(id: number) {
    this.isLoading = true;
    this.errorMessage = '';
    this.subs.add(this.orderService.getById(id).subscribe({
      next: (order) => {
        this.isLoading = false;
        this.order = order;
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Error al cargar la orden.';
      }
    }));
  }

  get role(): string {
    return this.authService.getRole() || '';
  }

  get isPreparer(): boolean {
    return this.role === 'Preparer';
  }

  get isAdmin(): boolean {
    return this.role === 'Administrative';
  }

  get isClient(): boolean {
    return this.role === 'Client';
  }

  get isPreparerOrAdmin(): boolean {
    return this.isPreparer || this.isAdmin;
  }

  canPrepare(): boolean {
    return this.isPreparerOrAdmin &&
      (this.order?.status === 'Pending' || this.order?.status === 'Delayed');
  }

  canDelay(): boolean {
    return this.isPreparerOrAdmin && this.order?.status === 'Pending';
  }

  canCancel(): boolean {
    return (this.isClient || this.isAdmin) &&
      (this.order?.status === 'Pending' || this.order?.status === 'Delayed');
  }

  canDispatch(): boolean {
    return this.isPreparerOrAdmin && this.order?.status === 'Prepared';
  }

  canDeliver(): boolean {
    return this.isPreparerOrAdmin && this.order?.status === 'OnTheWay';
  }

  canMarkNotDelivered(): boolean {
    return this.isPreparerOrAdmin && this.order?.status === 'OnTheWay';
  }

  requestAction(message: string, action: () => void) {
    this.confirmMessage = message;
    this.pendingAction = action;
    this.showConfirmDialog = true;
  }

  onPrepare() {
    this.requestAction('¿Confirmar que la orden está preparada?', () => this.executeTransition(
      this.orderService.prepare(this.order!.id), 'Prepared'
    ));
  }

  onDelay() {
    this.requestAction('¿Marcar esta orden como demorada?', () => this.executeTransition(
      this.orderService.delay(this.order!.id), 'Delayed'
    ));
  }

  onCancel() {
    this.requestAction('¿Estás seguro que querés cancelar esta orden?', () => this.executeTransition(
      this.orderService.cancel(this.order!.id), 'Cancelled'
    ));
  }

  onDispatch() {
    this.requestAction('¿Confirmar despacho de la orden?', () => this.executeTransition(
      this.orderService.dispatch(this.order!.id), 'OnTheWay'
    ));
  }

  onDeliver() {
    this.requestAction('¿Confirmar entrega de la orden?', () => this.executeTransition(
      this.orderService.deliver(this.order!.id), 'Delivered'
    ));
  }

  onMarkNotDelivered() {
    this.requestAction('¿Marcar la orden como no entregada?', () => this.executeTransition(
      this.orderService.markNotDelivered(this.order!.id), 'NotDelivered'
    ));
  }

  private executeTransition(call: Observable<void>, newStatus: string) {
    this.actionLoading = true;
    this.errorMessage = '';
    this.subs.add(call.subscribe({
      next: () => {
        this.actionLoading = false;
        this.order!.status = newStatus;
      },
      error: (error: Error) => {
        this.actionLoading = false;
        this.errorMessage = (error as any).error?.message || 'Error al actualizar la orden.';
      }
    }));
  }

  onConfirmDialog() {
    this.showConfirmDialog = false;
    if (this.pendingAction) {
      this.pendingAction();
      this.pendingAction = null;
    }
  }

  onCancelDialog() {
    this.showConfirmDialog = false;
    this.pendingAction = null;
  }

  onBack() {
    this.router.navigate(['/home/orders']);
  }

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      Pending: 'Pendiente', Prepared: 'Preparado', OnTheWay: 'En camino',
      Delivered: 'Entregado', NotDelivered: 'No entregado',
      Cancelled: 'Cancelado', Delayed: 'Demorado'
    };
    return labels[status] ?? status;
  }

  getDeliveryLabel(type: string): string {
    return type;
  }
}
