export interface ProductPhoto {
  path: string;
}

export interface PhotoRequest {
  fileName: string;
  extension: string;
  sizeInBytes: number;
  base64: string;
}

export interface PatchPhotoRequest {
  path?: string;
  fileName?: string;
  extension?: string;
  sizeInBytes?: number;
  base64?: string;
}
