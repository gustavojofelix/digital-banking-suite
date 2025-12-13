import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  DestroyRef,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { AuthApiService } from '../../../core/auth/services/auth-api/auth-api-service';
import { ForgotPasswordRequest } from '../../../core/auth/models';

@Component({
  standalone: true,
  selector: 'app-forgot-password-page',
  templateUrl: './forgot-password-page.html',
  styleUrl: './forgot-password-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
})
export class ForgotPasswordPage {
  private readonly fb = inject(FormBuilder);
  private readonly authApi = inject(AuthApiService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly cdr = inject(ChangeDetectorRef);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
  });

  isSubmitting = false;
  hasSubmitted = false;
  formError: string | null = null;

  get email() {
    return this.form.controls.email;
  }

  onSubmit(): void {
    if (this.form.invalid || this.isSubmitting) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.formError = null;
    this.cdr.markForCheck();
    const payload: ForgotPasswordRequest = this.form.getRawValue();

    this.authApi
      .forgotPassword(payload)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => {
          this.isSubmitting = false;
        })
      )
      .subscribe({
        next: () => {
          // Backend always responds with { sent: true }.
          // We always show the same generic message.
          this.hasSubmitted = true;
          this.cdr.markForCheck();
        },
        error: () => {
          // We still show the same UX; only log would differ in a real system.
          this.hasSubmitted = true;
          this.cdr.markForCheck();
        },
      });
  }
}
