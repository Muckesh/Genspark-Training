import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';
import { UpdateAgentDto } from '../../../models/agent.model';
import { UpdateBuyerDto } from '../../../models/buyer.model';
import { User } from '../../../models/user.model';
import { AgentService } from '../../../services/agent.service';
import { BuyerService } from '../../../services/buyer.service';
import { UserService } from '../../../services/user.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-edit-user',
  imports: [CommonModule,FormsModule,ReactiveFormsModule],
  templateUrl: './admin-edit-user.html',
  styleUrl: './admin-edit-user.css'
})
export class AdminEditUser implements OnInit{
  user!: User;
  profileForm!: FormGroup;
  isBuyer = false;
  isAgent = false;
  isLoading = false;
  isSubmitting = false;
  error: string | null = null;
  successMessage: string | null = null;

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private userService: UserService,
    private buyerService: BuyerService,
    private agentService: AgentService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.isLoading = true;
    const userId = this.route.snapshot.params['id'];

    this.userService.getAllUsers(userId).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (users) => {

        this.user = users.items.filter(u=>u.id===userId)[0];
        console.log(this.user)
        this.isBuyer = this.user.role === 'Buyer';
        this.isAgent = this.user.role === 'Agent';
        this.initializeForm();
      },
      error: (err) => {
        this.error = 'Failed to load user details. Please try again.';
        console.error('Error loading user:', err);
      }
    });
  }

  initializeForm(): void {
   

    if (this.isBuyer) {
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

  onSubmit(): void {
    if (this.profileForm.invalid) return;

    this.isSubmitting = true;
    this.error = null;
    this.successMessage = null;

    

    if(this.isBuyer){
      this.updateBuyerProfile();
    }
    else if(this.isAgent){
      this.updateAgentProfile();
    }
  }

  private updateBuyerProfile(): void {
    const buyerData: UpdateBuyerDto = {
      phone: this.profileForm.value.phone,
      preferredLocation: this.profileForm.value.preferredLocation,
      budget: this.profileForm.value.budget
    };

    this.buyerService.updateBuyer(this.user.id, buyerData).subscribe({
      next: () => this.onUpdateSuccess(),
      error: (err) => this.handleError('Failed to update buyer profile', err)
    });
  }

  private updateAgentProfile(): void {
    const agentData: UpdateAgentDto = {
      agencyName: this.profileForm.value.agencyName,
      licenseNumber: this.profileForm.value.licenseNumber,
      phone: this.profileForm.value.phone
    };

    this.agentService.updateAgent(this.user.id, agentData).subscribe({
      next: () => this.onUpdateSuccess(),
      error: (err) => this.handleError('Failed to update agent profile', err)
    });
  }

  private onUpdateSuccess(): void {
    this.successMessage = 'User updated successfully!';
    this.isSubmitting = false;
    setTimeout(() => {
      this.router.navigate(['/admin/users']);
    }, 1500);
  }

  private handleError(message: string, error: any): void {
    this.error = message;
    this.isSubmitting = false;
    console.error(error);
  }
}
