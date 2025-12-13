import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GuestLayout } from './guest-layout';

describe('GuestLayout', () => {
  let component: GuestLayout;
  let fixture: ComponentFixture<GuestLayout>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GuestLayout],
    }).compileComponents();

    fixture = TestBed.createComponent(GuestLayout);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
