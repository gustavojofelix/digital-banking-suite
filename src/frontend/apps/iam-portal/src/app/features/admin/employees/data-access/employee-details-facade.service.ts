import { DestroyRef, inject, Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { EmployeesApiService } from './employees-api.service';
import { EmployeeDetails } from '../models/employee-details';
import { UpdateEmployeeRequest } from '../models/update-employee-request';

export interface EmployeeDetailsVm {
  employee: EmployeeDetails | null;
  isLoading: boolean;

  isSaving: boolean;
  error: string | null;
  success: string | null;
}

@Injectable()
export class EmployeeDetailsFacadeService {
  private readonly api = inject(EmployeesApiService);
  private readonly destroyRef = inject(DestroyRef);

  private readonly state$ = new BehaviorSubject<EmployeeDetailsVm>({
    employee: null,
    isLoading: false,
    isSaving: false,
    error: null,
    success: null,
  });

  readonly vm$ = this.state$.asObservable();

  load(id: string): void {
    this.state$.next({
      ...this.state$.value,
      isLoading: true,
      error: null,
      success: null,
    });

    this.api
      .getEmployee(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (employee) => {
          this.state$.next({
            ...this.state$.value,
            employee,
            isLoading: false,
          });
        },
        error: () => {
          this.state$.next({
            ...this.state$.value,
            isLoading: false,
            error: 'Failed to load employee details.',
          });
        },
      });
  }

  save(id: string, request: UpdateEmployeeRequest): void {
    this.state$.next({
      ...this.state$.value,
      isSaving: true,
      error: null,
      success: null,
    });

    this.api
      .updateEmployee(id, request)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.state$.next({
            ...this.state$.value,
            isSaving: false,
            success: 'Changes saved successfully.',
          });
          this.load(id);
        },
        error: () => {
          this.state$.next({
            ...this.state$.value,
            isSaving: false,
            error: 'Failed to save changes. Please try again.',
          });
        },
      });
  }

  activate(id: string): void {
    this.state$.next({
      ...this.state$.value,
      isSaving: true,
      error: null,
      success: null,
    });

    this.api
      .activate(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.state$.next({
            ...this.state$.value,
            isSaving: false,
            success: 'Employee activated.',
          });
          this.load(id);
        },
        error: () => {
          this.state$.next({
            ...this.state$.value,
            isSaving: false,
            error: 'Failed to activate employee.',
          });
        },
      });
  }

  deactivate(id: string): void {
    this.state$.next({
      ...this.state$.value,
      isSaving: true,
      error: null,
      success: null,
    });

    this.api
      .deactivate(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.state$.next({
            ...this.state$.value,
            isSaving: false,
            success: 'Employee deactivated.',
          });
          this.load(id);
        },
        error: () => {
          this.state$.next({
            ...this.state$.value,
            isSaving: false,
            error: 'Failed to deactivate employee.',
          });
        },
      });
  }
}
