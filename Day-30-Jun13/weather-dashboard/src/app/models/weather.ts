// export interface WeatherModel{
//     city:string;
//     temperature:number;
//     condition:string;
//     icon:string;
//     humidity:number;
//     windspeed:number;
// }

export interface WeatherData {
  name: string;
  weather: { description: string; icon: string }[];
  main: { temp: number; humidity: number };
  wind: { speed: number };
}
