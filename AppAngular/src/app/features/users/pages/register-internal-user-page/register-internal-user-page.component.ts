import { Component, OnDestroy } from '@angular/core';
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
import { UserService } from '../../services/user.service';
import { ROLES } from '../../../../shared/constants/roles';
import { RegisterUserRequest } from '../../models/register-user-request.model';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';
import { HasUnsavedChanges } from '../../../../core/guards/pending-changes.guard';
import { PasswordInputComponent } from '../../../../shared/components/password-input/password-input.component';

@Component({
  selector: 'app-register-internal-user-page',
  imports: [CommonModule, FormsModule, CancelButtonComponent, SubmitButtonComponent,
    FormCardComponent, StatusMessagesComponent, PasswordInputComponent],
  templateUrl: './register-internal-user-page.component.html',
  styleUrl: './register-internal-user-page.component.css'
})
export class RegisterInternalUserPageComponent implements OnDestroy, HasUnsavedChanges {
  nombre = '';
  apellido = '';
  email = '';
  telefono = '';
  contrasena = '';
  rol = '';
  isLoading: boolean = false;
  errorMessage = '';
  successMessage = '';
  submitted = false;
  roles = ROLES.filter(r => r.value !== 'Client');
  private saveSucceeded = false;

  constructor(private userService: UserService, private router:
    Router) { }

  private subs = new Subscription();

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  hasUnsavedChanges(): boolean {
    return !this.saveSucceeded && !!(this.nombre || this.apellido || this.email || this.telefono || this.contrasena || this.rol);
  }

  onSubmit() {
    this.submitted = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.nombre.trim() || !this.apellido.trim() ||
      !this.email.trim() || !this.telefono.trim() || !this.contrasena.trim()
      || !this.rol) {
      return;
    }

    const request: RegisterUserRequest = {
      Name: this.nombre,
      LastName: this.apellido,
      Email: this.email,
      Phone: this.telefono,
      Password: this.contrasena,
      Role: this.rol
    };

    this.isLoading = true;
    this.subs.add(this.userService.registerInternalUser(request).subscribe({
      next: () => {
        this.isLoading = false;
        this.saveSucceeded = true;
        this.router.navigate(['/home/users'], { state: { successMessage: '¡Usuario registrado exitosamente!' } });
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Ocurrió un error al registrar el usuario.';
      }
    }));
  }

  onCancelled() {
    this.router.navigate(['/home/users']);
  }
}