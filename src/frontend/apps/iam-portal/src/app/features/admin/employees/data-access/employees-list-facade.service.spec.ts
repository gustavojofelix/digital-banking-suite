import { TestBed } from '@angular/core/testing';

import { EmployeesListFacadeService } from './employees-list-facade.service';

describe('EmployeesListFacadeService', () => {
  let service: EmployeesListFacadeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EmployeesListFacadeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
