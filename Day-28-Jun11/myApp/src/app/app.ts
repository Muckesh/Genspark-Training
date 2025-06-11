import { Component } from '@angular/core';
// import { Products } from "./products/products";
import { Recipes } from "./recipes/recipes";

@Component({
  selector: 'app-root',
  imports: [Recipes],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'myApp';
}
