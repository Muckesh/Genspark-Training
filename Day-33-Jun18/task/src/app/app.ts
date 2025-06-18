import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Users } from './users/users';
import { Menu } from './menu/menu';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,Menu],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'task';
}
