import { TestBed } from '@angular/core/testing';
import { AuthStateService } from './auth-state-service';

describe('AuthStateService', () => {
  let service: AuthStateService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AuthStateService],
    });

    service = TestBed.inject(AuthStateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
