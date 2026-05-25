import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Order } from '../../models/order.model';

@Component({
  selector: 'app-order-card',
  imports: [CommonModule],
  templateUrl: './order-card.component.html',
  styleUrl: './order-card.component.css'
})
export class OrderCardComponent {
  @Input() order!: Order;
  @Output() viewDetail = new EventEmitter<Order>();

  onView() {
    this.viewDetail.emit(this.order);
  }

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      Pending: 'Pendiente',
      Prepared: 'Preparado',
      OnTheWay: 'En camino',
      Delivered: 'Entregado',
      NotDelivered: 'No entregado',
      Cancelled: 'Cancelado',
      Delayed: 'Demorado'
    };
    return labels[status] ?? status;
  }
}
