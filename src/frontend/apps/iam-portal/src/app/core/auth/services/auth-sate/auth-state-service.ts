import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthStateService {
  private static readonly ACCESS_TOKEN_KEY = 'alvor_iam_access_token';

  private readonly accessTokenSubject = new BehaviorSubject<string | null>(
    this.readInitialToken()
  );

  readonly accessToken$: Observable<string | null> =
    this.accessTokenSubject.asObservable();

  readonly isAuthenticated$: Observable<boolean> = this.accessToken$.pipe(
    map((token) => !!token)
  );

  private readInitialToken(): string | null {
    if (typeof window === 'undefined') {
      return null;
    }

    try {
      return window.localStorage.getItem(AuthStateService.ACCESS_TOKEN_KEY);
    } catch {
      // In a real bank we might log this via a monitoring service.
      return null;
    }
  }

  getAccessToken(): string | null {
    return this.accessTokenSubject.value;
  }

  setSession(accessToken: string | null): void {
    this.accessTokenSubject.next(accessToken);

    if (typeof window === 'undefined') {
      return;
    }

    try {
      if (accessToken) {
        window.localStorage.setItem(
          AuthStateService.ACCESS_TOKEN_KEY,
          accessToken
        );
      } else {
        window.localStorage.removeItem(AuthStateService.ACCESS_TOKEN_KEY);
      }
    } catch {
      // Intentionally swallowed; failure to persist should not crash the app.
    }
  }

  clearSession(): void {
    this.setSession(null);
  }
}
