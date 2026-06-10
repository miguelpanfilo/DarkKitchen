import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
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
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';
import { PasswordInputComponent } from '../../../../shared/components/password-input/password-input.component';

@Component({
  selector: 'app-login-page',
  imports: [CommonModule, FormsModule, CancelButtonComponent, SubmitButtonComponent,
    FormCardComponent, StatusMessagesComponent, PasswordInputComponent],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.css'
})
export class LoginPageComponent implements OnInit, OnDestroy {
  email = '';
  contrasena = '';
  isLoading: boolean = false;
  errorMessage = '';
  successMessage = '';

  constructor(private authService: AuthService, private router: Router) { }

  private subs = new Subscription();

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  ngOnInit() {
    const nav = this.router.getCurrentNavigation()?.extras.state ??
      history.state;
    if (nav?.successMessage) {
      this.successMessage = nav.successMessage;
    }
  }

  onSubmit() {
    this.errorMessage = '';
    this.isLoading = true;

    const request: LoginRequest = {
      Email: this.email,
      Password: this.contrasena
    };

    this.subs.add(this.authService.login(request).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.authService.saveSession(response.sessionId, response.role, this.email);
        const redirectUrl = localStorage.getItem('redirectUrl');
        if (redirectUrl) {
          localStorage.removeItem('redirectUrl');
          this.router.navigate([redirectUrl]);
        } else {
          this.router.navigate(['/home']);
        }
      },
      error: (error) => {
        this.isLoading = false;
        if (error.status === 401) {
          this.errorMessage = 'Email o contraseña incorrectos.';
        } else {
          this.errorMessage = 'Ocurrió un error. Intentá de nuevo.';
        }
      }
    }));
  }

  onCancelled() {
    this.router.navigate(['/welcome']);
  }
}