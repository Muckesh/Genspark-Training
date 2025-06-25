import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Recipe } from "../models/recipe.model";

@Injectable()
export class RecipeService{
    private baseUrl = 'https://dummyjson.com/recipes';

    constructor(private http:HttpClient){}

    getAll():Observable<{recipes:Recipe[]}>{
        return this.http.get<{recipes:Recipe[]}>(`${this.baseUrl}`);
    }

    getOne(id:number):Observable<Recipe>{
        return this.http.get<Recipe>(`${this.baseUrl}/${id}`);
    }
}