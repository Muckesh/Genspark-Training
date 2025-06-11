export class RecipeModel{
    constructor(
        public id:number=0,
        public image:string="",
        public name:string="",
        public cuisine:string="",
        public cookingTime:number=0,
        public ingredients:string[]=[]
    ){}
}