import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-reset-password',
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.css'
})
export class ResetPassword {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  token = this.route.snapshot.queryParamMap.get('token');
  form = this.fb.group({
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]]
  },{validator:this.passwordMatchValidator});
  submitted = false;
  message = '';
  error = '';

  passwordMatchValidator(form: FormGroup) {
    return form.get('newPassword')?.value === form.get('confirmPassword')?.value 
      ? null : { mismatch: true };
  }

  resetPassword() {
    if (this.form.invalid || !this.token) return;

    this.submitted = true;
    this.message = '';
    this.error = '';

    this.auth.resetPassword(this.token, this.form.value.newPassword!).subscribe({
      next: () => {
        this.message = 'Password updated successfully.';
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: () => {
        this.error = 'Invalid or expired token.';
      },
      complete: () => this.submitted = false
    });
  }
}

