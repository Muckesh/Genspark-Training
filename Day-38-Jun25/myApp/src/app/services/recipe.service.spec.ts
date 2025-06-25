import { HttpClientTestingModule, HttpTestingController, provideHttpClientTesting } from "@angular/common/http/testing";
import { RecipeService } from "./recipe.service";
import { TestBed } from "@angular/core/testing";
import { HttpClient, provideHttpClient } from "@angular/common/http";
import { Recipe } from "../models/recipe.model";
import { of } from "rxjs";

describe('RecipeService using spy',()=>{
    let service:RecipeService;
    let httpClientSpy: jasmine.SpyObj<HttpClient>;

    beforeEach(()=>{
        // Creating a spy object with 'get' method
        httpClientSpy = jasmine.createSpyObj('HttpClient',['get']);

        // Injecting the service with the mocked HttpClient
        service = new RecipeService(httpClientSpy)
    });

    it('Should retrieve all recipes using spy',()=>{
        const mockRecipes: Recipe[]=[
            {
                id: 1,
                name: 'Taco',
                cuisine: 'Mexican',
                cookTimeMinutes: 20,
                ingredients: ['Tortilla', 'Beef', 'Lettuce'],
                instructions: ['Cook beef', 'Assemble taco']
            }
        ];

        const mockResponse = {recipes:mockRecipes};

        // Set up the return value of HttpClient.get
        httpClientSpy.get.and.returnValue(of(mockResponse));

        service.getAll().subscribe(res=>{
            expect(res.recipes.length).toBe(1);
            expect(res.recipes[0].name).toBe('Taco');
        });

        expect(httpClientSpy.get).toHaveBeenCalledWith('https://dummyjson.com/recipes');

    });

    it('Should retrieve one recipe using spy',()=>{
        const mockRecipes: Recipe=
            {
                id: 1,
                name: 'Taco',
                cuisine: 'Mexican',
                cookTimeMinutes: 20,
                ingredients: ['Tortilla', 'Beef', 'Lettuce'],
                instructions: ['Cook beef', 'Assemble taco']
            };

        const mockResponse = mockRecipes;

        httpClientSpy.get.and.returnValue(of(mockResponse));

        service.getOne(1).subscribe(res=>{
            expect(res.id).toBe(1);
            expect(res.name).toBe('Taco');
        });

        expect(httpClientSpy.get).toHaveBeenCalledWith('https://dummyjson.com/recipes/1');
    });
});

// describe('RecipeService',()=>{
//     let service: RecipeService;
//     let httpMock: HttpTestingController;

//     beforeEach(()=>{
//         TestBed.configureTestingModule({
//             imports:[],
//             providers:[RecipeService,provideHttpClient(),provideHttpClientTesting()]
//         });
//         service = TestBed.inject(RecipeService);
//         httpMock = TestBed.inject(HttpTestingController);
//     });

//     afterEach(()=>{
//         httpMock.verify();
//     });

//     it('Should retrieve recipes from API',()=>{
//         const mockRecipes: Recipe[]=[
//             {
//                 id: 1,
//                 name: 'Taco',
//                 cuisine: 'Mexican',
//                 cookTimeMinutes: 20,
//                 ingredients: ['Tortilla', 'Beef', 'Lettuce'],
//                 instructions: ['Cook beef', 'Assemble taco']
//             }
//         ];

//         const mockResponse = {recipes:mockRecipes};

//         service.getAll().subscribe((res:any)=>{
//             expect(res.recipes.length).toBe(1);
//             expect(res.recipes[0].name).toBe('Taco');
//         });

//         const req = httpMock.expectOne('https://dummyjson.com/recipes');
//         expect(req.request.method).toBe('GET');
//         req.flush(mockResponse);

//     });

//     it('Should retrieve recipe by id',()=>{
//         const mockRecipes: Recipe=
//             {
//                 id: 1,
//                 name: 'Taco',
//                 cuisine: 'Mexican',
//                 cookTimeMinutes: 20,
//                 ingredients: ['Tortilla', 'Beef', 'Lettuce'],
//                 instructions: ['Cook beef', 'Assemble taco']
//             };

//         const mockResponse = mockRecipes;

//         service.getOne(1).subscribe((res:any)=>{
//             expect(res.name).toBe('Taco');
//             expect(res.cuisine).toBe('Mexican');
//         });

//         const req = httpMock.expectOne('https://dummyjson.com/recipes/1');
//         expect(req.request.method).toBe('GET');
//         req.flush(mockResponse);


//     });


// });