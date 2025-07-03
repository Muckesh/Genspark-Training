import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [CommonModule,RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  constructor(private authService:AuthService){}

  isAuthenticated(){
    return this.authService.isAuthenticated();
  }

  getCurrentUser(){
    return this.authService.getCurrentUser();
  }

  hasRole(role:string){
    return this.authService.hasRole(role);
  }

  logout(){
    this.authService.logout();
  }
}
