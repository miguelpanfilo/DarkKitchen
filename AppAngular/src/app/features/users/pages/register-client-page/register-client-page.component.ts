import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CancelButtonComponent } from '../../../../shared/components/cancel-button/cancel-button.component';
import { SubmitButtonComponent } from '../../../../shared/components/submit-button/submit-button.component';
import { FormCardComponent } from '../../../../shared/components/form-card/form-card.component';
import { UserService } from '../../services/user.service';
import { RegisterClientRequest } from '../../models/register-client-request.model';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';
import { HasUnsavedChanges } from '../../../../core/guards/pending-changes.guard';
import { PasswordInputComponent } from '../../../../shared/components/password-input/password-input.component';

@Component({
  selector: 'app-register-client-page',
  imports: [CommonModule, FormsModule, CancelButtonComponent, SubmitButtonComponent,
    FormCardComponent, StatusMessagesComponent, PasswordInputComponent],
  templateUrl: './register-client-page.component.html',
  styleUrl: './register-client-page.component.css'
})
export class RegisterClientPageComponent implements OnDestroy, HasUnsavedChanges {
  nombre = '';
  apellido = '';
  email = '';
  telefono = '';
  contrasena = '';
  errorMessage = '';
  successMessage = '';
  submitted = false;
  private saveSucceeded = false;

  constructor(private userService: UserService, private router: Router) { }

  isLoading: boolean = false;
  private subs = new Subscription();

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  hasUnsavedChanges(): boolean {
    return !this.saveSucceeded && !!(this.nombre || this.apellido || this.email || this.telefono || this.contrasena);
  }

  onSubmit() {
    this.submitted = true;
    this.errorMessage = '';
    this.successMessage = '';
    if (!this.nombre.trim() || !this.apellido.trim() ||
      !this.email.trim() || !this.telefono.trim() || !this.contrasena.trim()) {
      return;
    }

    if (this.apellido.trim().length < 3 || this.apellido.trim().length >
      25) {
      this.errorMessage = 'El apellido debe tener entre 3 y 25 caracteres.';
      return;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(this.email)) {
      this.errorMessage = 'El email no tiene un formato válido.';
      return;
    }

    const phoneRegex = /^09[1-9][0-9]{6}$/;
    if (!phoneRegex.test(this.telefono)) {
      this.errorMessage = 'El teléfono no es válido.';
      return;
    }

    const passwordRegex =
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$/;
    if (!passwordRegex.test(this.contrasena)) {
      this.errorMessage = 'Contraseña con formato invalido.';
      return;
    }

    const request: RegisterClientRequest = {
      Name: this.nombre,
      LastName: this.apellido,
      Email: this.email,
      Phone: this.telefono,
      Password: this.contrasena
    };

    this.isLoading = true;
    this.subs.add(this.userService.registerClient(request).subscribe({
      next: () => {
        this.isLoading = false;
        this.saveSucceeded = true;
        this.router.navigate(['/login'], {
          state: { successMessage: '¡Registro exitoso! Iniciá sesión para continuar.' }
        });
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Ocurrió un error al registrarse.';
      }
    }));
  }

  onCancelled() {
    this.router.navigate(['/welcome']);
  }

}