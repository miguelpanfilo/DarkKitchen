import { OrderItem } from './order-item.model';

export interface OrderClient {
  id: number;
  name: string;
  lastName: string;
  email: string;
}

export interface OrderAddress {
  street: string;
  doorNumber: string;
  apartment?: string;
}

export interface Order {
  id: number;
  orderNumber: string;
  client: OrderClient;
  deliveryType: string;
  address: OrderAddress;
  items: OrderItem[];
  itemsCount: number;
  status: string;
  createdAt: string;
  statusUpdatedAt: string;
  subtotal: number;
  shippingCost: number;
  iva: number;
  ivaPercentage: number;
  total: number;
}
