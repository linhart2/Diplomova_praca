import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OpenExamPageComponent } from './open-exam-page.component';

describe('OpenExamPageComponent', () => {
  let component: OpenExamPageComponent;
  let fixture: ComponentFixture<OpenExamPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OpenExamPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OpenExamPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
