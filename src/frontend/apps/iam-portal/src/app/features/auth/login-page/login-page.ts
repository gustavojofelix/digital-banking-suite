import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  DestroyRef,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { AuthApiService } from '../../../core/auth/services/auth-api/auth-api-service';
import { AuthStateService } from '../../../core/auth/services/auth-sate/auth-state-service';
import { TwoFactorSessionService } from '../../../core/auth/services/two-factor-session/two-factor-session-service';
import { LoginRequest } from '../../../core/auth/models';

@Component({
  standalone: true,
  selector: 'app-login-page',
  templateUrl: './login-page.html',
  styleUrl: './login-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
})
export class LoginPage {
  private readonly fb = inject(FormBuilder);
  private readonly authApi = inject(AuthApiService);
  private readonly authState = inject(AuthStateService);
  private readonly twoFactorSession = inject(TwoFactorSessionService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);
  private readonly cdr = inject(ChangeDetectorRef);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  isSubmitting = false;
  formError: string | null = null;

  get email() {
    return this.form.controls.email;
  }

  get password() {
    return this.form.controls.password;
  }

  onSubmit(): void {
    if (this.form.invalid || this.isSubmitting) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.formError = null;
    this.cdr.markForCheck();

    const raw = this.form.getRawValue();
    const payload: LoginRequest = {
      email: raw.email.trim(),
      password: raw.password,
    };

    this.authApi
      .login(payload)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (result) => {
          this.isSubmitting = false;

          if (result.requiresTwoFactor) {
            if (!result.userId) {
              this.formError =
                'We could not complete your sign-in. Please try again.';
              this.cdr.markForCheck();
              return;
            }

            this.twoFactorSession.setPendingUserId(result.userId);
            this.cdr.markForCheck();
            this.router.navigate(['/login/2fa']);
            return;
          }

          if (!result.accessToken) {
            this.formError =
              'We could not complete your sign-in. Please try again.';
            this.cdr.markForCheck();
            return;
          }

          this.authState.setSession(result.accessToken);

          const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
          const target =
            returnUrl && returnUrl !== '/login' ? returnUrl : '/me/security';

          this.cdr.markForCheck();
          this.router.navigateByUrl(target);
        },
        error: (error: HttpErrorResponse) => {
          this.isSubmitting = false;

          if (error.status === 400 || error.status === 401) {
            this.formError =
              'Invalid email or password. If the problem persists, contact support.';
          } else if (error.status === 423 || error.status === 429) {
            this.formError =
              'Your account is temporarily locked due to too many attempts. Please try again later.';
          } else {
            this.formError =
              'Something went wrong while signing you in. Please try again in a moment.';
          }

          this.cdr.markForCheck();
        },
      });
  }
}
