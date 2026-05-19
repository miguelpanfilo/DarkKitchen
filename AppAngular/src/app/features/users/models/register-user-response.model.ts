export interface RegisterUserResponse {
  id: number;
  name: string;
  lastName: string;
  email: string;
  role: string;
  phone?: string;
}
