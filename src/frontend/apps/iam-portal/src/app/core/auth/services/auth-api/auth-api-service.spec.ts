import { TestBed } from '@angular/core/testing';
import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing';

import { AuthApiService } from './auth-api-service';
import {
  LoginRequest,
  LoginResult,
  ForgotPasswordRequest,
  EmailActionResponse,
} from '../../models';

describe('AuthApiService', () => {
  let service: AuthApiService;
  let httpMock: HttpTestingController;

  const IAM_API_URL =
    (process.env['IAM_API_URL'] as string | undefined) ??
    'https://localhost:5001';
  const baseUrl = `${IAM_API_URL}/api/iam/auth`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthApiService],
    });

    service = TestBed.inject(AuthApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should call /login with email and password', () => {
    const payload: LoginRequest = {
      email: 'user@example.com',
      password: 'Password123!',
    };

    const mockResponse: LoginResult = {
      requiresTwoFactor: false,
      accessToken: 'jwt-token',
      userId: 'some-id',
      expiresAt: '2024-01-01T00:00:00Z',
    };

    service.login(payload).subscribe((result) => {
      expect(result).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${baseUrl}/login`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(payload);

    req.flush(mockResponse);
  });

  it('should call /forgot-password and parse EmailActionResponse', () => {
    const payload: ForgotPasswordRequest = {
      email: 'user@example.com',
    };

    const mockResponse: EmailActionResponse = {
      sent: true,
    };

    service.forgotPassword(payload).subscribe((result) => {
      expect(result).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${baseUrl}/forgot-password`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(payload);

    req.flush(mockResponse);
  });
});
