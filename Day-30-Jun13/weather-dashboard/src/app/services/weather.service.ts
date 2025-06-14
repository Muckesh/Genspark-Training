import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, catchError, map, Observable, of, switchMap, throwError, timer, using } from "rxjs";
import { WeatherData } from "../models/weather";
import { Injectable } from "@angular/core";

@Injectable({
    providedIn:'root'
})
export class WeatherService {
    // private apiKey = "edc468346244a90f5cb58a1af608a807";
    // private apiUrl = "https://api.openweathermap.org/data/2.5/weather";

    private apiKey = "";
    private apiUrl = "";

    private citySubject = new BehaviorSubject<string>('Chennai');

    public weatherData$ = this.citySubject.asObservable().pipe(
        switchMap(city=>
            timer(0,3000).pipe(
                switchMap(()=>this.fetchWeather(city))
            )
        )
    );

    constructor(private http:HttpClient){}

    updateCity(city:string){
        this.citySubject.next(city);
    }

    fetchWeather(city:string):Observable<any>{
        const url = `${this.apiUrl}?q=${city}&appid=${this.apiKey}&units=metric`;
        return this.http.get(url).pipe(
            catchError(error=>{
                return throwError(()=>new Error(error.error.message || 'City not found.'));
            })
        );

    }

    
    
}