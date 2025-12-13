import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';

import { AuthStateService } from '../services/auth-sate/auth-state-service';

export const guestGuard: CanActivateFn = (): boolean | UrlTree => {
  const authState = inject(AuthStateService);
  const router = inject(Router);

  const token = authState.getAccessToken();

  if (!token) {
    // User is not authenticated — allow access to login, 2FA, etc.
    return true;
  }

  // Already authenticated — send them to a sensible "home" route
  return router.createUrlTree(['/me/security']);
};
