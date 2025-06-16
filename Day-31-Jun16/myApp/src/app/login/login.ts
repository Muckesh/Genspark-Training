// // import { Component } from '@angular/core';
// // import { FormsModule } from '@angular/forms';
// // import { LoginModel } from '../models/login';
// // import { LoginService } from '../services/login.service';

// // @Component({
// //   selector: 'app-login',
// //   imports: [FormsModule],
// //   templateUrl: './login.html',
// //   styleUrl: './login.css'
// // })
// // export class Login {
// //   user:LoginModel=new LoginModel();
// //   constructor(private loginService:LoginService){

// //   }
// //   handleLogin(){
// //     this.loginService.validateUserLogin(this.user);
// //   }
// // }
// import { Component } from '@angular/core';
// import { User } from '../models/user';
// // import { UserService } from '../services/user.service';
// import { Router } from '@angular/router';
// import { FormsModule } from '@angular/forms';
// // import { AuthService } from '../services/auth.service';

// @Component({
//   selector: 'app-login',
//   imports: [FormsModule],
//   templateUrl: './login.html',
//   styleUrls: ['./login.css']
// })
// export class Login {
//   username="";
//   password="";
//   useSession = false;

//   constructor(private authService:AuthService,private router:Router){}

//   login():void{
//     const user:User = {username:this.username,password:this.password}
//     if(this.authService.validateUser(user)){
//       this.authService.saveUser(user,this.useSession);
//       this.router.navigate(['/login-dashboard']);
//     }else{
//       alert("Invalid credentials.");
//     }
//   }
// }
