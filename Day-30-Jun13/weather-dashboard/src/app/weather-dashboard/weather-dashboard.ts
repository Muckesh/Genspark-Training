import { AsyncPipe, CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { CitySearch } from '../city-search/city-search';
import { WeatherCard } from '../weather-card/weather-card';
import { Observable } from 'rxjs';
import { WeatherService } from '../services/weather.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-weather-dashboard',
  imports: [CommonModule,AsyncPipe,WeatherCard,CitySearch,FormsModule],
  templateUrl: './weather-dashboard.html',
  styleUrl: './weather-dashboard.css'
})
export class WeatherDashboard implements OnInit {

  weather$!:Observable<any>;
  errorMsg:string|null=null;

  constructor(private weatherService:WeatherService){}

  ngOnInit(): void {
    this.weather$=this.weatherService.weatherData$;
    this.weather$.subscribe({
      error:err=>(this.errorMsg=err.message)
    });
  }
}
