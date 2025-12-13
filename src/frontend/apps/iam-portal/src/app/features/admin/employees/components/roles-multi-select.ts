import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
} from '@angular/core';
import { CommonModule } from '@angular/common';

export type EmployeeRole = 'Employee' | 'IamAdmin' | 'SuperAdmin';

@Component({
  selector: 'app-roles-multi-select',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './roles-multi-select.html',
  styleUrl: './roles-multi-select.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RolesMultiSelect {
  @Input({ required: true }) selected: string[] = [];
  @Output() selectedChange = new EventEmitter<string[]>();

  readonly roles: EmployeeRole[] = ['Employee', 'IamAdmin', 'SuperAdmin'];

  toggle(role: EmployeeRole, checked: boolean): void {
    const current = new Set(this.selected ?? []);
    if (checked) current.add(role);
    else current.delete(role);

    this.selectedChange.emit(Array.from(current));
  }

  isChecked(role: EmployeeRole): boolean {
    return (this.selected ?? []).includes(role);
  }
}
