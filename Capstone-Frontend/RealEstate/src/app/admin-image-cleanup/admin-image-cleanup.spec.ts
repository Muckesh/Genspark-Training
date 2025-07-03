import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminImageCleanup } from './admin-image-cleanup';

describe('AdminImageCleanup', () => {
  let component: AdminImageCleanup;
  let fixture: ComponentFixture<AdminImageCleanup>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminImageCleanup]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminImageCleanup);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
