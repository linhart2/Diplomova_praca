import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ClassStatisticsPageComponent } from './class-statistics-page.component';

describe('ClassStatisticsPageComponent', () => {
  let component: ClassStatisticsPageComponent;
  let fixture: ComponentFixture<ClassStatisticsPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ClassStatisticsPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ClassStatisticsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
