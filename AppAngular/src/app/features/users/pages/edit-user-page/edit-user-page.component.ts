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
import { ROLES } from '../../../../shared/constants/roles';
import { UpdateUserRequest } from '../../models/update-user-request.model';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';

@Component({
  selector: 'app-edit-user-page',
  imports: [CommonModule, FormsModule, CancelButtonComponent, SubmitButtonComponent,
    FormCardComponent, StatusMessagesComponent],
  templateUrl: './edit-user-page.component.html',
  styleUrl: './edit-user-page.component.css'
})

export class EditUserPageComponent implements OnInit {
  userId = 0;
  nombre = '';
  apellido = '';
  email = '';
  telefono = '';
  rol = '';
  isLoading: boolean = false;
  errorMessage = '';
  roles = ROLES;

  constructor(private router: Router, private userService:
    UserService) { }

  ngOnInit() {
    const user =
      this.router.getCurrentNavigation()?.extras.state?.['user']
      ?? history.state?.user;

    if (user) {
      this.userId = user.id;
      this.nombre = user.name;
      this.apellido = user.lastName;
      this.email = user.email;
      this.telefono = user.phone;
      this.rol = user.role;
    }
  }

  onSubmit() {
    const request: UpdateUserRequest = {
      Name: this.nombre,
      LastName: this.apellido,
      Email: this.email,
      Phone: this.telefono,
      Role: this.rol
    };

    this.isLoading = true;
    this.userService.updateUser(this.userId, request).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigate(['/app/users']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Error al modificar el usuario.';
      }
    });
  }

  onCancelled() {
    this.router.navigate(['/app/users']);
  }
}