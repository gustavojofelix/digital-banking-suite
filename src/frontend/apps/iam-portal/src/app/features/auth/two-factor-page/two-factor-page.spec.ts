import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { TwoFactorPage } from './two-factor-page';

describe('TwoFactorPage', () => {
  let component: TwoFactorPage;
  let fixture: ComponentFixture<TwoFactorPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        TwoFactorPage,
        HttpClientTestingModule, // <-- provides HttpClient for AuthApiService
        RouterTestingModule, // <-- avoids router-related provider errors
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(TwoFactorPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
