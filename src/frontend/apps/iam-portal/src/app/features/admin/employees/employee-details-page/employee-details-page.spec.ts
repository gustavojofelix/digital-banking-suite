import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EmployeeDetailsPage } from './employee-details-page';

describe('EmployeeDetailsPage', () => {
  let component: EmployeeDetailsPage;
  let fixture: ComponentFixture<EmployeeDetailsPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmployeeDetailsPage],
    }).compileComponents();

    fixture = TestBed.createComponent(EmployeeDetailsPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
