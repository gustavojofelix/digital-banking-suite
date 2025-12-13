import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ActivatedRoute, convertToParamMap } from '@angular/router';

import { EmployeeDetailsPage } from './employee-details-page';
import {
  EmployeeDetailsFacadeService,
  EmployeeDetailsVm,
} from '../data-access/employee-details-facade.service';

class EmployeeDetailsFacadeStub {
  vm$ = of<EmployeeDetailsVm>({
    employee: {
      id: '1',
      fullName: 'Admin User',
      email: 'admin@bank.test',
      phoneNumber: null,
      isActive: true,
      emailConfirmed: true,
      twoFactorEnabled: true,
      roles: ['IamAdmin'],
      lastLoginUtc: null,
    },
    isLoading: false,
    isSaving: false,
    error: null,
    success: null,
  });

  load = jest.fn();
  save = jest.fn();
  activate = jest.fn();
  deactivate = jest.fn();
}

describe('EmployeeDetailsPage', () => {
  it('should create and load employee by route id', async () => {
    const facade = new EmployeeDetailsFacadeStub();

    await TestBed.configureTestingModule({
      imports: [EmployeeDetailsPage],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: { paramMap: of(convertToParamMap({ id: '1' })) },
        },
      ],
    })
      // IMPORTANT: override component providers (component-level providers win otherwise)
      .overrideComponent(EmployeeDetailsPage, {
        set: {
          providers: [
            { provide: EmployeeDetailsFacadeService, useValue: facade },
          ],
        },
      })
      .compileComponents();

    const fixture = TestBed.createComponent(EmployeeDetailsPage);
    fixture.detectChanges();

    expect(fixture.componentInstance).toBeTruthy();
    expect(facade.load).toHaveBeenCalledWith('1');
  });
});
