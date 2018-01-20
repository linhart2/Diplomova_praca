import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowExamPageComponent } from './show-exam-page.component';

describe('ShowExamPageComponent', () => {
  let component: ShowExamPageComponent;
  let fixture: ComponentFixture<ShowExamPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShowExamPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowExamPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
