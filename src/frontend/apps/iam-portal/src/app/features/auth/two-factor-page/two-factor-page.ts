import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  DestroyRef,
  OnInit,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { AuthApiService } from '../../../core/auth/services/auth-api/auth-api-service';
import { AuthStateService } from '../../../core/auth/services/auth-sate/auth-state-service';
import { TwoFactorSessionService } from '../../../core/auth/services/two-factor-session/two-factor-session-service';
import { TwoFactorVerifyRequest } from '../../../core/auth/models';

@Component({
  standalone: true,
  selector: 'app-two-factor-page',
  templateUrl: './two-factor-page.html',
  styleUrl: './two-factor-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
})
export class TwoFactorPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly authApi = inject(AuthApiService);
  private readonly authState = inject(AuthStateService);
  private readonly twoFactorSession = inject(TwoFactorSessionService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly cdr = inject(ChangeDetectorRef);

  readonly form = this.fb.nonNullable.group({
    code: ['', [Validators.required, Validators.minLength(4)]],
  });

  isSubmitting = false;
  formError: string | null = null;

  get code() {
    return this.form.controls.code;
  }

  ngOnInit(): void {
    // If there is no pending 2FA user, redirect back to login.
    if (!this.twoFactorSession.hasPendingUserId()) {
      this.router.navigate(['/login']);
    }
  }

  onSubmit(): void {
    if (this.form.invalid || this.isSubmitting) {
      this.form.markAllAsTouched();
      return;
    }

    const pendingUserId = this.twoFactorSession.getPendingUserId();
    if (!pendingUserId) {
      // Defensive: if the user somehow lost the session, send them back to login.
      this.router.navigate(['/login']);
      return;
    }

    this.isSubmitting = true;
    this.formError = null;
    this.cdr.markForCheck();

    const payload: TwoFactorVerifyRequest = {
      userId: pendingUserId,
      code: this.code.value,
    };

    this.authApi
      .verifyTwoFactor(payload)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => {
          this.isSubmitting = false;
        })
      )
      .subscribe({
        next: (result) => {
          if (!result.accessToken) {
            this.formError =
              'We could not complete your sign-in. Please try again.';
            this.cdr.markForCheck();
            return;
          }

          // 2FA completed successfully: set session and clear pending user.
          this.authState.setSession(result.accessToken);
          this.twoFactorSession.clear();

          // For now we always send the user to their security page.
          // Later we can preserve a returnUrl through the 2FA flow if needed.
          this.router.navigate(['/me/security']);
        },
        error: () => {
          // Generic message: we don't reveal whether the code was wrong or expired.
          this.formError =
            'Invalid or expired verification code. Please try again.';
          this.cdr.markForCheck();
        },
      });
  }
}
