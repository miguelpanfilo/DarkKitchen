export interface ProductPhoto {
  path: string;
}

export interface PhotoRequest {
  fileName: string;
  extension: string;
  sizeInBytes: number;
  base64: string;
}
