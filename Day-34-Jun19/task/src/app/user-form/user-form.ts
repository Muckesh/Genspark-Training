import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../services/user.service';
import { bannedWordsValidator } from '../validators/banned-words.validator';
import { passwordStrengthValidator } from '../validators/password-strength.validator';
import { confirmPasswordValidator } from '../validators/confirm-password.validator';
import { User } from '../models/user.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-form',
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './user-form.html',
  styleUrl: './user-form.css'
})
export class UserForm implements OnInit {

  constructor(private fb:FormBuilder,private userService:UserService){}

  userForm!:FormGroup;
  roles=['Admin','User','Guest'];

  ngOnInit(): void {
    this.userForm = this.fb.group({
    username: ['', [Validators.required, bannedWordsValidator()]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6), passwordStrengthValidator]],
    confirmPassword: ['', Validators.required],
    role: ['User', Validators.required]
  }, {
    validators: confirmPasswordValidator 
  });

  }

  submitForm(){
    if(this.userForm.valid){
      const {username,email,password,role} = this.userForm.value;
      const newUser = new User(username,email,password,role);
      this.userService.addUser(newUser);
      this.userForm.reset({role:'User'});
    }
  }

}
