import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { InquiryService } from '../../../services/inquiry.service';
import { AuthService } from '../../../services/auth.service';
import { Inquiry } from '../../../models/inquiry.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-inquiry-form',
  imports: [CommonModule,ReactiveFormsModule],
  templateUrl: './inquiry-form.html',
  styleUrl: './inquiry-form.css'
})
export class InquiryForm {
  @Input() listingId!:string;
  @Output() inquirySubmitted = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  inquiryForm:FormGroup;
  isSubmitting=false;

  constructor(
    private fb:FormBuilder,
    private inquiryService:InquiryService,
    private authService:AuthService
  ){
    this.inquiryForm=this.fb.group({
      message: ['',[Validators.required,Validators.minLength(10)]]
    });
  }


  onSubmit(){
    if(this.inquiryForm.valid){
      this.isSubmitting=true;
      const inquiry:Inquiry={
        listingId:this.listingId,
        replies:[],
        buyerId:this.authService.getCurrentUser()?.id,
        message:this.inquiryForm.value.message
      };

      this.inquiryService.createInquiry(inquiry).subscribe({
        next:()=>{
          this.inquirySubmitted.emit();
          this.isSubmitting=false;
        },
        error:()=>{
          this.isSubmitting=false;
        }
      });
    }
  }

  onCancel(){
    this.cancel.emit();
  }

}
