import { Component } from '@angular/core';
import { First } from "./first/first";
import { CustomerDetails } from "./customer-details/customer-details";
import { Products } from "./products/products";

@Component({
  selector: 'app-root',
  imports: [First, CustomerDetails, Products],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'myApp';
}
