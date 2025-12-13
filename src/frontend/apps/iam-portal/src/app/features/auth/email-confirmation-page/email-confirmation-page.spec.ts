import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { EmailConfirmationPage } from './email-confirmation-page';

describe('EmailConfirmationPage', () => {
  let component: EmailConfirmationPage;
  let fixture: ComponentFixture<EmailConfirmationPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        EmailConfirmationPage,
        HttpClientTestingModule, // Provides HttpClient for AuthApiService
        RouterTestingModule, // Provides Router/ActivatedRoute for query params
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(EmailConfirmationPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
