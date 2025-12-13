import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { MySecurityPage } from './my-security-page';

describe('MySecurityPage', () => {
  let component: MySecurityPage;
  let fixture: ComponentFixture<MySecurityPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        MySecurityPage,
        HttpClientTestingModule, // provides HttpClient for AuthApiService
        RouterTestingModule, // in case the page uses Router/ActivatedRoute
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(MySecurityPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
