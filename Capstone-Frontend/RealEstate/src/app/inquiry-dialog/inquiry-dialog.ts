import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { InquiryService } from '../services/inquiry.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Inquiry } from '../models/inquiry.model';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-inquiry-dialog',
  imports: [],
  templateUrl: './inquiry-dialog.html',
  styleUrl: './inquiry-dialog.css'
})
export class InquiryDialog {
  inquiryForm:FormGroup;
  // listingId:string;

  constructor(private fb:FormBuilder,
    private inquiryService:InquiryService,
    private authService:AuthService,
    public activeModal:NgbActiveModal,
    // @Inject(MAT_DIALOG_DATA) public data: {listingId:string}
  ){
    this.inquiryForm=this.fb.group({
      message:['',[Validators.required,Validators.minLength(10)]]
    });
  }

  onSubmit(){
    if(this.inquiryForm.valid){
      const inquiry:Inquiry={
        message: this.inquiryForm.value.message,
        listingId: '',
        replies:[],
        buyerId: this.authService.getCurrentUser().id,
      };

      this.inquiryService.createInquiry(inquiry).subscribe({
        next:()=>{
          this.activeModal.close(true);
        },
        error:(err)=>{
          console.error('Error creating inquiry',err);
        }
      });
    }
  }

  onCancel(){
    this.activeModal.dismiss();
  }

}
