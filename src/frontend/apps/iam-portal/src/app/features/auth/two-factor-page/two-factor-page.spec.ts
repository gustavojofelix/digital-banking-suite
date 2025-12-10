import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TwoFactorPage } from './two-factor-page';

describe('TwoFactorPage', () => {
  let component: TwoFactorPage;
  let fixture: ComponentFixture<TwoFactorPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TwoFactorPage],
    }).compileComponents();

    fixture = TestBed.createComponent(TwoFactorPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
