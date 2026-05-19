import { Component } from '@angular/core';
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

@Component({
  selector: 'app-register-internal-user-page',
  imports: [CommonModule, FormsModule, CancelButtonComponent, SubmitButtonComponent,
    FormCardComponent, StatusMessagesComponent],
  templateUrl: './register-internal-user-page.component.html',
  styleUrl: './register-internal-user-page.component.css'
})
export class RegisterInternalUserPageComponent {
  nombre = '';
  apellido = '';
  email = '';
  telefono = '';
  contrasena = '';
  rol = '';
  isLoading: boolean = false;
  errorMessage = '';
  successMessage = '';
  roles = ROLES.filter(r => r.value !== 'Client');

  constructor(private userService: UserService, private router:
    Router) { }

  onSubmit() {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.nombre.trim() || !this.apellido.trim() ||
      !this.email.trim() || !this.telefono.trim() || !this.contrasena.trim()
      || !this.rol) {
      this.errorMessage = 'Todos los campos son obligatorios.';
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
    this.userService.registerInternalUser(request).subscribe({
      next: () => {
        this.isLoading = false;
        this.successMessage = '¡Usuario registrado exitosamente!';
        this.nombre = '';
        this.apellido = '';
        this.email = '';
        this.telefono = '';
        this.contrasena = '';
        this.rol = '';
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Ocurrió un error al registrar el usuario.';
      }
    });
  }

  onCancelled() {
    this.router.navigate(['/app']);
  }
}