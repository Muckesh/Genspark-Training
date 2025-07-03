import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Inquiry, InquiryResponseDto } from '../models/inquiry.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-inquiry-card',
  imports: [CommonModule],
  templateUrl: './inquiry-card.html',
  styleUrl: './inquiry-card.css'
})
export class InquiryCard {
  @Input() inquiry!: InquiryResponseDto;
  @Input() showAgent = false;
  @Input() showBuyer = false;
  @Output() selected = new EventEmitter<string>();

  select() {
    this.selected.emit(this.inquiry.id);
  }
}
