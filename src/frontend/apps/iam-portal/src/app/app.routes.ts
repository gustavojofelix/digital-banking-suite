import { Route } from '@angular/router';
import { LoginPage } from './features/auth/login-page/login-page';
import { TwoFactorPage } from './features/auth/two-factor-page/two-factor-page';
import { EmailConfirmationPage } from './features/auth/email-confirmation-page/email-confirmation-page';
import { ForgotPasswordPage } from './features/auth/forgot-password-page/forgot-password-page';
import { ResetPasswordPage } from './features/auth/reset-password-page/reset-password-page';
import { MySecurityPage } from './features/auth/my-security-page/my-security-page';
import { guestGuard } from './core/auth/guards/guest-guard';
import { authGuard } from './core/auth/guards/auth-guard';
import { ShellLayout } from './layout/shell-layout';
import { GuestLayout } from './layout/guest-layout';
import { EmployeesPage } from './features/admin/employees/pages/employees-page';
import { EmployeeDetailsPage } from './features/admin/employees/pages/employee-details-page';

export const appRoutes: Route[] = [
  // -------- Guest area (no main nav) --------
  {
    path: '',
    component: GuestLayout,
    canActivate: [guestGuard],
    children: [
      {
        path: 'login',
        component: LoginPage,
      },
      {
        path: 'login/2fa',
        component: TwoFactorPage,
      },
      {
        path: 'forgot-password',
        component: ForgotPasswordPage,
      },
      {
        path: 'reset-password',
        component: ResetPasswordPage,
      },
      {
        path: 'email-confirmation',
        component: EmailConfirmationPage,
      },
    ],
  },

  // -------- Authenticated portal (with ShellLayout) --------
  {
    path: '',
    component: ShellLayout,
    canActivate: [authGuard],
    children: [
      {
        path: 'me/security',
        component: MySecurityPage,
      },
      {
        path: 'admin/employees',
        component: EmployeesPage,
      },
      {
        path: 'admin/employees/:id',
        component: EmployeeDetailsPage,
      },
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'me/security',
      },
    ],
  },

  // Fallback
  {
    path: '**',
    redirectTo: 'login',
  },
];
