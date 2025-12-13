import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { EmployeeDetailsFacadeService } from './employee-details-facade.service';
import { EmployeesApiService } from './employees-api.service';

describe('EmployeeDetailsFacadeService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        EmployeeDetailsFacadeService,
        {
          provide: EmployeesApiService,
          useValue: {
            getEmployee: jest.fn(() => of(null)),
            updateEmployee: jest.fn(() => of(void 0)),
            activate: jest.fn(() => of(void 0)),
            deactivate: jest.fn(() => of(void 0)),
          },
        },
      ],
    });
  });

  it('should be created', () => {
    const service = TestBed.inject(EmployeeDetailsFacadeService);
    expect(service).toBeTruthy();
  });
});
