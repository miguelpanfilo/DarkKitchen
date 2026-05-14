import { Component, OnInit } from '@angular/core';
  import { Router, RouterLink, RouterOutlet } from '@angular/router';

  @Component({
    selector: 'app-main-layout',
    imports: [RouterOutlet, RouterLink],
    templateUrl: './main-layout.component.html',
    styleUrl: './main-layout.component.css'
  })
  export class MainLayoutComponent implements OnInit {
    email = '';

    constructor(private router: Router) {}

    ngOnInit() {
      this.email = localStorage.getItem('email') || '';
    }

    onLogout() {
      localStorage.clear();
      this.router.navigate(['/login']);
    }
  }