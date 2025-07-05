// import { Component, OnInit, OnDestroy } from '@angular/core';
// import { InquiryService } from '../services/inquiry.service';
// import { AuthService } from '../services/auth.service';
// import { SignalRService } from '../services/signalr.service';
// import { InquiryReply, InquiryResponseDto } from '../models/inquiry.model';
// import { CommonModule } from '@angular/common';
// import { RouterLink } from '@angular/router';
// import { FormsModule } from '@angular/forms';
// import { Subject, takeUntil } from 'rxjs';

// @Component({
//   selector: 'app-my-inquiries',
//   standalone: true,
//   imports: [CommonModule, RouterLink, FormsModule],
//   templateUrl: './my-inquiries.html',
//   styleUrl: './my-inquiries.css'
// })
// export class Test implements OnInit, OnDestroy {
//   inquiries: InquiryResponseDto[] = [];
//   selectedInquiry: InquiryResponseDto | null = null;
//   newMessage: string = '';
//   private destroy$ = new Subject<void>();

//   constructor(
//     private inquiryService: InquiryService,
//     private signalRService: SignalRService,
//     private authService: AuthService
//   ) {}

//   ngOnInit(): void {
//     this.inquiryService.getMyInquiries()
//       .pipe(takeUntil(this.destroy$))
//       .subscribe({
//         next: result => {
//           this.inquiries = result.items;
//         },
//         error: err => console.error('Failed to load inquiries', err)
//       });

//     this.signalRService.startConnection()
//       .then(() => {
//         this.signalRService.onReceiveReply(this.handleIncomingReply.bind(this));
//       })
//       .catch(err => console.error('SignalR connection error', err));
//   }

//   openChat(inquiry: InquiryResponseDto): void {
//     this.selectedInquiry = inquiry;

//     if (!inquiry.replies) {
//       this.inquiryService.getReplies(inquiry.id!)
//         .pipe(takeUntil(this.destroy$))
//         .subscribe({
//           next: replies => {
//             this.selectedInquiry!.replies = replies;
//           },
//           error: err => console.error('Failed to load replies', err)
//         });
//     }

//     this.signalRService.joinInquiryGroup(inquiry.id!)
//       .catch(err => console.error('Failed to join SignalR group', err));
//   }

//   sendReply(): void {
//     if (!this.newMessage.trim() || !this.selectedInquiry) return;

//     this.signalRService.sendReply(
//       this.selectedInquiry.id!,
//       this.newMessage.trim(),
//       'Buyer'
//     ).catch(err => console.error('Failed to send reply', err));

//     this.newMessage = '';
//   }

//   handleIncomingReply(reply: InquiryReply): void {
//     if (this.selectedInquiry?.id === reply.inquiryId) {
//       this.selectedInquiry.replies = this.selectedInquiry.replies || [];

//     //   const exists = this.selectedInquiry.replies.some(r => r.id === reply.id);
//     //   if (!exists) {
//     //     this.selectedInquiry.replies.push(reply);
//     //   }
//     }
//   }

//   ngOnDestroy(): void {
//     this.destroy$.next();
//     this.destroy$.complete();
//     this.signalRService.stopConnection();
//   }
// }
