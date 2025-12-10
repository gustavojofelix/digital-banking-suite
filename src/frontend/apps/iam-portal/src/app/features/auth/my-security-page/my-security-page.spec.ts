import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MySecurityPage } from './my-security-page';

describe('MySecurityPage', () => {
  let component: MySecurityPage;
  let fixture: ComponentFixture<MySecurityPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MySecurityPage],
    }).compileComponents();

    fixture = TestBed.createComponent(MySecurityPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
