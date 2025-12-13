import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  LoginRequest,
  LoginResult,
  TwoFactorVerifyRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  ChangePasswordRequest,
  CurrentPasswordRequest,
  EmailActionResponse,
} from '../../models';

// IAM_API_URL is injected at build time via Nx "define" config in project.json
const IAM_API_URL =
  (process.env['IAM_API_URL'] as string | undefined) ?? 'http://localhost:5001';

@Injectable({
  providedIn: 'root',
})
export class AuthApiService {
  private readonly baseUrl = `${IAM_API_URL}/api/iam/auth`;

  private readonly http = inject(HttpClient);

  login(request: LoginRequest): Observable<LoginResult> {
    return this.http.post<LoginResult>(`${this.baseUrl}/login`, request);
  }

  verifyTwoFactor(request: TwoFactorVerifyRequest): Observable<LoginResult> {
    return this.http.post<LoginResult>(`${this.baseUrl}/2fa/verify`, request);
  }

  confirmEmail(userId: string, token: string): Observable<void> {
    const params = new HttpParams().set('userId', userId).set('token', token);

    return this.http.get<void>(`${this.baseUrl}/confirm-email`, { params });
  }

  resendConfirmation(
    request: ForgotPasswordRequest
  ): Observable<EmailActionResponse> {
    return this.http.post<EmailActionResponse>(
      `${this.baseUrl}/resend-confirmation`,
      request
    );
  }

  forgotPassword(
    request: ForgotPasswordRequest
  ): Observable<EmailActionResponse> {
    return this.http.post<EmailActionResponse>(
      `${this.baseUrl}/forgot-password`,
      request
    );
  }

  resetPassword(request: ResetPasswordRequest): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/reset-password`, request);
  }

  changePassword(request: ChangePasswordRequest): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/change-password`, request);
  }

  enableTwoFactor(request: CurrentPasswordRequest): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/2fa/enable`, request);
  }

  disableTwoFactor(request: CurrentPasswordRequest): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/2fa/disable`, request);
  }
}
