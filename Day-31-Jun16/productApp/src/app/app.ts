import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Products } from './products/products';
import { CommonModule } from '@angular/common';
import { Menu } from './menu/menu';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,Menu,CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'productApp';
}
