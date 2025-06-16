import { Component } from '@angular/core';
import { Product } from '../product/product';
import { Products } from '../products/products';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-menu',
  imports: [RouterLink],
  templateUrl: './menu.html',
  styleUrl: './menu.css'
})
export class Menu {
  
}
