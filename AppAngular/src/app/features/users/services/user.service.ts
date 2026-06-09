import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { User } from '../models/user.model';
import { RegisterClientRequest } from '../models/register-client-request.model';
import { RegisterClientResponse } from '../models/register-client-response.model';
import { RegisterUserRequest } from '../models/register-user-request.model';
import { RegisterUserResponse } from '../models/register-user-response.model';
import { FindUserRequest } from '../models/find-user-request.model';
import { UpdateUserRequest } from '../models/update-user-request.model';
import { UpdateUserResponse } from '../models/update-user-response.model';
import { ChangePasswordRequest } from '../models/change-password-request.model';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl = environment.webApiBaseUrl;

  constructor(private http: HttpClient) { }

  registerClient(request: RegisterClientRequest): Observable<RegisterClientResponse> {
    return this.http.post<any>(`${this.apiUrl}/api/v1/clients`, request).pipe(
      map(item => ({
        name: item.name,
        lastName: item.lastName,
        email: item.email
      } as RegisterClientResponse))
    );
  }

  registerInternalUser(request: RegisterUserRequest): Observable<RegisterUserResponse> {
    return this.http.post<any>(`${this.apiUrl}/api/v1/users`, request).pipe(
      map(item => ({
        id: item.id,
        name: item.name,
        lastName: item.lastName,
        email: item.email,
        role: item.role,
        phone: item.phone
      } as RegisterUserResponse))
    );
  }

  getUserById(id: number): Observable<User> {
    return this.http.get<any>(`${this.apiUrl}/api/v1/users/${id}`).pipe(
      map(item => ({
        id: item.id,
        name: item.name,
        lastName: item.lastName,
        email: item.email,
        role: item.role,
        phone: item.phone
      } as User))
    );
  }

  getUsers(filters?: FindUserRequest): Observable<User[]> {
    let params = new HttpParams();
    if (filters?.name) params = params.set('Name', filters.name);
    if (filters?.lastName) params = params.set('LastName', filters.lastName);
    return this.http.get<any[]>(`${this.apiUrl}/api/v1/users`, { params }).pipe(
      map(response => response.map(item => ({
        id: item.id,
        name: item.name,
        lastName: item.lastName,
        email: item.email,
        role: item.role,
        phone: item.phone
      } as User)))
    );
  }

  updateUser(id: number, request: UpdateUserRequest): Observable<UpdateUserResponse> {
    return this.http.patch<any>(`${this.apiUrl}/api/v1/users/${id}`, request).pipe(
      map(item => ({
        id: item.id,
        name: item.name,
        lastName: item.lastName,
        email: item.email,
        role: item.role,
        phone: item.phone
      } as UpdateUserResponse))
    );
  }

  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/v1/users/${id}`);
  }

  changePassword(id: number, request: ChangePasswordRequest): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/api/v1/users/${id}/password`, request);
  }
}
