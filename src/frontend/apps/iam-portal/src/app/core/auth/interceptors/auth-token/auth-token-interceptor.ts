import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';

import { AuthStateService } from '../../services/auth-sate/auth-state-service';

const IAM_API_URL =
  (process.env['IAM_API_URL'] as string | undefined) ?? 'http://localhost:5001';

export const authTokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authState = inject(AuthStateService);
  const token = authState.getAccessToken();

  if (!token) {
    return next(req);
  }

  // If later we call multiple backends, we can restrict this to IAM URLs only.
  if (IAM_API_URL && !req.url.startsWith(IAM_API_URL)) {
    return next(req);
  }

  const authReq = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`,
    },
  });

  return next(authReq);
};
