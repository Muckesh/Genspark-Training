import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecipeCard } from './recipe-card';
import { Recipe } from '../models/recipe.model';
import { By } from '@angular/platform-browser';

describe('RecipeCard', () => {
  let component: RecipeCard;
  let fixture: ComponentFixture<RecipeCard>;

  const mockRecipe:Recipe={
    id:1,
    name: 'Spaghetti Bolognese',
    cuisine: 'Italian',
    cookTimeMinutes: 45,
    ingredients: ['Spaghetti', 'Ground Beef', 'Tomato Sauce'],
    instructions: ['Boil pasta', 'Cook meat', 'Mix with sauce']
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecipeCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RecipeCard);
    component = fixture.componentInstance;
    component.recipe = mockRecipe;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display recipe name and cuisine',()=>{
    const titleEl = fixture.nativeElement.querySelector('.recipe-title');
    const categoryEl = fixture.nativeElement.querySelector('.recipe-category');
    
    expect(titleEl.textContent).toContain('Spaghetti Bolognese');
    expect(categoryEl.textContent).toContain('Italian');
  });

  it('should display cook time', () => {
    const descEl = fixture.nativeElement.querySelector('.recipe-description');
    expect(descEl.textContent).toContain('45');
  });

  it('should list all ingredients', () => {
    const ingredients = fixture.debugElement.queryAll(By.css('ul li'));
    expect(ingredients.length).toBe(3);
    expect(ingredients.map(el => el.nativeElement.textContent.trim())).toEqual(mockRecipe.ingredients);
  });

  it('should list all instructions', () => {
    const steps = fixture.debugElement.queryAll(By.css('ol li'));
    expect(steps.length).toBe(3);
    expect(steps.map(el => el.nativeElement.textContent.trim())).toEqual(mockRecipe.instructions);
  });
});
