import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminViewUser } from './admin-view-user';

describe('AdminViewUser', () => {
  let component: AdminViewUser;
  let fixture: ComponentFixture<AdminViewUser>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminViewUser]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminViewUser);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
