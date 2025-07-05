import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { RegisterAgentDto } from '../../../models/auth-dto.model';

@Component({
  selector: 'app-register-agent',
  imports: [CommonModule,ReactiveFormsModule,RouterLink],
  templateUrl: './register-agent.html',
  styleUrl: './register-agent.css'
})
export class RegisterAgent {

  private fb = inject(FormBuilder);

  constructor(private authService:AuthService,private router:Router){}

  registerAgentForm=this.fb.group({
    name:['',Validators.required],
    email:['',[Validators.required,Validators.email]],
    password:['',[Validators.required,Validators.minLength(6)]],
    licenseNumber:['',Validators.required],
    agencyName:['',Validators.required],
    phone:['',Validators.required]
  });

  errorMessage:string|null=null;
  loading = false;

  onSubmit():void{
    if(this.registerAgentForm.invalid)
      return;

    this.loading=true;
    this.errorMessage=null;

    const registerData:RegisterAgentDto={
      name:this.registerAgentForm.value.name!,
      email:this.registerAgentForm.value.email!,
      password: this.registerAgentForm.value.password!,
      licenseNumber: this.registerAgentForm.value.licenseNumber!,
      agencyName:this.registerAgentForm.value.agencyName!,
      phone: this.registerAgentForm.value.phone!
    };

    this.authService.registerAgent(registerData).subscribe({
      next:()=>{
        this.router.navigate(['']);
      },
      error:(error)=>{
        this.errorMessage=error.message|| "Registration failed. Please try again."
        this.loading=false;
      },
      complete:()=>{
        this.loading=false;
      }
    });
  }

}
