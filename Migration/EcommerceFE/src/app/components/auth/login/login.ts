import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { Router } from '@angular/router';
import { AuthLoginRequest } from '../../../models/auth.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [CommonModule,ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  private fb = inject(FormBuilder);
  loginForm = this.fb.group({
    username: ['',[Validators.required, Validators.minLength(3)]],
    password: ['',[Validators.required, Validators.minLength(6)]]
  });

  errorMessage: string | null = null;
  loading = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit():void{
    if(this.loginForm.invalid)
      return;

    this.loading=true;
    this.errorMessage=null;

    const credentials:AuthLoginRequest={
      username:this.loginForm.value.username!,
      password:this.loginForm.value.password!
    };

    this.authService.login(credentials).subscribe({
      next:()=>{
        if(this.authService.hasRole("Admin"))
          this.router.navigate(['/']);
        else
          this.router.navigate(['/']);
      },
      error:(error)=>{
        this.loading=false;
        if (error.error && error.error.error) {
          this.errorMessage = error.error.error;
        } else if (error.status === 0) {
          this.errorMessage = 'Server is unreachable. Please try again later.';
        } else {
          this.errorMessage = 'Login failed. Please try again.';
        }
      },
      complete:()=>{
        this.loading=false;
      }
    });
  }

}
