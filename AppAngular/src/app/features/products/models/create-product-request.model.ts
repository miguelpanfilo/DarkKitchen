import { PhotoRequest } from './product-photo.model';

export interface CreateProductRequest {
  code: string;
  name: string;
  description: string;
  commercialLine: string;
  category: string;
  price: number;
  photos: PhotoRequest[];
}
