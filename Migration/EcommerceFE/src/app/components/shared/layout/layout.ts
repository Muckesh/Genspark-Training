import { Component } from '@angular/core';
import { Header } from '../header/header';
import { Home } from '../home/home';
import { Sidebar } from '../sidebar/sidebar';
import { RouterOutlet } from '@angular/router';
import { Topmenu } from '../topmenu/topmenu';
import { Footer } from '../footer/footer';

@Component({
  selector: 'app-layout',
  imports: [Header,Footer,Sidebar,RouterOutlet,Topmenu],
  templateUrl: './layout.html',
  styleUrl: './layout.css'
})
export class Layout {

}
