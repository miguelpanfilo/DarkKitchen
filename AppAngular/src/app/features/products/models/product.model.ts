import { ProductPhoto } from './product-photo.model';

export interface Product {
  id: number;
  code: string;
  name: string;
  description: string;
  commercialLine: string;
  category: string;
  price: number;
  isActive: boolean;
  photos: ProductPhoto[];
}
