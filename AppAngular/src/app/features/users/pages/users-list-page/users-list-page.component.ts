import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UserCardComponent } from
  '../../components/user-card/user-card.component';
import { UserService } from '../../services/user.service';
import { AddButtonComponent } from
  '../../../../shared/components/add-button/add-button.component';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';
import { FilterButtonComponent } from '../../../../shared/components/filter-button/filter-button.component';
import { ClearButtonComponent } from '../../../../shared/components/clear-button/clear-button.component';
import { User } from '../../models/user.model';
import { FindUserRequest } from '../../models/find-user-request.model';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';

@Component({
  selector: 'app-users-list-page',
  imports: [CommonModule, FormsModule, UserCardComponent, AddButtonComponent,
    ConfirmDialogComponent, FilterButtonComponent, ClearButtonComponent, StatusMessagesComponent],
  templateUrl: './users-list-page.component.html',
  styleUrl: './users-list-page.component.css'
})
export class UsersListPageComponent implements OnInit {
  users: User[] = [];
  isLoading: boolean = false;
  errorMessage = '';
  successMessage = '';
  showConfirmDialog = false;
  userToDelete: User | null = null;
  filterName = '';
  filterLastName = '';

  constructor(private userService: UserService, private router: Router) { }

  ngOnInit() {
    const nav = this.router.getCurrentNavigation()?.extras.state ?? history.state;
    if (nav?.successMessage) {
      this.successMessage = nav.successMessage;
    }
    this.loadUsers();
  }

  loadUsers() {
    this.isLoading = true;
    this.errorMessage = '';
    const filters: FindUserRequest = {
      name: this.filterName || undefined,
      lastName: this.filterLastName || undefined
    };

    this.userService.getUsers(filters).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.users = response;
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Error al cargar los  usuarios.';
      }
    });
  }

  onAdd() {
    this.router.navigate(['/app/register-internal-user']);
  }

  onFiltrar() {
    this.loadUsers();
  }

  onLimpiar() {
    this.filterName = '';
    this.filterLastName = '';
    this.loadUsers();
  }

  onEdit(user: User) {
    this.router.navigate(['/app/edit-user'], { state: { user } });
  }

  onDelete(user: User) {
    this.userToDelete = user;
    this.showConfirmDialog = true;
  }

  onConfirmDelete() {
    if (!this.userToDelete) return;

    const id = this.userToDelete.id;
    this.userService.deleteUser(id).subscribe({
      next: () => {
        this.users = this.users.filter(u => u.id !== id);
        this.showConfirmDialog = false;
        this.userToDelete = null;
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Error al eliminar el usuario.';
        this.showConfirmDialog = false;
      }
    });
  }

  onCancelDelete() {
    this.showConfirmDialog = false;
    this.userToDelete = null;
  }
  onChangePassword(user: User) {
    this.router.navigate(['/app/change-password'], { state: { user } });
  }
}