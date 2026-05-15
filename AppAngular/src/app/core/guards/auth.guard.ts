 import { inject } from '@angular/core';
  import { CanActivateFn, Router } from '@angular/router';

  export const authGuard: CanActivateFn = () => {
    const router = inject(Router);
    const sessionId = localStorage.getItem('sessionId');

    if (sessionId) {
      return true;
    }

    router.navigate(['/login']);
    return false;
  };