import { Component, inject } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@microsoft/signalr';
import { AuthService } from '../../../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-forgot-password',
  imports: [CommonModule,ReactiveFormsModule,FormsModule],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css'
})
export class ForgotPassword {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  form = this.fb.group({
    email:['',[Validators.required,Validators.email]]
  });


  submitted = false;
  message="";
  error="";

  requestReset(){
    if (this.form.invalid) return; 

    this.submitted=true;
    this.message='';
    this.error='';

    this.authService.requestPasswordReset(this.form.value.email!).subscribe({
      next: () => {
        this.message = 'If your email exists, a reset link has been sent.';
      },
      error: () => {
        this.error = 'Something went wrong. Try again.';
      },
      complete: () => this.submitted = false
    });

  }
}
