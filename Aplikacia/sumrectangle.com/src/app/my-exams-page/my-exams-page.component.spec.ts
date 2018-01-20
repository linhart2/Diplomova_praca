import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MyExamsPageComponent } from './my-exams-page.component';

describe('MyExamsPageComponent', () => {
  let component: MyExamsPageComponent;
  let fixture: ComponentFixture<MyExamsPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MyExamsPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MyExamsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
