import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';

@Component({
  selector: 'app-home-page',
  imports: [CommonModule, StatusMessagesComponent],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.css'
})
export class HomePageComponent implements OnInit {
  email = '';
  errorMessage = '';

  constructor(private router: Router) {}

  ngOnInit() {
    this.loadEmail();
    this.loadNavigationState();
  }

  private loadEmail(): void {
    this.email = localStorage.getItem('email') || '';
  }

  private loadNavigationState(): void {
    const nav = this.router.getCurrentNavigation()?.extras.state ?? history.state;
    if (nav?.errorMessage) this.errorMessage = nav.errorMessage;
  }
}
