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
import { ChangePasswordRequest } from '../../models/change-password-request.model';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';
import { PasswordInputComponent } from '../../../../shared/components/password-input/password-input.component';

@Component({
  selector: 'app-change-password-page',
  imports: [CommonModule, FormsModule, CancelButtonComponent, SubmitButtonComponent,
    FormCardComponent, StatusMessagesComponent, PasswordInputComponent],
  templateUrl: './change-password-page.component.html',
  styleUrl: './change-password-page.component.css'
})
export class ChangePasswordPageComponent implements OnInit, OnDestroy {
  userId = 0;
  contrasenaActual = '';
  contrasenaNueva = '';
  confirmarContrasena = '';
  isLoading: boolean = false;
  errorMessage = '';
  successMessage = '';
  submitted = false;
  private subs = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService
  ) { }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  ngOnInit() {
    this.userId = Number(this.route.snapshot.paramMap.get('id'));
  }

  onSubmit() {
    this.submitted = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.contrasenaActual.trim() || !this.contrasenaNueva.trim() || !this.confirmarContrasena.trim()) {
      return;
    }

    if (this.contrasenaNueva !== this.confirmarContrasena) {
      this.errorMessage = 'Las contraseñas nuevas no coinciden.';
      return;
    }

    const request: ChangePasswordRequest = {
      OldPassword: this.contrasenaActual,
      NewPassword: this.contrasenaNueva
    };

    this.isLoading = true;
    this.subs.add(this.userService.changePassword(this.userId, request).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigate(['/home/users'], {
          state: { successMessage: 'Contraseña cambiada exitosamente.' }
        });
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Error al cambiar la contraseña.';
      }
    }));
  }
  onCancelled() {
    this.router.navigate(['/home/users']);
  }
}