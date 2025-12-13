export interface EmployeeDetails {
  id: string;

  fullName: string;
  email: string;
  phoneNumber: string | null;

  isActive: boolean;
  emailConfirmed: boolean;
  twoFactorEnabled: boolean;

  roles: string[];
  lastLoginUtc?: string | null;
}
