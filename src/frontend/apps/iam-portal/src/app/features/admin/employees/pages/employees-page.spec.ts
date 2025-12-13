import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { Router } from '@angular/router';

import { EmployeesPage } from './employees-page';
import {
  EmployeesListFacadeService,
  EmployeesListVm,
} from '../data-access/employees-list-facade.service';

class EmployeesListFacadeStub {
  vm$ = of<EmployeesListVm>({
    query: { pageNumber: 1, pageSize: 10, search: '', includeInactive: false },
    result: {
      items: [
        {
          id: '1',
          fullName: 'Admin User',
          email: 'admin@bank.test',
          isActive: true,
          emailConfirmed: true,
          twoFactorEnabled: true,
          roles: ['IamAdmin'],
          lastLoginUtc: null,
        },
      ],
      pageNumber: 1,
      pageSize: 10,
      totalCount: 1,
    },
    isLoading: false,
    error: null,
  });

  load = jest.fn();
  setSearch = jest.fn();
  setIncludeInactive = jest.fn();
  setPageSize = jest.fn();
  setPageNumber = jest.fn();
}

describe('EmployeesPage', () => {
  it('should create and load employees', async () => {
    const facade = new EmployeesListFacadeStub();

    await TestBed.configureTestingModule({
      imports: [EmployeesPage],
      providers: [{ provide: Router, useValue: { navigate: jest.fn() } }],
    })
      .overrideComponent(EmployeesPage, {
        set: {
          providers: [
            { provide: EmployeesListFacadeService, useValue: facade },
          ],
        },
      })
      .compileComponents();

    const fixture = TestBed.createComponent(EmployeesPage);
    fixture.detectChanges();

    expect(fixture.componentInstance).toBeTruthy();
    expect(facade.load).toHaveBeenCalled();
  });
});
