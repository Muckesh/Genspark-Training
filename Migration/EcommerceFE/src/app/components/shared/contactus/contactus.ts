import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ContactService } from '../../../services/contact.service';
import { ContactRequest, ContactResponse } from '../../../models/contact.model';
import { CommonModule } from '@angular/common';
import { RecaptchaModule } from 'ng-recaptcha';

declare var grecaptcha: any;

@Component({
  selector: 'app-contactus',
  imports: [FormsModule,CommonModule,ReactiveFormsModule,RecaptchaModule],
  templateUrl: './contactus.html',
  styleUrl: './contactus.css'
})
export class Contactus implements OnInit {
  contact: ContactRequest = {
    name: '',
    email: '',
    phone: '',
    content: '',
    recaptchaToken: ''
  };

  contacts:ContactResponse[]=[];

  

  submitted = false;
  errorMessage = '';

  constructor(private contactService: ContactService) {}

   ngOnInit(): void {
    this.loadContacts();
  }



  handleCaptcha(token: string|null) {
    this.contact.recaptchaToken = token ?? '';
  }

  onSubmit() {
    this.contactService.create(this.contact).subscribe({
      next: () => {
        this.submitted = true;
        this.errorMessage = '';
        this.contact = {
          name: '',
          email: '',
          phone: '',
          content: '',
          recaptchaToken: ''
        };
        this.loadContacts();
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to submit';
      }
    });
  }

  loadContacts() {
    this.contactService.getAll().subscribe({
      next: (data) => (this.contacts = data),
      error: (err) => console.error('Failed to load contacts', err)
    });
  }
}
