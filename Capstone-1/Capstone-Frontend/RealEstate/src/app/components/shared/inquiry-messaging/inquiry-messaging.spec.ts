import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InquiryMessaging } from './inquiry-messaging';

describe('InquiryMessaging', () => {
  let component: InquiryMessaging;
  let fixture: ComponentFixture<InquiryMessaging>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InquiryMessaging]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InquiryMessaging);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
