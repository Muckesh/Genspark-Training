import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-topmenu',
  imports: [RouterLink,CommonModule],
  templateUrl: './topmenu.html',
  styleUrl: './topmenu.css'
})
export class Topmenu implements OnInit {
  isAdmin=false;
  private authService = inject(AuthService)
  ngOnInit(): void {
    this.isAdmin = this.authService.hasRole("Admin");
  }
  logout(){
    this.authService.logout();
  }
}
