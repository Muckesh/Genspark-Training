import { Component, OnInit } from '@angular/core';
import { User } from '../models/user';
import { AuthService } from '../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard-login',
  imports: [CommonModule],
  templateUrl: './dashboard-login.html',
  styleUrl: './dashboard-login.css'
})
export class DashboardLogin implements OnInit {
  user:User|null=null;
  
  constructor(private authService:AuthService){}

  ngOnInit():void{
    this.user = this.authService.getUser(true) || this.authService.getUser(false); // user from session storage or local storage
  }
}
