import { DestroyRef, inject, Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { EmployeesApiService } from './employees-api.service';
import { EmployeesQuery } from '../models/employees-query';
import { EmployeeListItem } from '../models/employee-list-item';
import { PagedResult } from '../models/paged-result';

export interface EmployeesListVm {
  query: EmployeesQuery;
  result: PagedResult<EmployeeListItem> | null;
  isLoading: boolean;
  error: string | null;
}

@Injectable()
export class EmployeesListFacadeService {
  private readonly api = inject(EmployeesApiService);
  private readonly destroyRef = inject(DestroyRef);

  private readonly state$ = new BehaviorSubject<EmployeesListVm>({
    query: {
      pageNumber: 1,
      pageSize: 10,
      search: '',
      includeInactive: false,
    },
    result: null,
    isLoading: false,
    error: null,
  });

  readonly vm$ = this.state$.asObservable();

  load(): void {
    const { query } = this.state$.value;

    this.state$.next({
      ...this.state$.value,
      isLoading: true,
      error: null,
    });

    this.api
      .listEmployees(query)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (result) => {
          this.state$.next({
            ...this.state$.value,
            result,
            isLoading: false,
          });
        },
        error: () => {
          this.state$.next({
            ...this.state$.value,
            isLoading: false,
            error: 'Failed to load employees. Please try again.',
          });
        },
      });
  }

  setSearch(search: string): void {
    this.state$.next({
      ...this.state$.value,
      query: { ...this.state$.value.query, search, pageNumber: 1 },
    });
    this.load();
  }

  setIncludeInactive(includeInactive: boolean): void {
    this.state$.next({
      ...this.state$.value,
      query: { ...this.state$.value.query, includeInactive, pageNumber: 1 },
    });
    this.load();
  }

  setPageNumber(pageNumber: number): void {
    this.state$.next({
      ...this.state$.value,
      query: { ...this.state$.value.query, pageNumber },
    });
    this.load();
  }

  setPageSize(pageSize: number): void {
    this.state$.next({
      ...this.state$.value,
      query: { ...this.state$.value.query, pageSize, pageNumber: 1 },
    });
    this.load();
  }
}
