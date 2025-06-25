import { TestBed } from '@angular/core/testing';
import { App } from './app';
import { RecipeList } from './recipe-list/recipe-list';
import { RecipeService } from './services/recipe.service';
import { of } from 'rxjs';

const recipeServiceSpy = jasmine.createSpyObj('RecipeService', ['getAll']);
recipeServiceSpy.getAll.and.returnValue(of({ recipes: [] }));


describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App,RecipeList],
      providers:[{provide:RecipeService,useValue:recipeServiceSpy}]
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render title', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Hello, myApp');
  });
});
