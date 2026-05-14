 import { Component } from '@angular/core';
  import { FormsModule } from '@angular/forms';
  import { CancelButtonComponent } from
  '../../../../shared/components/cancel-button/cancel-button.component';
  import { SubmitButtonComponent } from
  '../../../../shared/components/submit-button/submit-button.component';
  import { FormCardComponent } from
  '../../../../shared/components/form-card/form-card.component';

  @Component({
    selector: 'app-change-password-page',
    imports: [FormsModule, CancelButtonComponent, SubmitButtonComponent,
   FormCardComponent],
    templateUrl: './change-password-page.component.html',
    styleUrl: './change-password-page.component.css'
  })
  export class ChangePasswordPageComponent {
    contrasenaActual = '';
    contrasenaNueva = '';
    confirmarContrasena = '';

    onSubmit() {
      console.log('Cambiando contraseña...');
    }

    onCancelled() {
      console.log('Cancelado');
    }
  }