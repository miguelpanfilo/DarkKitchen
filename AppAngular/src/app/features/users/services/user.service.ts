import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl = 'http://localhost:5254';

  constructor(private http: HttpClient) { }

  registerClient(name: string, lastName: string, email: string, phone:
    string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/api/v1/clients`, {
      Name: name,
      LastName: lastName,
      Email: email,
      Phone: phone,
      Password: password
    });
  }

  registerInternalUser(name: string, lastName: string, email: string,
    phone: string, password: string, role: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/api/v1/users`, {
      Name: name,
      LastName: lastName,
      Email: email,
      Phone: phone,
      Password: password,
      Role: role
    }, {
      headers: {
        'X-Session-Id': localStorage.getItem('sessionId') || ''
      }
    });
  }

  getUsers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/api/v1/users`, {
      headers: {
        'X-Session-Id': localStorage.getItem('sessionId') || ''
      }
    });
  }

  updateUser(id: number, data: {
    Name?: string; LastName?: string;
    Email?: string; Phone?: string; Role?: string
  }): Observable<any> {
    return this.http.patch(`${this.apiUrl}/api/v1/users/${id}`, data, {
      headers: {
        'X-Session-Id': localStorage.getItem('sessionId') || ''
      }
    });
  }
  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/v1/users/${id}`, {
      headers: {'X-Session-Id':localStorage.getItem('sessionId') || ''
      }
    });
  }

  changePassword(id: number, oldPassword: string, newPassword: string):
  Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/api/v1/users/${id}/password`, {
      OldPassword: oldPassword,
      NewPassword: newPassword
    }, {
      headers: { 'X-Session-Id': localStorage.getItem('sessionId') || ''
   }
    });
  }
}