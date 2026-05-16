 import { Component, OnInit } from '@angular/core';
  import { Router } from '@angular/router';
  import { UserCardComponent } from
  '../../components/user-card/user-card.component';
  import { UserService } from '../../services/user.service';
  import { AddButtonComponent } from
  '../../../../shared/components/add-button/add-button.component';

  @Component({
    selector: 'app-users-list-page',
    imports: [UserCardComponent, AddButtonComponent],
    templateUrl: './users-list-page.component.html',
    styleUrl: './users-list-page.component.css'
  })
  export class UsersListPageComponent implements OnInit {
    users: any[] = [];
    errorMessage = '';

    constructor(private userService: UserService, private router:
  Router) {}

    ngOnInit() {
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
      this.router.navigate(['/register-internal-user']);
    }

    onEdit(user: any) {
      this.router.navigate(['/edit-user'], { state: { user } });
    }

    onDelete(user: any) {
      console.log('Eliminar usuario', user);
    }
  }