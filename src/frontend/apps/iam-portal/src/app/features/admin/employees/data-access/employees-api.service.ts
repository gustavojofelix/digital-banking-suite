import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

//import { IAM_API_URL } from "../../../../core/http/iam-api-url.token";
import { PagedResult } from '../models/paged-result';
import { EmployeeListItem } from '../models/employee-list-item';
import { EmployeeDetails } from '../models/employee-details';
import { EmployeesQuery } from '../models/employees-query';
import { UpdateEmployeeRequest } from '../models/update-employee-request';

// IAM_API_URL is injected at build time via Nx "define" config in project.json
const IAM_API_URL =
  (process.env['IAM_API_URL'] as string | undefined) ?? 'http://localhost:5001';

@Injectable({ providedIn: 'root' })
export class EmployeesApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${IAM_API_URL}`;

  private readonly adminBase = `${this.baseUrl}/api/iam/admin`;

  listEmployees(
    query: EmployeesQuery
  ): Observable<PagedResult<EmployeeListItem>> {
    const params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize)
      .set('search', query.search?.trim() ?? '')
      .set('includeInactive', query.includeInactive);

    return this.http.get<PagedResult<EmployeeListItem>>(
      `${this.adminBase}/employees`,
      { params }
    );
  }

  getEmployee(id: string): Observable<EmployeeDetails> {
    // Backend requirement uses singular "employee" here.
    return this.http.get<EmployeeDetails>(
      `${this.adminBase}/employee/${encodeURIComponent(id)}`
    );
  }

  updateEmployee(id: string, request: UpdateEmployeeRequest): Observable<void> {
    return this.http.put<void>(
      `${this.adminBase}/employees/${encodeURIComponent(id)}`,
      request
    );
  }

  activate(id: string): Observable<void> {
    return this.http.post<void>(
      `${this.adminBase}/employees/${encodeURIComponent(id)}/activate`,
      {}
    );
  }

  deactivate(id: string): Observable<void> {
    return this.http.post<void>(
      `${this.adminBase}/employees/${encodeURIComponent(id)}/deactivate`,
      {}
    );
  }
}
