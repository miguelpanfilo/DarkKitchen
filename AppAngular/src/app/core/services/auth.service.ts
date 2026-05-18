import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = 'http://localhost:5254';

  constructor(private http: HttpClient) { }

  login(email: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/api/v1/sessions`, {
      Email: email,
      Password: password
    });
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