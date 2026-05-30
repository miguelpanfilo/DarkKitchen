import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { HasRoleDirective } from '../../shared/directives/has-role.directive';

@Component({
  selector: 'app-main-layout',
  imports: [RouterOutlet, RouterLink, HasRoleDirective],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.css'
})
export class MainLayoutComponent implements OnInit {
  email = '';

  constructor(private router: Router, private authService:
    AuthService) { }

  ngOnInit() {
    this.email = this.authService.getEmail() || '';
  }

  onLogout() {
    this.authService.clearSession();
    this.router.navigate(['/welcome']);
  }
}