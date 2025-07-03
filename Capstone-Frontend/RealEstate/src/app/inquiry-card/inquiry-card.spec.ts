import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InquiryCard } from './inquiry-card';

describe('InquiryCard', () => {
  let component: InquiryCard;
  let fixture: ComponentFixture<InquiryCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InquiryCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InquiryCard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
