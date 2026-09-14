import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthService } from '../services/auth.service';

export const adminGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    return router.createUrlTree(['/login']);
  }

  const token = authService.getToken();

  if (!token) {
    return router.createUrlTree(['/login']);
  }

  try {
    const payload = JSON.parse(
      atob(token.split('.')[1])
    );

    const isAdmin = String(payload.IsAdmin).toLowerCase() === 'true';

    if (isAdmin) {
      return true;
    }

    return router.createUrlTree(['/forbidden']);
  } catch {
    return router.createUrlTree(['/login']);
  }
};