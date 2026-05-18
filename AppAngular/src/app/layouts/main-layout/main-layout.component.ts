import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-main-layout',
  imports: [RouterOutlet, RouterLink],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.css'
})
export class MainLayoutComponent implements OnInit {
  email = '';
  role = '';

  constructor(private router: Router, private authService:
    AuthService) { }

  ngOnInit() {
    this.email = this.authService.getEmail() || '';
    this.role = this.authService.getRole() || '';
  }

  get isAdmin(): boolean {
    return this.role === 'Administrative';
  }

  get isAdminOrClient(): boolean {
    return this.role === 'Administrative' || this.role === 'Client';
  }

  onLogout() {
    this.authService.clearSession();
    this.router.navigate(['/welcome']);
  }
} 