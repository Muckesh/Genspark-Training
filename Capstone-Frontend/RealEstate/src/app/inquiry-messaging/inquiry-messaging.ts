import { Component, Input, OnInit, OnDestroy, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InquiryService } from '../services/inquiry.service';
import { SignalRService } from '../services/signalr.service';
import { BehaviorSubject, Subject, takeUntil, filter, first } from 'rxjs';
import { InquiryReply, InquiryResponseDto } from '../models/inquiry.model';
import { AuthService } from '../services/auth.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-inquiry-messaging',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './inquiry-messaging.html',
  styleUrls: ['./inquiry-messaging.css']
})
export class InquiryMessaging implements OnInit, OnDestroy {
  inquiries: InquiryResponseDto[] = [];
  inquiryId: string = '';
  selectedInquiry: InquiryResponseDto | null = null;
  newMessage: string = '';
  replies$ = new BehaviorSubject<InquiryReply[]>([]);
  private destroy$ = new Subject<void>();
  private messageIds = new Set<string>(); // Track message IDs to prevent duplicates
  private optimisticMessages = new Map<string, InquiryReply>(); // Track optimistic messages
  private recentMessageHashes = new Set<string>(); // Track recent message content hashes
  isAgent = false;
  isBuyer = false;
  currentUserType: string = '';

  @ViewChild('scrollContainer') scrollContainer!: ElementRef<HTMLDivElement>;

