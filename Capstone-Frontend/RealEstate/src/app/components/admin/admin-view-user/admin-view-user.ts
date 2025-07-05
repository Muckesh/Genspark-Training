import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { User } from '../../../models/user.model';
import { Agent } from '../../../models/agent.model';
import { Buyer } from '../../../models/buyer.model';
import { ActivatedRoute, Router, RouterLink, RouterModule } from '@angular/router';
import { FormBuilder, FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { AgentService } from '../../../services/agent.service';
import { BuyerService } from '../../../services/buyer.service';
import { UserService } from '../../../services/user.service';

@Component({
  selector: 'app-admin-view-user',
  imports: [CommonModule,RouterLink,FormsModule,RouterModule],
  templateUrl: './admin-view-user.html',
  styleUrl: './admin-view-user.css'
})
export class AdminViewUser implements OnInit {

  user!: User;
  isBuyer = false;
  isAgent = false;
  // userDetails: Buyer | Agent | null = null;
  isLoading = false;
  error: string | null = null;

  // ngOnInit(): void {
  //   // This component expects the parent to pass the complete user object
  //   // including either buyerProfile or agentProfile
  //   if (this.user.buyerProfile) {
  //     this.userDetails = this.user.buyerProfile;
  //   } else if (this.user.agentProfile) {
  //     this.userDetails = this.user.agentProfile;
  //   }
  // }

  constructor(
      private route: ActivatedRoute,
      public router: Router,
      private userService: UserService,
      private buyerService: BuyerService,
      private agentService: AgentService,
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
          },
          error: (err) => {
            this.error = 'Failed to load user details. Please try again.';
            console.error('Error loading user:', err);
          }
        });
      }

  getRoleBadgeClass(role: string): string {
    switch (role) {
      case 'Admin': return 'bg-danger';
      case 'Agent': return 'bg-success';
      case 'Buyer': return 'bg-primary';
      default: return 'bg-secondary';
    }
  }

}
