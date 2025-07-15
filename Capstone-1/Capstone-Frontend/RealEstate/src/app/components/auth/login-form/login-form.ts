import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { Router, RouterLink } from '@angular/router';
import { LoginDto } from '../../../models/auth-dto.model';

@Component({
  selector: 'app-login-form',
  imports: [CommonModule,ReactiveFormsModule,RouterLink],
  templateUrl: './login-form.html',
  styleUrl: './login-form.css'
})
export class LoginForm {
  private fb = inject(FormBuilder);
  loginForm = this.fb.group({
    email:['',[Validators.required,Validators.email]],
    password:['',Validators.required]
  });

  errorMessage: string|null = null;
  loading=false;

  constructor(
    private authService:AuthService,
    private router:Router
  ){}

  onSubmit():void{
    if(this.loginForm.invalid){
      return;
    }

    this.loading=true;
    this.errorMessage=null;

    const credentials:LoginDto={
      email:this.loginForm.value.email!,
      password:this.loginForm.value.password!
    };

    this.authService.login(credentials).subscribe({
      next:()=>{
        // this.router.navigate([`/${(this.authService.getCurrentUser().role).toString().toLower()}`]);
        if(this.authService.hasRole('Admin')){
          this.router.navigate(['/admin']);
        }else if(this.authService.hasRole('Agent')){
          this.router.navigate(['/agent']);
        }else if(this.authService.hasRole('Buyer')){
          this.router.navigate(['/buyer']);
        }
      },
      error:(error)=>{
        // this.errorMessage=error.message || 'Login Failed. Please try again.';
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
