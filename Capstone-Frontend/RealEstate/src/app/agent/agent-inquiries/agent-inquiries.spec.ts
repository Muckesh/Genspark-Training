import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AgentInquiries } from './agent-inquiries';

describe('AgentInquiries', () => {
  let component: AgentInquiries;
  let fixture: ComponentFixture<AgentInquiries>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AgentInquiries]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AgentInquiries);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
