import { Component, OnInit } from '@angular/core';
import { UserService } from '../services/user.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserModel } from '../models/user.model';

@Component({
  selector: 'app-users',
  imports: [CommonModule,FormsModule],
  templateUrl: './users.html',
  styleUrl: './users.css'
})
export class Users implements OnInit{
  users:UserModel[]=[];
  filteredUsers:UserModel[]=[];
  newUser: UserModel = new UserModel();


  genderFilter="";
  roleFilter="";
  stateFilter="";

  constructor(private userService:UserService){}

  ngOnInit(): void {
    this.userService.getUsers().subscribe((res:any)=>{
      this.users=res.users.map((user:any)=> new UserModel(
        user.firstName,
        user.lastName,
        user.age,
        user.gender,
        user.company,
        user.address
      ));
      this.filteredUsers=[...this.users];
    });
  }

  addUser(){
    // const newUser = new UserModel(
    //   'Muhammad',
    //   'Ovi',
    //   250,
    //   'male',
    //   { title: 'Developer' },
    //   { state: 'Tamil Nadu' }
    // );

    this.userService.addUser(this.newUser).subscribe((res: any) => {
      const addedUser = new UserModel(
        res.firstName,
        res.lastName,
        res.age,
        res.gender,
        res.company,
        res.address
      );
      this.users.push(addedUser);
      this.filterUsers();  
      this.newUser=new UserModel();
      console.log("User Added:", addedUser);
    });
  }


  filterUsers(){
    this.filteredUsers=this.users.filter((user:UserModel)=>{
      return (
        (!this.genderFilter || user.gender===this.genderFilter) &&
        (!this.roleFilter || user.company?.title?.toLowerCase().includes(this.roleFilter.toLowerCase())) &&
        (!this.stateFilter || user.address?.state?.toLowerCase().includes(this.stateFilter.toLowerCase()))
      );
    });
  }


}
