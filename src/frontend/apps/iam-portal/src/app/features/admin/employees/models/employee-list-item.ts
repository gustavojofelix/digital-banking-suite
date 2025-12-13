export interface EmployeeListItem {
  id: string;
  fullName: string;
  email: string;

  isActive: boolean;
  emailConfirmed: boolean;
  twoFactorEnabled: boolean;

  roles: string[];
  lastLoginUtc?: string | null;
}
