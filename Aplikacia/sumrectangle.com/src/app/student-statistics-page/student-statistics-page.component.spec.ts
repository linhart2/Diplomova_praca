import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentStatisticsPageComponent } from './student-statistics-page.component';

describe('StudentStatisticsPageComponent', () => {
  let component: StudentStatisticsPageComponent;
  let fixture: ComponentFixture<StudentStatisticsPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentStatisticsPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentStatisticsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
