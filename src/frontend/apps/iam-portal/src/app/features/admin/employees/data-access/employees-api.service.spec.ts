import { TestBed } from '@angular/core/testing';

import { EmployeesApiService } from './employees-api.service';

describe('EmployeesApiService', () => {
  let service: EmployeesApiService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EmployeesApiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
