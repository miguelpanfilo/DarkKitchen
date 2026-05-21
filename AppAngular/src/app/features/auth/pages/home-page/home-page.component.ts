import { Component, OnInit } from '@angular/core';

  @Component({
    selector: 'app-home-page',
    imports: [],
    templateUrl: './home-page.component.html',
    styleUrl: './home-page.component.css'
  })
  export class HomePageComponent implements OnInit {
    email = '';

    ngOnInit() {
      this.loadEmail();
    }

    private loadEmail(): void {
      this.email = localStorage.getItem('email') || '';
    }
  }