  constructor(
    private inquiryService: InquiryService,
    private signalRService: SignalRService,
    private authService: AuthService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.inquiryId = this.route.snapshot.params['id'];
    this.isAgent = this.authService.hasRole("Agent");
    this.isBuyer = this.authService.hasRole("Buyer");
    this.currentUserType = this.isBuyer ? 'Buyer' : 'Agent';

    const loadInquiries = this.isBuyer
      ? this.inquiryService.getMyInquiries()
      : this.inquiryService.getAgentInquiries();

    loadInquiries.pipe(takeUntil(this.destroy$)).subscribe({
      next: result => {
        this.inquiries = result.items;

        // Set selectedInquiry from loaded list using ID from URL
        const matched = this.inquiries.find(i => i.id === this.inquiryId);
        if (matched) {
          this.openChat(matched);
        } else {
          console.warn('Inquiry not found for ID:', this.inquiryId);
        }
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
    this.messageIds.clear(); // Clear previous message IDs
    this.optimisticMessages.clear(); // Clear optimistic messages
    this.recentMessageHashes.clear(); // Clear recent message hashes

    if (!inquiry.replies) {
      this.inquiryService.getReplies(inquiry.id!)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: replies => {
            this.setRepliesWithDeduplication(replies);
          },
          error: err => console.error('Failed to fetch replies', err)
        });
    } else {
      this.setRepliesWithDeduplication(inquiry.replies);
    }

    this.signalRService.joinInquiryGroup(inquiry.id!)
      .catch(err => console.error('Join group failed:', err));
  }

  private setRepliesWithDeduplication(replies: InquiryReply[]): void {
    this.messageIds.clear();
    this.recentMessageHashes.clear();
    
    const uniqueReplies = replies.filter(reply => {
      const messageId = this.getMessageId(reply);
      const messageHash = this.getMessageHash(reply);
      
      if (this.messageIds.has(messageId) || this.recentMessageHashes.has(messageHash)) {
        return false;
      }
      
      this.messageIds.add(messageId);
      this.recentMessageHashes.add(messageHash);
      return true;
    });
    
    this.replies$.next(uniqueReplies);
    this.scrollToBottom();
  }

  private getMessageId(reply: InquiryReply): string {
    // Use the actual ID if available
    if (reply.id) {
      return reply.id;
    }
    
    // For messages without ID, create a unique identifier based on content and timestamp
    const timestamp = reply.createdAt ? new Date(reply.createdAt).getTime() : Date.now();
    return `${reply.inquiryId}-${reply.authorId}-${reply.authorType}-${reply.message}-${timestamp}`;
  }

  private getMessageHash(reply: InquiryReply): string {
    // Create a hash based on message content, author, and rounded timestamp (to handle slight timing differences)
    const timestamp = reply.createdAt ? Math.floor(new Date(reply.createdAt).getTime() / 1000) : Math.floor(Date.now() / 1000);
    return `${reply.inquiryId}-${reply.authorId}-${reply.authorType}-${reply.message}-${timestamp}`;
  }

  sendReply(): void {
    const msg = this.newMessage.trim();
    if (!msg || !this.selectedInquiry) return;

    const inquiryId = this.selectedInquiry.id!;
    const authorType = this.currentUserType;
    const optimisticId = `optimistic-${Date.now()}-${Math.random()}`;

    // Create optimistic reply object
    const optimisticReply: InquiryReply = {
      inquiryId: inquiryId,
      authorId: 'current-user', // You might want to get this from authService
      message: msg,
      authorType: authorType,
      createdAt: new Date()
    };

    // Store optimistic message
    this.optimisticMessages.set(optimisticId, optimisticReply);

    // Add message optimistically to UI
    this.addReplyWithDeduplication(optimisticReply);
    this.newMessage = '';

    // Send through SignalR (this will notify other users)
    this.signalRService.sendReply(inquiryId, msg, authorType)
      .catch(err => console.error('SignalR send failed:', err));

    // Persist to backend
    this.inquiryService.addReply(inquiryId, msg)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (savedReply) => {
          // Remove optimistic message and replace with saved reply
          this.optimisticMessages.delete(optimisticId);
          
          // Update the replies list by replacing the optimistic message
          const currentReplies = this.replies$.getValue();
          const optimisticMessageId = this.getMessageId(optimisticReply);
          
          // Remove the optimistic message and add the saved one
          const filteredReplies = currentReplies.filter(reply => 
            this.getMessageId(reply) !== optimisticMessageId
          );
          
          // Add the saved reply if it's not already there
          const savedMessageId = this.getMessageId(savedReply);
          const savedMessageHash = this.getMessageHash(savedReply);
          
          if (!this.messageIds.has(savedMessageId) && !this.recentMessageHashes.has(savedMessageHash)) {
            this.messageIds.add(savedMessageId);
            this.recentMessageHashes.add(savedMessageHash);
            this.replies$.next([...filteredReplies, savedReply]);
          } else {
            this.replies$.next(filteredReplies);
          }
        },
        error: (err) => {
          console.error('Failed to save reply to backend:', err);
          // Remove optimistic reply on error
          this.optimisticMessages.delete(optimisticId);
          const currentReplies = this.replies$.getValue();
          const optimisticMessageId = this.getMessageId(optimisticReply);
          const filteredReplies = currentReplies.filter(reply => 
            this.getMessageId(reply) !== optimisticMessageId
          );
          this.replies$.next(filteredReplies);
        }
      });
  }

  handleIncomingReply(reply: InquiryReply): void {
    if (this.selectedInquiry?.id === reply.inquiryId) {
      // Skip messages from current user (they're handled optimistically)
      if (reply.authorType === this.currentUserType) {
        return;
      }
      
      // Add the incoming message with deduplication
      this.addReplyWithDeduplication(reply);
    }
  }

  private addReplyWithDeduplication(reply: InquiryReply): void {
    const messageId = this.getMessageId(reply);
    const messageHash = this.getMessageHash(reply);
    
    // Check if message already exists using both ID and hash
    if (this.messageIds.has(messageId) || this.recentMessageHashes.has(messageHash)) {
      return; // Skip duplicate
    }

    // Check if this is a duplicate based on recent messages (for messages without proper IDs)
    const currentReplies = this.replies$.getValue();
    const isDuplicate = currentReplies.some(existingReply => {
      return existingReply.message === reply.message &&
             existingReply.authorType === reply.authorType &&
             existingReply.inquiryId === reply.inquiryId &&
             Math.abs(
               (existingReply.createdAt ? new Date(existingReply.createdAt).getTime() : 0) -
               (reply.createdAt ? new Date(reply.createdAt).getTime() : 0)
             ) < 2000; // Within 2 seconds
    });

    if (isDuplicate) {
      return; // Skip duplicate
    }

    this.messageIds.add(messageId);
    this.recentMessageHashes.add(messageHash);
    
    const current = this.replies$.getValue();
    this.replies$.next([...current, reply]);
    this.scrollToBottom();

    // Clean up old hashes to prevent memory leaks (keep only recent ones)
    setTimeout(() => {
      this.cleanupOldHashes();
    }, 30000); // Clean up after 30 seconds
  }

  private cleanupOldHashes(): void {
    // Keep only the most recent 100 message hashes to prevent memory leaks
    if (this.recentMessageHashes.size > 100) {
      const hashArray = Array.from(this.recentMessageHashes);
      this.recentMessageHashes.clear();
      // Keep the last 50 hashes
      hashArray.slice(-50).forEach(hash => this.recentMessageHashes.add(hash));
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
    return authorType === 'Buyer' ? 'message-outgoing' : 'message-incoming';
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.signalRService.stopConnection();
  }
}