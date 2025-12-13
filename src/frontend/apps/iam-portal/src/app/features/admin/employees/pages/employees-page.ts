import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { EmployeesListFacadeService } from '../data-access/employees-list-facade.service';

@Component({
  selector: 'app-employees-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './employees-page.html',
  styleUrl: './employees-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [EmployeesListFacadeService],
})
export class EmployeesPage {
  private readonly router = inject(Router);
  private readonly facade = inject(EmployeesListFacadeService);
  private readonly destroyRef = inject(DestroyRef);

  readonly vm$ = this.facade.vm$;

  readonly search = new FormControl<string>('', { nonNullable: true });
  readonly includeInactive = new FormControl<boolean>(false, {
    nonNullable: true,
  });

  readonly pageSizeOptions = [10, 20, 50];

  constructor() {
    this.facade.load();

    this.search.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe((value) => this.facade.setSearch(value));

    this.includeInactive.valueChanges
      .pipe(distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe((value) => this.facade.setIncludeInactive(value));
  }

  onPageSizeChange(value: string): void {
    this.facade.setPageSize(Number(value));
  }

  previousPage(pageNumber: number): void {
    this.facade.setPageNumber(pageNumber - 1);
  }

  nextPage(pageNumber: number): void {
    this.facade.setPageNumber(pageNumber + 1);
  }

  openEmployee(id: string): void {
    this.router.navigate(['/admin/employees', id]);
  }
}
