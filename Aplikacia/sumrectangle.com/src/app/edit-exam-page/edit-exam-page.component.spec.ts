import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditExamPageComponent } from './edit-exam-page.component';

describe('EditExamPageComponent', () => {
  let component: EditExamPageComponent;
  let fixture: ComponentFixture<EditExamPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditExamPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditExamPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
