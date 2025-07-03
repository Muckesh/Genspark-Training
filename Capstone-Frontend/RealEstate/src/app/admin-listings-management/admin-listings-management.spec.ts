import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminListingsManagement } from './admin-listings-management';

describe('AdminListingsManagement', () => {
  let component: AdminListingsManagement;
  let fixture: ComponentFixture<AdminListingsManagement>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminListingsManagement]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminListingsManagement);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
