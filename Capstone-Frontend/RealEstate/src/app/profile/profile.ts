import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { BuyerService } from '../services/buyer.service';
import { UserService } from '../services/user.service';
import { User } from '../models/user.model';
import { UpdateBuyerDto } from '../models/buyer.model';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TruncatePipe } from '../pipes/truncate.pipe';
import { Subject, takeUntil } from 'rxjs';
import { AgentService } from '../services/agent.service';
import { UpdateAgentDto } from '../models/agent.model';

@Component({
  selector: 'app-profile',
  imports: [CommonModule,ReactiveFormsModule,RouterLink,CurrencyPipe],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class Profile implements OnInit,OnDestroy{
  user!:User;
  profileForm!:FormGroup;
  passwordForm!:FormGroup;
  isProfileEditing=false;
  isPasswordEditing=false;
  profileUpdateMessage: string | null = null;
  passwordUpdateMessage: string | null = null;
  profileUpdateError: string | null = null;
  passwordUpdateError: string | null = null;
  // updateDto!:UpdateBuyerDto;
  isBuyer=false;
  isAgent=false;
  isAdmin=false;
  isLoading = true;
  error: string | null = null;
  private destroy$ = new Subject<void>();

  constructor(
    private authService:AuthService,
    private buyerService:BuyerService,
    private userService:UserService,
    private agentService:AgentService,
    private fb:FormBuilder
  ){}


  ngOnInit(): void {
    // this.user = this.authService.getCurrentUser();

    // this.profileForm = this.fb.group({
    //   preferredLocation:[this.user.buyerProfile?.preferredLocation || ''],
    //   budget:[this.user.buyerProfile?.budget || 0]
    // });

    // this.passwordForm = this.fb.group({
    //   oldPassword:[''],
    //   newPassword:['']
    // });
    // this.loadUserProfile();
    this.isLoading=true;
    this.authService.currentUser$
    .pipe(takeUntil(this.destroy$))
    .subscribe({
        next: (user) => {
          if (user) {
            this.user = user;
            this.isBuyer = this.authService.hasRole('Buyer');
            this.isAgent = this.authService.hasRole('Agent');
            this.isAdmin=this.authService.hasRole('Admin');
            this.initializeForms();
            this.isLoading = false;
          }
        },
        error: (err) => {
          this.error = 'Failed to load profile data. Please try again later.';
          this.isLoading = false;
          console.error('Profile loading error:', err);
        }
    });
    // this.initializeForms();

  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
  
  // loadUserProfile(){
  //   this.user=this.authService.getCurrentUser();
  // }

  initializeForms(){
    // this.profileForm = this.fb.group({
    //   // name:[this.user.name||'',[Validators.required]],
    //   // email:[this.user.email||'',[Validators.required,Validators.email]],
    //   preferredLocation:[this.user.buyerProfile?.preferredLocation || '',[Validators.required]],
    //   budget:[this.user.buyerProfile?.budget || 0,[Validators.required,Validators.min(0)]]
    // });

    // common password form for both roles
    this.passwordForm = this.fb.group({
      oldPassword: ['', [Validators.required, Validators.minLength(6)]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    },{validator:this.passwordMatchValidator});

    // role specific profile forms
    if(this.isBuyer){
      this.profileForm=this.fb.group({
        phone: [this.user.buyerProfile?.phone||'',[Validators.required]],
        preferredLocation:[this.user.buyerProfile?.preferredLocation||'',[Validators.required]],
        budget:[this.user.buyerProfile?.budget||0,[Validators.required,Validators.min(1)]]
      });
    }else if(this.isAgent){
      this.profileForm=this.fb.group({
        agencyName:[this.user.agentProfile?.agencyName||'',[Validators.required]],
        phone: [this.user.agentProfile?.phone||'',[Validators.required]],
        licenseNumber:[this.user.agentProfile?.licenseNumber||'',[Validators.required]]
      });
    }
  }

  passwordMatchValidator(form: FormGroup) {
    return form.get('newPassword')?.value === form.get('confirmPassword')?.value 
      ? null : { mismatch: true };
  }

  onUpdateProfile():void{
    if (this.profileForm.invalid) {
      this.profileUpdateError = 'Please fill all required fields correctly';
      setTimeout(() => this.profileUpdateError = null, 3000);
      return;
    }

    this.profileUpdateMessage = 'Updating profile...';
    this.profileUpdateError = null;

    // this.updateDto.budget=this.profileForm.value.budget;
    // this.updateDto.preferredLocation=this.profileForm.value.preferredLocation

    // this.buyerService.updateBuyer(this.user.id,this.profileForm.value)
    // .subscribe({
    //   next: (updatedProfile) => {
    //     const updatedUser = {
    //       ...this.user,
    //       buyerProfile: updatedProfile
    //     };

    //     this.authService.setCurrentUser(updatedUser);
    //     // this.user.buyerProfile=updatedUser;
    //     // this.authService.currentUserSubject.value.buyerProfile=updatedUser;
    //     this.profileUpdateMessage = 'Profile updated successfully';
    //     this.isProfileEditing = false;
    //     setTimeout(() => this.profileUpdateMessage = null, 3000);
    //   },
    //   error:()=> {
    //     this.profileUpdateError = 'Failed to update profile';
    //     setTimeout(() => this.profileUpdateError = null, 3000);
    //   }
    // })
    if(this.isBuyer){
      const updateData:UpdateBuyerDto={
        phone:this.profileForm.value.phone,
        preferredLocation:this.profileForm.value.preferredLocation,
        budget:this.profileForm.value.budget
      };
      this.buyerService.updateBuyer(this.user.id,updateData)
        .subscribe({
          next:(updatedProfile)=>{
            this.handleProfileUpdateSuccess(updatedProfile);
          },
          error:()=>{
            this.handleProfileUpdateError();
          }
        });
    }else if(this.isAgent){
      const updateData:UpdateAgentDto={
        agencyName:this.profileForm.value.agencyName,
        phone:this.profileForm.value.phone,
        licenseNumber:this.profileForm.value.licenseNumber
      };

      this.agentService.updateAgent(this.user.id,updateData)
        .subscribe({
          next:(updatedProfile)=>{
            this.handleProfileUpdateSuccess(updatedProfile);
          },
          error:()=>{
            this.handleProfileUpdateError();
          }
        });
    }
  }

  private handleProfileUpdateSuccess(updatedProfile:any){
    const updatedUser={
      ...this.user,
      ...(this.isBuyer ? {buyerProfile:updatedProfile} : {}),
      ...(this.isAgent ? {agentProfile:updatedProfile} : {})
    };
    this.authService.setCurrentUser(updatedUser);
    this.profileUpdateMessage = 'Profile updated successfully';
    this.isProfileEditing=false;
    setTimeout(() => this.profileUpdateMessage = null, 3000);
  }

  private handleProfileUpdateError() {
    this.profileUpdateError = 'Failed to update profile';
    setTimeout(() => this.profileUpdateError = null, 3000);
  }

  onChangePassword():void{
    if (this.passwordForm.invalid) {
      this.passwordUpdateError = 'Please fill all password fields correctly';
      setTimeout(() => this.passwordUpdateError = null, 3000);
      return;
    }

    this.passwordUpdateMessage = 'Changing password...';
    this.passwordUpdateError = null;

    const { oldPassword, newPassword } = this.passwordForm.value;

    this.userService.changePassword(this.user.id,{ oldPassword, newPassword })
    .subscribe({
       next: (res) => {
        this.passwordUpdateMessage = 'Password changed successfully';
        // this.passwordUpdateMessage = res.message;
        this.passwordForm.reset();
        this.isPasswordEditing = false;
        setTimeout(() => this.passwordUpdateMessage = null, 3000);
      },
      error: () => {
        this.passwordUpdateError = 'Failed to change password. Please check your old password.';
        this.passwordForm.reset();
        this.isPasswordEditing = false;
        setTimeout(() => this.passwordUpdateError = null, 3000);
      }
    });
  }

    toggleProfileEdit(): void {
    this.isProfileEditing = !this.isProfileEditing;
    if (this.isProfileEditing) {
      // this.profileForm.patchValue({
      //   preferredLocation: this.user.buyerProfile?.preferredLocation,
      //   budget: this.user.buyerProfile?.budget
      // });
      if(this.isBuyer){
        this.profileForm.patchValue({
          phone:this.user.buyerProfile?.phone,
          preferredLocation:this.user.buyerProfile?.preferredLocation,
          budget: this.user.buyerProfile?.budget
        });
      }else if(this.isAgent){
        this.profileForm.patchValue({
          agencyName:this.user.agentProfile?.agencyName,
          phone:this.user.agentProfile?.phone,
          licenseNumber:this.user.agentProfile?.licenseNumber
        });
      }
    }
  }

  togglePasswordEdit(): void {
    this.isPasswordEditing = !this.isPasswordEditing;
    if (this.isPasswordEditing) {
      this.passwordForm.reset();
    }
  }

  // get formattedPrice(): string {
//     return new Intl.NumberFormat('en-IN', {
//       style: 'currency',
//       currency: 'INR',
//       maximumFractionDigits: 0
//     }).format(this.listing.price);
//   }

}
