import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TwoFactorSessionService {
  private pendingUserId: string | null = null;

  setPendingUserId(userId: string): void {
    this.pendingUserId = userId;
  }

  getPendingUserId(): string | null {
    return this.pendingUserId;
  }

  hasPendingUserId(): boolean {
    return !!this.pendingUserId;
  }

  clear(): void {
    this.pendingUserId = null;
  }
}
