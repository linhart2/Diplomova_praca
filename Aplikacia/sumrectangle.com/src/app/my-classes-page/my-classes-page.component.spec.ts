import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MyClassesPageComponent } from './my-classes-page.component';

describe('MyClassesPageComponent', () => {
  let component: MyClassesPageComponent;
  let fixture: ComponentFixture<MyClassesPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MyClassesPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MyClassesPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
