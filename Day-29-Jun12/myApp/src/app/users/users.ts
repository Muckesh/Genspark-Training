import { Component, OnInit } from '@angular/core';
import { UserService } from '../services/user.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-users',
  imports: [CommonModule],
  templateUrl: './users.html',
  styleUrl: './users.css'
})
export class Users implements  OnInit {
  usersCallback: any[]=[];
  usersPromise: any[]=[];
  usersAsync:any[]=[];

  constructor(private userService:UserService){}

  ngOnInit(): void {
    // callback
    this.userService.getUsersCallback(users=>{
      this.usersCallback=users;
    });

    // promise
    this.userService.getUsersPromise().then(users=>{
      this.usersPromise=users;
    });

    // async/await
    this.loadUsersAsync();
  }
  async loadUsersAsync(){
    this.usersAsync = await this.userService.getUsersAsync();
  }
}
