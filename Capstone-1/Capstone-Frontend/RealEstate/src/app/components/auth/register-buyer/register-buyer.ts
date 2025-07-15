import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { RegisterBuyerDto } from '../../../models/auth-dto.model';

@Component({
  selector: 'app-register-buyer',
  imports: [CommonModule,ReactiveFormsModule,RouterLink],
  templateUrl: './register-buyer.html',
  styleUrl: './register-buyer.css'
})
export class RegisterBuyer {
  private fb = inject(FormBuilder);

  constructor(private authService:AuthService,private router:Router){}

  registerBuyerForm = this.fb.group({
    name:['',Validators.required],
    email:['',[Validators.required,Validators.email]],
    password:['',[Validators.required,Validators.minLength(6)]],
    phone:['',Validators.required],
    preferredLocation:['',Validators.required],
    budget:[0,Validators.required]
  });

  errorMessage:string|null=null;
  loading = false;

  onSubmit():void{
    if(this.registerBuyerForm.invalid)
      return;

    this.loading=true;
    this.errorMessage=null;

    const registerData: RegisterBuyerDto={
      name:this.registerBuyerForm.value.name!,
      email:this.registerBuyerForm.value.email!,
      password:this.registerBuyerForm.value.password!,
      phone: this.registerBuyerForm.value.phone!,
      preferredLocation:this.registerBuyerForm.value.preferredLocation!,
      budget:this.registerBuyerForm.value.budget!
    };

    this.authService.registerBuyer(registerData).subscribe({
      next:()=>{
        this.router.navigate(['/buyer']);
      },
      error:(error)=>{
        this.errorMessage=error.message || 'Registration failed. Please try again.';
        this.loading=false;
      },
      complete:()=>{
        this.loading=false;
      }
    });
  }

}
