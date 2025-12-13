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
import { RouterModule } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { AuthApiService } from '../../../core/auth/services/auth-api/auth-api-service';
import {
  ChangePasswordRequest,
  CurrentPasswordRequest,
} from '../../../core/auth/models';

@Component({
  standalone: true,
  selector: 'app-my-security-page',
  templateUrl: './my-security-page.html',
  styleUrl: './my-security-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
})
export class MySecurityPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly authApi = inject(AuthApiService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly cdr = inject(ChangeDetectorRef);

  // In a later chapter we'll load this from a /me endpoint.
  isTwoFactorEnabled = false;

  // Change password form
  readonly changePasswordForm = this.fb.nonNullable.group(
    {
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmNewPassword: ['', [Validators.required]],
    },
    {
      validators: (group) => {
        const newPassword = group.get('newPassword')?.value;
        const confirm = group.get('confirmNewPassword')?.value;
        return newPassword && confirm && newPassword !== confirm
          ? { passwordMismatch: true }
          : null;
      },
    }
  );

  isChangingPassword = false;
  changePasswordSuccess: string | null = null;
  changePasswordError: string | null = null;

  // 2FA form (current password required for enable/disable)
  readonly twoFactorForm = this.fb.nonNullable.group({
    currentPassword: ['', [Validators.required]],
  });

  isUpdatingTwoFactor = false;
  twoFactorSuccess: string | null = null;
  twoFactorError: string | null = null;

  get currentPassword() {
    return this.changePasswordForm.controls.currentPassword;
  }

  get newPassword() {
    return this.changePasswordForm.controls.newPassword;
  }

  get confirmNewPassword() {
    return this.changePasswordForm.controls.confirmNewPassword;
  }

  get hasPasswordMismatch(): boolean {
    return this.changePasswordForm.hasError('passwordMismatch');
  }

  get twoFactorCurrentPassword() {
    return this.twoFactorForm.controls.currentPassword;
  }

  ngOnInit(): void {
    // Placeholder: in a real system, we'd load the current 2FA status from the backend.
    // this.loadCurrentSecuritySettings();
    console.log('loadCurrentSecuritySettings');
  }

  onChangePasswordSubmit(): void {
    if (this.changePasswordForm.invalid || this.isChangingPassword) {
      this.changePasswordForm.markAllAsTouched();
      return;
    }

    this.isChangingPassword = true;
    this.changePasswordSuccess = null;
    this.changePasswordError = null;
    this.cdr.markForCheck();

    const payload: ChangePasswordRequest = {
      currentPassword: this.currentPassword.value,
      newPassword: this.newPassword.value,
    };

    this.authApi
      .changePassword(payload)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => {
          this.isChangingPassword = false;
        })
      )
      .subscribe({
        next: () => {
          this.changePasswordSuccess = 'Your password has been updated.';
          this.changePasswordForm.reset();
          this.cdr.markForCheck();
        },
        error: () => {
          this.changePasswordError =
            'We could not update your password. Please check your current password and try again.';
          this.cdr.markForCheck();
        },
      });
  }

  onEnableTwoFactor(): void {
    this.updateTwoFactor(true);
  }

  onDisableTwoFactor(): void {
    this.updateTwoFactor(false);
  }

  private updateTwoFactor(enable: boolean): void {
    if (this.twoFactorForm.invalid || this.isUpdatingTwoFactor) {
      this.twoFactorForm.markAllAsTouched();
      return;
    }

    this.isUpdatingTwoFactor = true;
    this.twoFactorSuccess = null;
    this.twoFactorError = null;

    const payload: CurrentPasswordRequest = {
      currentPassword: this.twoFactorCurrentPassword.value,
    };

    const request$ = enable
      ? this.authApi.enableTwoFactor(payload)
      : this.authApi.disableTwoFactor(payload);

    request$
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => {
          this.isUpdatingTwoFactor = false;
        })
      )
      .subscribe({
        next: () => {
          this.isTwoFactorEnabled = enable;
          this.twoFactorForm.reset();

          this.twoFactorSuccess = enable
            ? 'Two-factor authentication has been enabled for your account.'
            : 'Two-factor authentication has been disabled for your account.';
          this.cdr.markForCheck();
        },
        error: () => {
          this.twoFactorError =
            'We could not update your two-factor settings. Please check your password and try again.';
          this.cdr.markForCheck();
        },
      });
  }
}
