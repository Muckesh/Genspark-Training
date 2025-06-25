import { Component, OnInit } from '@angular/core';
import { Recipe } from '../models/recipe.model';
import { RecipeService } from '../services/recipe.service';
import { CommonModule } from '@angular/common';
import { RecipeCard } from '../recipe-card/recipe-card';

@Component({
  selector: 'app-recipe-list',
  imports: [CommonModule,RecipeCard],
  templateUrl: './recipe-list.html',
  styleUrl: './recipe-list.css'
})
export class RecipeList implements OnInit{
  recipes:Recipe[]=[];
  loading=false;
  error:string|null=null;

  constructor(private service:RecipeService){}

  ngOnInit(): void {
    this.loading=true;
    this.service.getAll().subscribe({
      next:res=>{
        this.recipes=res.recipes;
        this.loading=false;
      },
      error:err=>{
        this.error=err.message||'Error fetching recipes';
        this.loading=false;
      }
    });
  }
}
