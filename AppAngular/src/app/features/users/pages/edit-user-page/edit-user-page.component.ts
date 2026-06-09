import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
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
import { HasUnsavedChanges } from '../../../../core/guards/pending-changes.guard';

@Component({
  selector: 'app-edit-user-page',
  imports: [CommonModule, FormsModule, CancelButtonComponent, SubmitButtonComponent,
    FormCardComponent, StatusMessagesComponent],
  templateUrl: './edit-user-page.component.html',
  styleUrl: './edit-user-page.component.css'
})

export class EditUserPageComponent implements OnInit, OnDestroy, HasUnsavedChanges {
  userId = 0;
  nombre = '';
  apellido = '';
  email = '';
  telefono = '';
  rol = '';
  isLoading: boolean = false;
  errorMessage = '';
  submitted = false;
  isDirty = false;
  roles = ROLES;
  private subs = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService
  ) { }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  markDirty(): void { this.isDirty = true; }

  hasUnsavedChanges(): boolean { return this.isDirty; }

  ngOnInit() {
    this.userId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadUser();
  }

  loadUser() {
    this.isLoading = true;
    this.errorMessage = '';
    this.subs.add(this.userService.getUserById(this.userId).subscribe({
      next: (user) => {
        this.isLoading = false;
        this.nombre = user.name;
        this.apellido = user.lastName;
        this.email = user.email;
        this.telefono = user.phone;
        this.rol = user.role;
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Error al cargar el usuario.';
      }
    }));
  }

  onSubmit() {
    this.submitted = true;
    if (!this.nombre.trim() || !this.apellido.trim() || !this.email.trim() || !this.telefono.trim() || !this.rol) {
      return;
    }
    const request: UpdateUserRequest = {
      Name: this.nombre,
      LastName: this.apellido,
      Email: this.email,
      Phone: this.telefono,
      Role: this.rol
    };

    this.isLoading = true;
    this.subs.add(this.userService.updateUser(this.userId, request).subscribe({
      next: () => {
        this.isLoading = false;
        this.isDirty = false;
        this.router.navigate(['/home/users']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Error al modificar el usuario.';
      }
    }));
  }

  onCancelled() {
    this.router.navigate(['/home/users']);
  }
}