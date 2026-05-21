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
import { UserService } from '../../services/user.service';
import { ChangePasswordRequest } from '../../models/change-password-request.model';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';

@Component({
  selector: 'app-change-password-page',
  imports: [CommonModule, FormsModule, CancelButtonComponent, SubmitButtonComponent,
    FormCardComponent, StatusMessagesComponent],
  templateUrl: './change-password-page.component.html',
  styleUrl: './change-password-page.component.css'
})
export class ChangePasswordPageComponent implements OnInit {
  userId = 0;
  userName = '';
  contrasenaActual = '';
  contrasenaNueva = '';
  confirmarContrasena = '';
  isLoading: boolean = false;
  errorMessage = '';
  successMessage = '';
  submitted = false;

  constructor(private router: Router, private userService:
    UserService) { }

  ngOnInit() {
    this.loadUserFromState();
  }

  private loadUserFromState(): void {
    const user =
      this.router.getCurrentNavigation()?.extras.state?.['user']
      ?? history.state?.user;

    if (user) {
      this.userId = user.id;
      this.userName = user.name + ' ' + user.lastName;
    }
  }

  onSubmit() {
    this.submitted = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.contrasenaActual.trim() || !this.contrasenaNueva.trim() || !this.confirmarContrasena.trim()) {
      return;
    }

    if (this.contrasenaNueva !== this.confirmarContrasena) {
      this.errorMessage = 'Las contraseñas nuevas no coinciden.';
      return;
    }

    const request: ChangePasswordRequest = {
      OldPassword: this.contrasenaActual,
      NewPassword: this.contrasenaNueva
    };

    this.isLoading = true;
    this.userService.changePassword(this.userId, request).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigate(['/app/users'], {
          state: { successMessage: 'Contraseña cambiada exitosamente.' }
        });
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Error al cambiar la contraseña.';
      }
    });
  }
  onCancelled() {
    this.router.navigate(['/app/users']);
  }
}