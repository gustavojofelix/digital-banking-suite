import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthStateService } from '../core/auth/services/auth-sate/auth-state-service';

@Component({
  standalone: true,
  selector: 'app-shell-layout',
  templateUrl: './shell-layout.html',
  styleUrl: './shell-layout.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, RouterModule],
})
export class ShellLayout {
  private readonly router = inject(Router);
  private readonly authState = inject(AuthStateService);

  readonly isAuthenticated$ = this.authState.isAuthenticated$;

  signOut(): void {
    this.authState.clearSession();
    this.router.navigate(['/login']);
  }
}
