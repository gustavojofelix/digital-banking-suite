import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RolesMultiSelect } from './roles-multi-select';

describe('RolesMultiSelect', () => {
  let component: RolesMultiSelect;
  let fixture: ComponentFixture<RolesMultiSelect>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RolesMultiSelect],
    }).compileComponents();

    fixture = TestBed.createComponent(RolesMultiSelect);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
