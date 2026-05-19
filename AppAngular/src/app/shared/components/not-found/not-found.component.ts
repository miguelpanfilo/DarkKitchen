import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-not-found',
  imports: [],
  template: ''
})
export class NotFoundComponent {

  constructor(private router: Router, private authService: AuthService) {
    if (authService.hasSession()) {
      router.navigate(['/app']);
    } else {
      router.navigate(['/welcome']);
    }
  }
}
