import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CancelButtonComponent } from '../../../../shared/components/cancel-button/cancel-button.component';
import { SubmitButtonComponent } from '../../../../shared/components/submit-button/submit-button.component';
import { FormCardComponent } from '../../../../shared/components/form-card/form-card.component';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-register-client-page',
  imports: [FormsModule, CancelButtonComponent, SubmitButtonComponent,
    FormCardComponent],
  templateUrl: './register-client-page.component.html',
  styleUrl: './register-client-page.component.css'
})
export class RegisterClientPageComponent {
  nombre = '';
  apellido = '';
  email = '';
  telefono = '';
  contrasena = '';
  errorMessage = '';
  successMessage = '';

  constructor(private userService: UserService, private router:
    Router) { }

  onSubmit() {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.nombre.trim() || !this.apellido.trim() || !this.email.trim()
      || !this.telefono.trim() || !this.contrasena.trim()) {
      this.errorMessage = 'Todos los campos son obligatorios.';
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

    this.userService.registerClient(this.nombre, this.apellido, this.email, this.telefono, this.contrasena).subscribe({
      next: () => {
        this.router.navigate(['/login'], {
          state: {
            successMessage:
              '¡Registro exitoso! Iniciá sesión para continuar.'
          }
        });
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Ocurrió un error al registrarse.';
      }
    });
  }

  onCancelled() {
    this.router.navigate(['/welcome']);
  }

}