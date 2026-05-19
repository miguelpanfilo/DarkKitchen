import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { LoginRequest } from '../../features/auth/models/login-request.model';
import { LoginResponse } from '../../features/auth/models/login-response.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = 'http://localhost:5254';

  constructor(private http: HttpClient) { }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<any>(`${this.apiUrl}/api/v1/sessions`, request).pipe(
      map(item => ({
        sessionId: item.sessionId,
        role: item.role
      } as LoginResponse))
    );
  }

  saveSession(sessionId: string, role: string, email: string): void {
    localStorage.setItem('sessionId', sessionId);
    localStorage.setItem('role', role);
    localStorage.setItem('email', email);
  }

  hasSession(): boolean {
    return !!localStorage.getItem('sessionId');
  }

  getSessionId(): string | null {
    return localStorage.getItem('sessionId');
  }

  getRole(): string | null {
    return localStorage.getItem('role');
  }

  getEmail(): string | null {
    return localStorage.getItem('email');
  }

  clearSession(): void {
    localStorage.removeItem('sessionId');
    localStorage.removeItem('role');
    localStorage.removeItem('email');
  }
}