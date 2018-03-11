import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ActualScreenPageComponent } from './actual-screen-page.component';

describe('ActualScreenPageComponent', () => {
  let component: ActualScreenPageComponent;
  let fixture: ComponentFixture<ActualScreenPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ActualScreenPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ActualScreenPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
