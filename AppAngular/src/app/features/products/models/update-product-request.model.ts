import { PatchPhotoRequest } from './product-photo.model';

export interface UpdateProductRequest {
  code?: string;
  name?: string;
  description?: string;
  commercialLine?: string;
  category?: string;
  price?: number;
  isActive?: boolean;
  photos?: PatchPhotoRequest[];
}
