import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CancelButtonComponent } from
  '../../../../shared/components/cancel-button/cancel-button.component';
import { SubmitButtonComponent } from
  '../../../../shared/components/submit-button/submit-button.component';
import { FormCardComponent } from
  '../../../../shared/components/form-card/form-card.component';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login-page',
  imports: [FormsModule, CancelButtonComponent, SubmitButtonComponent,
    FormCardComponent],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.css'
})
export class LoginPageComponent {
  email = '';
  contrasena = '';
  errorMessage = '';

  constructor(private authService: AuthService, private router:
    Router) { }

  onSubmit() {
    this.errorMessage = '';
    this.authService.login(this.email, this.contrasena).subscribe({
      next: (response) => {
        localStorage.setItem('sessionId', response.sessionId);
        localStorage.setItem('role', response.role);
        localStorage.setItem('email', this.email);
        this.router.navigate(['/']);
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
    this.email = '';
    this.contrasena = '';
    this.errorMessage = '';
  }
}