import { Component } from '@angular/core';
import { Products } from "./products/products";
import { Menu } from "./menu/menu";
import { Login } from "./login/login";
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [Products, Menu, Login,RouterModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'myApp';
}

/*

Create a login component – username and password- 
Add respective model and service (Dummy – just to get and validate against a dummy array)

Store the object in local storage and retrieve from local storage in another component.
Change from local to session 

Note the functional difference

Remember to serialize the object
--------------------------------------------
I have to implement this in angular.


*/