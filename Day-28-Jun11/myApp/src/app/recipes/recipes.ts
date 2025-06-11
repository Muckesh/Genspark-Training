import { Component, OnInit, signal } from '@angular/core';
import { Recipe } from '../recipe/recipe';
import { RecipeModel } from '../models/recipe';
import { RecipeService } from '../services/recipe.service';

@Component({
  selector: 'app-recipes',
  imports: [Recipe],
  templateUrl: './recipes.html',
  styleUrl: './recipes.css'
})
export class Recipes implements OnInit {
  // recipes:RecipeModel[]|undefined=undefined;
  recipes = signal<RecipeModel[]>([]);

  constructor(private recipeService:RecipeService){}
  ngOnInit(): void {
    this.recipeService.getAllRecipes().subscribe(
      {
        next:(data:any)=>{
          const mapped = data.recipes.map((r:any)=> new RecipeModel(
            r.id,
            r.image,
            r.name,
            r.cuisine,
            r.cookTimeMinutes,
            r.ingredients
          ));
          this.recipes.set(mapped);
        },
        error:(err)=>{
          console.error('Error fetching recipes : ',err);
        },
        complete:()=>{}
      }
    );
  }
}
