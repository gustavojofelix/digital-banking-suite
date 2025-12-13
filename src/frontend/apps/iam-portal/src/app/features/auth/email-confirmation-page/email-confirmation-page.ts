import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  DestroyRef,
  OnInit,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { AuthApiService } from '../../../core/auth/services/auth-api/auth-api-service';

@Component({
  standalone: true,
  selector: 'app-email-confirmation-page',
  templateUrl: './email-confirmation-page.html',
  styleUrl: './email-confirmation-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, RouterModule],
})
export class EmailConfirmationPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly authApi = inject(AuthApiService);
  private readonly destroyRef = inject(DestroyRef);

  private readonly cdr = inject(ChangeDetectorRef);

  isLoading = true;
  isSuccess = false;
  errorMessage: string | null = null;

  ngOnInit(): void {
    const query = this.route.snapshot.queryParamMap;
    const userId = query.get('userId');
    const token = query.get('token');

    if (!userId || !token) {
      this.isLoading = false;
      this.isSuccess = false;
      this.errorMessage =
        'This email confirmation link is invalid or has expired. Please request a new one or contact support.';
      this.cdr.markForCheck();
      return;
    }

    this.authApi
      .confirmEmail(userId, token)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe({
        next: () => {
          this.isSuccess = true;
          this.cdr.markForCheck();
        },
        error: () => {
          this.isSuccess = false;
          this.errorMessage =
            'We could not confirm your email. Your link may have expired or already been used. Please try signing in or request a new confirmation email.';
          this.cdr.markForCheck();
        },
      });
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}
