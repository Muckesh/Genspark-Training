export class User{
    constructor(
        public username:string,
        public email:string,
        public password:string,
        public role:'Admin' | 'User' | 'Guest'
    ){}
}