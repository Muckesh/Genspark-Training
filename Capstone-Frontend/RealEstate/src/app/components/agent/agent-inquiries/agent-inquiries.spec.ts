import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { AgentInquiries } from './agent-inquiries';
import { InquiryService } from '../../../services/inquiry.service';
import { SignalRService } from '../../../services/signalr.service';
import { BehaviorSubject, of, Subject } from 'rxjs';
import { InquiryResponseDto, InquiryReply, InquiryWithRepliesDto } from '../../../models/inquiry.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { PagedResult } from '../../../models/paged-result.model';

describe('AgentInquiries', () => {
  let component: AgentInquiries;
  let fixture: ComponentFixture<AgentInquiries>;
  let inquiryServiceSpy: jasmine.SpyObj<InquiryService>;
  let signalRServiceSpy: jasmine.SpyObj<SignalRService>;

  const mockInquiries: InquiryResponseDto[] = [
    {
      id: '1',
      listingId: 'listing1',
      listingTitle: 'Luxury Villa',
      buyerId: 'buyer1',
      buyerName: 'John Buyer',
      agentId: 'agent1',
      agentName: 'Jane Agent',
      status: 'Active',
      message: 'Test inquiry 1',
      createdAt: new Date(),
      replies: []
    },
    {
      id: '2',
      listingId: 'listing1',
      listingTitle: 'Luxury Villa',
      buyerId: 'buyer1',
      buyerName: 'John Buyer',
      agentId: 'agent1',
      agentName: 'Jane Agent',
      status: 'Active',
      message: 'Test inquiry 2',
      createdAt: new Date(),
      replies: []
    }
  ];

  const mockReplies: InquiryReply[] = [
    {
      id: '1',
      inquiryId: '1',
      authorId: 'agent1',
      authorType: 'Agent',
      message: 'Test reply from agent',
      createdAt: new Date()
    }
  ];

  const mockInquiryWithReplies: InquiryWithRepliesDto = {
    id: '1',
    message: 'Test inquiry with replies',
    status: 'Active',
    createdAt: new Date(),
    replies: mockReplies,
    listing: {
      id: 'listing1',
      title: 'Luxury Villa',
      price: 1000000,
      location: 'Beverly Hills'
    } as any
  };

  beforeEach(async () => {
    inquiryServiceSpy = jasmine.createSpyObj('InquiryService', [
      'getAgentInquiries',
      'getReplies'
    ]);

    signalRServiceSpy = jasmine.createSpyObj('SignalRService', [
      'startConnection',
      'onReceiveReply',
      'joinInquiryGroup',
      'sendReply',
      'stopConnection'
    ]);

    signalRServiceSpy.startConnection.and.returnValue(Promise.resolve());
    signalRServiceSpy.sendReply.and.returnValue(Promise.resolve());

    await TestBed.configureTestingModule({
      imports: [
        CommonModule,
        FormsModule,
        RouterTestingModule,
        AgentInquiries
      ],
      providers: [
        { provide: InquiryService, useValue: inquiryServiceSpy },
        { provide: SignalRService, useValue: signalRServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AgentInquiries);
    component = fixture.componentInstance;

    inquiryServiceSpy.getAgentInquiries.and.returnValue(of({
      items: mockInquiries,
      totalCount: 2
    } as PagedResult<InquiryResponseDto>));
    inquiryServiceSpy.getReplies.and.returnValue(of(mockReplies));

    fixture.detectChanges();
  });

  it('should create component and initialize with agent inquiries', fakeAsync(() => {
    expect(component).toBeTruthy();
    expect(inquiryServiceSpy.getAgentInquiries).toHaveBeenCalled();
    tick();
    expect(component.inquiries.length).toBe(2);
    expect(component.inquiries[0].listingTitle).toBe('Luxury Villa');
    expect(component.inquiries[0].buyerName).toBe('John Buyer');
  }));

  

it('should send messages and handle incoming replies', fakeAsync(() => {
    // Set initial state with empty replies
    component.replies$.next([]);
    component.selectedInquiry = mockInquiries[0];
    component.newMessage = 'Hello buyer';
    
    // Send first message
    component.sendMessage();
    tick();
    
    expect(signalRServiceSpy.sendReply).toHaveBeenCalledWith(
      '1',
      'Hello buyer',
      'Agent'
    );
    expect(component.newMessage).toBe('');

    // Verify initial state after send (should still be empty until reply arrives)
    expect(component.replies$.getValue().length).toBe(0);

    // Simulate incoming reply from buyer
    const buyerReply: InquiryReply = {
      id: '2',
      inquiryId: '1',
      authorId: 'buyer1',
      authorType: 'Buyer',
      message: 'Hello agent',
      createdAt: new Date()
    };
    
    component.handleIncomingReply(buyerReply);
    expect(component.replies$.getValue().length).toBe(1);
    expect(component.replies$.getValue()[0].message).toBe('Hello agent');

    // Send another message
    component.newMessage = 'How can I help?';
    component.sendMessage();
    tick();

    // Simulate our own message appearing (from SignalR)
    const agentReply: InquiryReply = {
      id: '3',
      inquiryId: '1',
      authorId: 'agent1',
      authorType: 'Agent',
      message: 'How can I help?',
      createdAt: new Date()
    };
    
    component.handleIncomingReply(agentReply);
    expect(component.replies$.getValue().length).toBe(2);
    expect(component.replies$.getValue()[1].message).toBe('How can I help?');
  }));

  it('should properly clean up resources on destroy', () => {
    component.ngOnDestroy();
    expect(signalRServiceSpy.stopConnection).toHaveBeenCalled();
    // Verify the destroy$ subject completes
    expect(component['destroy$'].isStopped).toBeTrue();
  });

  it('should return correct CSS classes for messages', () => {
    expect(component.getMessageClass('Agent')).toBe('message-outgoing');
    expect(component.getMessageClass('Buyer')).toBe('message-incoming');
    expect(component.getMessageClass('Unknown')).toBe('message-incoming');
  });

  it('should group inquiries by buyer and listing', () => {
    const testInquiries: InquiryResponseDto[] = [
      {...mockInquiries[0], buyerId: 'buyer1', listingId: 'listing1'},
      {...mockInquiries[1], buyerId: 'buyer1', listingId: 'listing1'},
      {...mockInquiries[0], id: '3', buyerId: 'buyer2', listingId: 'listing1'}
    ];
    
    component.inquiries = testInquiries;
    component['groupInquiries'](testInquiries);
    
    expect(Object.keys(component.groupedInquiries).length).toBe(2);
    expect(component.groupedInquiries['buyer1-listing1'].length).toBe(2);
    expect(component.groupedInquiries['buyer2-listing1'].length).toBe(1);
  });
});