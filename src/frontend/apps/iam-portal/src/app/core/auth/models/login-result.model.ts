export interface LoginResult {
  requiresTwoFactor: boolean;
  userId?: string | null;
  accessToken?: string | null;
  expiresAt?: string | null; // ISO-8601 timestamp
}
