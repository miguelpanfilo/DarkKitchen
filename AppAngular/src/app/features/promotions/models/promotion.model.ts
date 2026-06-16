export interface PromotionProduct {
  id: number;
  name: string;
}

export interface Promotion {
  id: number;
  name: string;
  discountPercentage: number;
  startDate: string;
  endDate: string;
  products: PromotionProduct[];
}
