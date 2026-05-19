import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CancelButtonComponent } from
  '../../../../shared/components/cancel-button/cancel-button.component';
import { SubmitButtonComponent } from
  '../../../../shared/components/submit-button/submit-button.component';
import { FormCardComponent } from
  '../../../../shared/components/form-card/form-card.component';
import { AuthService } from '../../../../core/services/auth.service';
import { LoginRequest } from '../../models/login-request.model';

@Component({
  selector: 'app-login-page',
  imports: [CommonModule, FormsModule, CancelButtonComponent, SubmitButtonComponent,
    FormCardComponent],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.css'
})
export class LoginPageComponent implements OnInit {
  email = '';
  contrasena = '';
  errorMessage = '';
  successMessage = '';

  constructor(private authService: AuthService, private router:
    Router) { }

  ngOnInit() {
    const nav = this.router.getCurrentNavigation()?.extras.state ??
      history.state;
    if (nav?.successMessage) {
      this.successMessage = nav.successMessage;
    }
  }

  onSubmit() {
    this.errorMessage = '';

    const request: LoginRequest = {
      Email: this.email,
      Password: this.contrasena
    };

    this.authService.login(request).subscribe({
      next: (response) => {
        this.authService.saveSession(response.sessionId, response.role, this.email);
        this.router.navigate(['/app']);
      },
      error: (error) => {
        if (error.status === 401) {
          this.errorMessage = 'Email o contraseña incorrectos.';
        } else {
          this.errorMessage = 'Ocurrió un error. Intentá de nuevo.';
        }
      }
    });
  }

  onCancelled() {
    this.router.navigate(['/welcome']);
  }
}