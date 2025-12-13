import { TestBed } from '@angular/core/testing';

import { TwoFactorSessionService } from './two-factor-session-service';

describe('TwoFactorSessionService', () => {
  let service: TwoFactorSessionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TwoFactorSessionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
