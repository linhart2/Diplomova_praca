import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OpenClassPageComponent } from './open-class-page.component';

describe('OpenClassPageComponent', () => {
  let component: OpenClassPageComponent;
  let fixture: ComponentFixture<OpenClassPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OpenClassPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OpenClassPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
