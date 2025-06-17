import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UserService } from '../services/user.service';
import { UserLoginModel } from '../models/UserLoginModel';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  user:UserLoginModel=new UserLoginModel();
  constructor(private userService:UserService, private route:Router){

  }
  handleLogin(){
    this.userService.validateUserLogin(this.user);
    this.route.navigateByUrl("/home/"+this.user.username);
  }
}
