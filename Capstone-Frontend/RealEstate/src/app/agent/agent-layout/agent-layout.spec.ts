import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AgentLayout } from './agent-layout';

describe('AgentLayout', () => {
  let component: AgentLayout;
  let fixture: ComponentFixture<AgentLayout>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AgentLayout]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AgentLayout);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
