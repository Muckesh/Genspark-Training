import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";

@Injectable()
export class RecipeService {
    private http = inject(HttpClient);

    getAllRecipes(){
        return this.http.get<any[]>("https://dummyjson.com/recipes");
    }
}