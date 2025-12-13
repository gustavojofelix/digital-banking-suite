import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { AuthStateService } from '../services/auth-sate/auth-state-service';

export const authGuard: CanActivateFn = (route, state): boolean | UrlTree => {
  const authState = inject(AuthStateService);
  const router = inject(Router);

  const token = authState.getAccessToken();

  if (token) {
    // User is authenticated, allow route activation
    return true;
  }

  // Not authenticated — redirect to login, preserving returnUrl
  return router.createUrlTree(['/login'], {
    queryParams: { returnUrl: state.url },
  });
};
