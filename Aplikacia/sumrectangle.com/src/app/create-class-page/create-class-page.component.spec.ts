import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateClassPageComponent } from './create-class-page.component';

describe('CreateClassPageComponent', () => {
  let component: CreateClassPageComponent;
  let fixture: ComponentFixture<CreateClassPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateClassPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateClassPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
