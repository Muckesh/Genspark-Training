import { Component, inject, Input } from '@angular/core';
import { RecipeModel } from '../models/recipe';
import { RecipeService } from '../services/recipe.service';

@Component({
  selector: 'app-recipe',
  imports: [],
  templateUrl: './recipe.html',
  styleUrl: './recipe.css'
})
export class Recipe {
@Input() recipe:RecipeModel | null = new RecipeModel();
private recipeService = inject(RecipeService);

constructor(){}
}
