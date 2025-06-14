// // src/app/services/user.service.ts
// import { Injectable } from '@angular/core';
// import { User } from '../models/user';

// @Injectable({
//   providedIn: 'root'
// })
// export class UserService {
//   private dummyUsers: User[] = [
//     new User('admin', '1234'),
//     new User('test', 'abcd'),
//   ];

//   login(user: User): boolean {
//     const found = this.dummyUsers.find(
//       u => u.username === user.username && u.password === user.password
//     );

//     if (found) {
//       // Store in sessionStorage (change to localStorage if needed)
//       sessionStorage.setItem('loggedInUser', JSON.stringify(found));
//       return true;
//     }

//     return false;
//   }

//   getLoggedInUser(): User | null {
//     const userString = sessionStorage.getItem('loggedInUser');
//     return userString ? JSON.parse(userString) : null;
//   }

//   logout() {
//     sessionStorage.removeItem('loggedInUser');
//   }
// }
