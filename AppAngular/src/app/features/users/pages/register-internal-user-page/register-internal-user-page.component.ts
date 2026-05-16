import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CancelButtonComponent } from
  '../../../../shared/components/cancel-button/cancel-button.component';
import { SubmitButtonComponent } from
  '../../../../shared/components/submit-button/submit-button.component';
import { FormCardComponent } from
  '../../../../shared/components/form-card/form-card.component';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-register-internal-user-page',
  imports: [FormsModule, CancelButtonComponent, SubmitButtonComponent,
    FormCardComponent],
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
  errorMessage = '';
  successMessage = '';

  constructor(private userService: UserService, private router:
    Router) { }

  roles = [
    { value: 'Administrative', label: 'Administrador' },
    { value: 'Preparer', label: 'Preparador' },
    { value: 'Client', label: 'Cliente' }
  ];
    


  onSubmit() {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.nombre.trim() || !this.apellido.trim() ||
      !this.email.trim() || !this.telefono.trim() || !this.contrasena.trim()
      || !this.rol) {
      this.errorMessage = 'Todos los campos son obligatorios.';
      return;
    }

    this.userService.registerInternalUser(this.nombre, this.apellido,
      this.email, this.telefono, this.contrasena, this.rol).subscribe({
        next: () => {
          this.successMessage = '¡Usuario registrado exitosamente!';
          this.nombre = '';
          this.apellido = '';
          this.email = '';
          this.telefono = '';
          this.contrasena = '';
          this.rol = '';
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Ocurrió un error al registrar el usuario.';
        }
      });
  }

  onCancelled() {
    this.router.navigate(['/']);
  }
}