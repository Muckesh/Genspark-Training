import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { InquiryService } from '../../../services/inquiry.service';
import { SignalRService } from '../../../services/signalr.service';
import { InquiryReply, InquiryResponseDto } from '../../../models/inquiry.model';
import { Subject, BehaviorSubject, takeUntil } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-agent-inquiries',
  standalone: true,
  imports: [CommonModule, FormsModule,RouterLink],
  templateUrl: './agent-inquiries.html',
  styleUrls: ['./agent-inquiries.css']
})
export class AgentInquiries implements OnInit, OnDestroy {
  groupedInquiries: Record<string, InquiryResponseDto[]> = {};
  inquiries: InquiryResponseDto[] = [];
  selectedInquiry: InquiryResponseDto | null = null;
  newMessage = '';
  replies$ = new BehaviorSubject<InquiryReply[]>([]);
  private destroy$ = new Subject<void>();

  @ViewChild('scrollContainer') scrollContainer!: ElementRef<HTMLDivElement>;

  constructor(
    private inquiryService: InquiryService,
    private signalRService: SignalRService
  ) {}

  ngOnInit(): void {
    this.inquiryService.getAgentInquiries()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: result => {
          this.inquiries = result.items;
        },
        error: (err) => console.error('Failed to load agent inquiries', err)
      });

    this.signalRService.startConnection().then(() => {
      this.signalRService.onReceiveReply(this.handleIncomingReply.bind(this));
    });
  }

  private groupInquiries(inquiries: InquiryResponseDto[]): void {
    this.groupedInquiries = {};
    for (const inquiry of inquiries) {
      const key = `${inquiry.buyerId}-${inquiry.listingId}`;
      if (!this.groupedInquiries[key]) this.groupedInquiries[key] = [];
      this.groupedInquiries[key].push(inquiry);
    }
  }

  openChat(inquiry: InquiryResponseDto): void {
    this.selectedInquiry = inquiry;

    if (!inquiry.replies) {
      this.inquiryService.getReplies(inquiry.id)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: replies => this.replies$.next(replies),
          error: err => console.error('Failed to fetch replies', err)
        });
    } else {
      this.replies$.next(inquiry.replies);
    }

    this.signalRService.joinInquiryGroup(inquiry.id).catch(console.error);
  }

  sendMessage(): void {
    const msg = this.newMessage.trim();
    if (!msg || !this.selectedInquiry) return;

    this.signalRService.sendReply(
      this.selectedInquiry.id,
      msg,
      'Agent'
    ).then(() => {
      this.newMessage = '';
    }).catch(console.error);
  }

  handleIncomingReply(reply: InquiryReply): void {
    if (this.selectedInquiry?.id === reply.inquiryId) {
      const current = this.replies$.getValue();
      const exists = current.some(r => r.id === reply.id);
      // if (!exists) {
      //   this.replies$.next([...current, reply]);
      //   this.scrollToBottom();
      // }
      this.replies$.next([...current, reply]);
      this.scrollToBottom();
    }
  }

  scrollToBottom(): void {
    setTimeout(() => {
      this.scrollContainer?.nativeElement.scrollTo({
        top: this.scrollContainer.nativeElement.scrollHeight,
        behavior: 'smooth'
      });
    }, 50);
  }

  getMessageClass(authorType: string): string {
    return authorType === 'Agent' ? 'message-outgoing' : 'message-incoming';
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.signalRService.stopConnection();
  }
}
