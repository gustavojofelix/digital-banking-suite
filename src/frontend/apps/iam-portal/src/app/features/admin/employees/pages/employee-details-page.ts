import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { filter, map } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { EmployeeDetailsFacadeService } from '../data-access/employee-details-facade.service';
import { UpdateEmployeeRequest } from '../models/update-employee-request';
import { RolesMultiSelect } from '../components/roles-multi-select';

@Component({
  selector: 'app-employee-details-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, RolesMultiSelect],
  templateUrl: './employee-details-page.html',
  styleUrl: './employee-details-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [EmployeeDetailsFacadeService],
})
export class EmployeeDetailsPage {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly facade = inject(EmployeeDetailsFacadeService);
  private readonly destroyRef = inject(DestroyRef);

  readonly vm$ = this.facade.vm$;

  private employeeId: string | null = null;
  private patchedForEmployeeId: string | null = null;

  readonly form = new FormGroup({
    fullName: new FormControl<string>('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(2)],
    }),
    phoneNumber: new FormControl<string | null>(null),
    roles: new FormControl<string[]>([], { nonNullable: true }),
  });

  constructor() {
    // Load employee id from route
    this.route.paramMap
      .pipe(
        map((pm) => pm.get('id')),
        filter((id): id is string => !!id),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe((id) => {
        this.employeeId = id;
        this.facade.load(id);
      });

    // Patch form when employee data arrives
    this.vm$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((vm) => {
      const e = vm.employee;
      if (!e) return;

      if (this.patchedForEmployeeId === e.id) return;
      this.patchedForEmployeeId = e.id;

      this.form.patchValue(
        {
          fullName: e.fullName,
          phoneNumber: e.phoneNumber ?? null,
          roles: e.roles ?? [],
        },
        { emitEvent: false }
      );
    });
  }

  setRoles(roles: string[]): void {
    this.form.controls.roles.setValue(roles);
  }

  save(): void {
    if (!this.employeeId) return;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const request: UpdateEmployeeRequest = {
      fullName: this.form.controls.fullName.value.trim(),
      phoneNumber: this.form.controls.phoneNumber.value,
      roles: this.form.controls.roles.value,
    };

    this.facade.save(this.employeeId, request);
  }

  activate(): void {
    if (!this.employeeId) return;
    if (!confirm('Activate this employee?')) return;
    this.facade.activate(this.employeeId);
  }

  deactivate(): void {
    if (!this.employeeId) return;
    if (!confirm('Deactivate this employee?')) return;
    this.facade.deactivate(this.employeeId);
  }

  backToList(): void {
    this.router.navigate(['/admin/employees']);
  }
}
