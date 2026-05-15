import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = 'http://localhost:5254';

  constructor(private http: HttpClient) { }

  login(email: string, contrasena: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/api/v1/sessions`, {
      Email:
        email, Password: contrasena
    });
  }
}