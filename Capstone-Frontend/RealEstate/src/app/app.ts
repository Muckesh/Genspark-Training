import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoginForm } from './auth/components/login-form/login-form';
import { Header } from './header/header';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,Header,CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'RealEstate';
}
