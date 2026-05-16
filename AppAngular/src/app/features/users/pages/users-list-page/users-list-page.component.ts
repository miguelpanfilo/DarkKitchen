import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserCardComponent } from
  '../../components/user-card/user-card.component';
import { UserService } from '../../services/user.service';
import { AddButtonComponent } from
  '../../../../shared/components/add-button/add-button.component';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-users-list-page',
  imports: [UserCardComponent, AddButtonComponent,
    ConfirmDialogComponent],
  templateUrl: './users-list-page.component.html',
  styleUrl: './users-list-page.component.css'
})
export class UsersListPageComponent implements OnInit {
  users: any[] = [];
  errorMessage = '';
  showConfirmDialog = false;
  userToDelete: any = null;

  constructor(private userService: UserService, private router:
    Router) { }

  successMessage = '';
  ngOnInit() {
    const nav = this.router.getCurrentNavigation()?.extras.state
      ?? history.state;
    if (nav?.successMessage) {
      this.successMessage = nav.successMessage;
    }
    this.userService.getUsers().subscribe({
      next: (response) => {
        this.users = response;
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Error al cargar los usuarios.';
      }
    });
  }

  onAdd() {
    this.router.navigate(['/app/register-internal-user']);
  }

  onEdit(user: any) {
    this.router.navigate(['/app/edit-user'], { state: { user } });
  }

  onDelete(user: any) {
    this.userToDelete = user;
    this.showConfirmDialog = true;
  }

  onConfirmDelete() {
    this.userService.deleteUser(this.userToDelete.id).subscribe({
      next: () => {
        this.users = this.users.filter(u => u.id !==
          this.userToDelete.id);
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
  onChangePassword(user: any) {
    this.router.navigate(['/app/change-password'], { state: { user } });
  }
}