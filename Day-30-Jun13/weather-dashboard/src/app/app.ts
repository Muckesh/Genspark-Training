import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { WeatherDashboard } from './weather-dashboard/weather-dashboard';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [CommonModule,WeatherDashboard],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'weather-dashboard';
}
