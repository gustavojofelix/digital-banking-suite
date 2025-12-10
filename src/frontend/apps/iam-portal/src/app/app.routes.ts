import { Route } from '@angular/router';
import { LoginPage } from './features/auth/login-page/login-page';
import { TwoFactorPage } from './features/auth/two-factor-page/two-factor-page';
import { EmailConfirmationPage } from './features/auth/email-confirmation-page/email-confirmation-page';
import { ForgotPasswordPage } from './features/auth/forgot-password-page/forgot-password-page';
import { ResetPasswordPage } from './features/auth/reset-password-page/reset-password-page';
import { MySecurityPage } from './features/auth/my-security-page/my-security-page';
import { EmployeesPage } from './features/admin/employees/employees-page/employees-page';
import { EmployeeDetailsPage } from './features/admin/employees/employee-details-page/employee-details-page';

export const appRoutes: Route[] = [
  // Auth flows
  { path: 'login', component: LoginPage },

  { path: 'login/2fa', component: TwoFactorPage },
  { path: 'email-confirmation', component: EmailConfirmationPage },
  { path: 'forgot-password', component: ForgotPasswordPage },
  { path: 'reset-password', component: ResetPasswordPage },

  // Logged-in user security
  { path: 'me/security', component: MySecurityPage },

  // Admin employee management
  { path: 'admin/employees', component: EmployeesPage },
  { path: 'admin/employees/:id', component: EmployeeDetailsPage },

  // Default + wildcard
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  { path: '**', redirectTo: 'login' },
];
