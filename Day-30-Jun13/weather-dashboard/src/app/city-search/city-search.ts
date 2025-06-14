import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { WeatherService } from '../services/weather.service';

@Component({
  selector: 'app-city-search',
  imports: [CommonModule,FormsModule],
  templateUrl: './city-search.html',
  styleUrl: './city-search.css'
})
export class CitySearch {

  city="";
  constructor(private weatherService:WeatherService){}

  search(){
    if(this.city.trim()){
      this.weatherService.updateCity(this.city.trim());
      this.city="";
    }
  }
}
