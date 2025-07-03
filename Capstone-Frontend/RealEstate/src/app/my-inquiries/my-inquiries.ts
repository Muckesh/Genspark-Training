import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { InquiryService } from '../services/inquiry.service';
import { SignalRService } from '../services/signalr.service';
import { AuthService } from '../services/auth.service';
import { InquiryReply, InquiryResponseDto } from '../models/inquiry.model';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BehaviorSubject, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-my-inquiries',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './my-inquiries.html',
  styleUrl: './my-inquiries.css'
})
export class MyInquiries implements OnInit, OnDestroy {
  inquiries: InquiryResponseDto[] = [];
  selectedInquiry: InquiryResponseDto | null = null;
  newMessage: string = '';
  replies$ = new BehaviorSubject<InquiryReply[]>([]);
  private destroy$ = new Subject<void>();

  @ViewChild('scrollContainer') scrollContainer!: ElementRef<HTMLDivElement>;

  constructor(
    private inquiryService: InquiryService,
    private signalRService: SignalRService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.inquiryService.getMyInquiries()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: result => {
          this.inquiries = result.items;
        },
        error: err => console.error('Failed to load inquiries', err)
      });

    this.signalRService.startConnection()
      .then(() => {
        this.signalRService.onReceiveReply(this.handleIncomingReply.bind(this));
      })
      .catch(err => console.error('SignalR error:', err));
  }

  openChat(inquiry: InquiryResponseDto): void {
    this.selectedInquiry = inquiry;

    if (!inquiry.replies) {
      this.inquiryService.getReplies(inquiry.id!)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: replies => this.replies$.next(replies),
          error: err => console.error('Failed to fetch replies', err)
        });
    } else {
      this.replies$.next(inquiry.replies);
    }

    this.signalRService.joinInquiryGroup(inquiry.id!)
      .catch(err => console.error('Join group failed:', err));
  }

  sendReply(): void {
    const msg = this.newMessage.trim();
    if (!msg || !this.selectedInquiry) return;

    this.signalRService.sendReply(
      this.selectedInquiry.id!,
      msg,
      'Buyer'
    ).catch(err => console.error('SignalR send failed:', err));

    this.newMessage = '';
  }

  handleIncomingReply(reply: InquiryReply): void {
    if (this.selectedInquiry?.id === reply.inquiryId) {
      const current = this.replies$.getValue();
      const exists = current.some(r => r.id === reply.id);
      // if (!exists) {
      //   const updated = [...current, reply];
      //   this.replies$.next(updated);
      //   this.scrollToBottom();
      // }
      this.replies$.next([...current,reply]);
      this.scrollToBottom();

    }
  }

  scrollToBottom(): void {
    setTimeout(() => {
      this.scrollContainer?.nativeElement.scrollTo({
        top: this.scrollContainer.nativeElement.scrollHeight,
        behavior: 'smooth'
      });
    }, 50); // ensure DOM is updated
  }

  getMessageClass(authorType: string): string {
    return authorType === 'Buyer' ? 'message-outgoing' : 'message-incoming';
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.signalRService.stopConnection();
  }
}
