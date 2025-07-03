// src/app/services/modal.service.ts
import { Injectable } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { InquiryForm } from '../inquiry-form/inquiry-form';

@Injectable({ providedIn: 'root' })
export class ModalService {
  constructor(private modalService: NgbModal) {}

  openInquiryModal(listingId: string) {
    const modalRef = this.modalService.open(InquiryForm);
    modalRef.componentInstance.listingId = listingId;
    
    modalRef.componentInstance.inquirySubmitted.subscribe(() => {
      modalRef.close();
      // You can add a toast notification here
    });
    
    modalRef.componentInstance.cancel.subscribe(() => {
      modalRef.dismiss();
    });
  }
}