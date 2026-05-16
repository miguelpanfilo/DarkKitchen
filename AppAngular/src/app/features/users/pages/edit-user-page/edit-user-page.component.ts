  import { Component } from '@angular/core';
  import { FormsModule } from '@angular/forms';
  import { CancelButtonComponent } from
  '../../../../shared/components/cancel-button/cancel-button.component';
  import { SubmitButtonComponent } from
  '../../../../shared/components/submit-button/submit-button.component';
  import { FormCardComponent } from
  '../../../../shared/components/form-card/form-card.component';

  @Component({
    selector: 'app-edit-user-page',
    imports: [FormsModule, CancelButtonComponent, SubmitButtonComponent,
   FormCardComponent],
    templateUrl: './edit-user-page.component.html',
    styleUrl: './edit-user-page.component.css'
  })
  export class EditUserPageComponent {
    nombre = '';
    apellido = '';
    email = '';
    telefono = '';
    rol = '';

    onSubmit() {
      console.log('Modificando usuario...', this.nombre, this.rol);
    }

    onCancelled() {
      console.log('Cancelado');
    }
  }