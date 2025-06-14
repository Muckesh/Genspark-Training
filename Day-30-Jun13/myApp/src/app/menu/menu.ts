import { Component } from '@angular/core';
import { LoginService } from '../services/login.service';

@Component({
  selector: 'app-menu',
  imports: [],
  templateUrl: './menu.html',
  styleUrl: './menu.css'
})
export class Menu {
  username$:any;
  username:string|null="";

  constructor(private loginService:LoginService){
    this.loginService.username$.subscribe(
      {
        next:(value)=>{
          this.username=value;
        },
        error:(err)=>{
          alert(err);
        }
      }
    );
  }
}
