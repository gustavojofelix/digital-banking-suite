import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { EmployeesListFacadeService } from './employees-list-facade.service';
import { EmployeesApiService } from './employees-api.service';

describe('EmployeesListFacadeService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        EmployeesListFacadeService,
        {
          provide: EmployeesApiService,
          useValue: {
            listEmployees: jest.fn(() =>
              of({
                items: [],
                pageNumber: 1,
                pageSize: 10,
                totalCount: 0,
              })
            ),
          },
        },
      ],
    });
  });

  it('should be created', () => {
    const service = TestBed.inject(EmployeesListFacadeService);
    expect(service).toBeTruthy();
  });
});
