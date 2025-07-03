import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InquiryDialog } from './inquiry-dialog';

describe('InquiryDialog', () => {
  let component: InquiryDialog;
  let fixture: ComponentFixture<InquiryDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InquiryDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InquiryDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
