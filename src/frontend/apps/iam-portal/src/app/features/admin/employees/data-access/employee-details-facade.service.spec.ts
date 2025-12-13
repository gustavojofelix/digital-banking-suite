import { TestBed } from '@angular/core/testing';

import { EmployeeDetailsFacadeService } from './employee-details-facade.service';

describe('EmployeeDetailsFacadeService', () => {
  let service: EmployeeDetailsFacadeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EmployeeDetailsFacadeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
