import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

import { EmployeesApiService } from './employees-api.service';
//import { IAM_API_URL } from '../../../../core/http/iam-api-url.token';

// IAM_API_URL is injected at build time via Nx "define" config in project.json
const IAM_API_URL =
  (process.env['IAM_API_URL'] as string | undefined) ?? 'http://localhost:5001';

describe('EmployeesApiService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: IAM_API_URL, useValue: 'http://localhost:5001' },
      ],
    });
  });

  it('should be created', () => {
    const service = TestBed.inject(EmployeesApiService);
    expect(service).toBeTruthy();
  });
});
