export interface CreateOrderItemRequest {
  productId: number;
  quantity: number;
}

export interface CreateAddressRequest {
  street: string;
  doorNumber: string;
  apartment?: string;
}

export interface CreateOrderRequest {
  deliveryType: string;
  address: CreateAddressRequest;
  items: CreateOrderItemRequest[];
}
