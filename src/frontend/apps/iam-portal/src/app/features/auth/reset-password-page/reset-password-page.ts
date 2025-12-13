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
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { AuthApiService } from '../../../core/auth/services/auth-api/auth-api-service';
import { ResetPasswordRequest } from '../../../core/auth/models';

@Component({
  standalone: true,
  selector: 'app-reset-password-page',
  templateUrl: './reset-password-page.html',
  styleUrl: './reset-password-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
})
export class ResetPasswordPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly authApi = inject(AuthApiService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly cdr = inject(ChangeDetectorRef);

  readonly form = this.fb.nonNullable.group(
    {
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]],
    },
    {
      validators: (group) => {
        const password = group.get('password')?.value;
        const confirm = group.get('confirmPassword')?.value;
        return password && confirm && password !== confirm
          ? { passwordMismatch: true }
          : null;
      },
    }
  );

  email: string | null = null;
  token: string | null = null;

  isSubmitting = false;
  isCompleted = false;
  formError: string | null = null;

  get password() {
    return this.form.controls.password;
  }

  get confirmPassword() {
    return this.form.controls.confirmPassword;
  }

  get hasPasswordMismatch(): boolean {
    return this.form.hasError('passwordMismatch');
  }

  ngOnInit(): void {
    const query = this.route.snapshot.queryParamMap;
    this.email = query.get('email');
    this.token = query.get('token');
    this.cdr.markForCheck();

    if (!this.email || !this.token) {
      // Missing parameters; show generic error and send user back to login.
      this.form.disable();
      this.formError =
        'This password reset link is invalid or has expired. Please request a new one.';
      this.cdr.markForCheck();
    }
  }

  onSubmit(): void {
    if (!this.email || !this.token) {
      return;
    }

    if (this.form.invalid || this.isSubmitting) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.formError = null;
    this.cdr.markForCheck();

    const payload: ResetPasswordRequest = {
      email: this.email,
      token: this.token,
      newPassword: this.password.value,
    };

    this.authApi
      .resetPassword(payload)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => {
          this.isSubmitting = false;
        })
      )
      .subscribe({
        next: () => {
          this.isCompleted = true;
          this.form.disable();
          this.cdr.markForCheck();
        },
        error: () => {
          this.formError =
            'We could not reset your password. Your link may have expired. Please request a new reset email.';
          this.cdr.markForCheck();
        },
      });
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}
