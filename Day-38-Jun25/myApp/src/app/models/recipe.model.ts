export interface Recipe{
    id: number;
    name: string;
    cuisine: string;
    cookTimeMinutes: number;
    ingredients: string[];
    instructions: string[];
}