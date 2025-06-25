import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecipeList } from './recipe-list';
import { Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { RecipeService } from '../services/recipe.service';
import { Recipe } from '../models/recipe.model';
import { of, throwError } from 'rxjs';

@Component({
  selector:'app-recipe-card',
  template:''
})
class MockRecipeComponent{}

describe('RecipeList', () => {
  let component: RecipeList;
  let fixture: ComponentFixture<RecipeList>;
  let recipeServiceSpy: jasmine.SpyObj<RecipeService>;

  const mockRecipes: Recipe[] = [
    {
      id: 1,
      name: 'Pasta',
      cuisine: 'Italian',
      cookTimeMinutes: 30,
      ingredients: ['Pasta', 'Tomato Sauce'],
      instructions: ['Boil pasta', 'Add sauce']
    },
    {
      id: 2,
      name: 'Tacos',
      cuisine: 'Mexican',
      cookTimeMinutes: 20,
      ingredients: ['Tortilla', 'Beef'],
      instructions: ['Cook beef', 'Fill tortilla']
    }
  ];

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('RecipeService',['getAll'])


    await TestBed.configureTestingModule({
      imports: [RecipeList],
      providers:[{provide:RecipeService,useValue:spy}],
      schemas:[CUSTOM_ELEMENTS_SCHEMA]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RecipeList);
    component = fixture.componentInstance;
    recipeServiceSpy= TestBed.inject(RecipeService) as jasmine.SpyObj<RecipeService>;
    // fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load recipes on init',()=>{
    recipeServiceSpy.getAll.and.returnValue(of({recipes:mockRecipes}));
    fixture.detectChanges(); //trigger ngOnInit

    expect(component.recipes.length).toBe(2);
    expect(component.loading).toBeFalse();
    expect(component.error).toBeNull();
  });

  it('should show error on service failure', () => {
    recipeServiceSpy.getAll.and.returnValue(throwError(() => new Error('Failed to load')));

    fixture.detectChanges(); 

    expect(component.recipes.length).toBe(0);
    expect(component.loading).toBeFalse();
    expect(component.error).toBe('Failed to load');
  });

  it('should handle empty recipes', () => {
    recipeServiceSpy.getAll.and.returnValue(of({ recipes: [] }));

    fixture.detectChanges(); 

    expect(component.recipes.length).toBe(0);
    expect(component.error).toBeNull();
  });
  
});
