import { Component } from '@angular/core';
import { UserLogin } from '../models/userLoginModel';
import { UserService } from '../services/user.service';
import { Router, RouterOutlet } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  imports: [FormsModule,RouterOutlet],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  user:UserLogin = new UserLogin();

  constructor(private userService:UserService,private router:Router){}

  handleLogin(){
    this.userService.validateUserLogin(this.user);
    this.router.navigate(["products"]);
  }
}
