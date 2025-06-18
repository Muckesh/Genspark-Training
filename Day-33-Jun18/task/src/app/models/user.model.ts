export class UserModel{
    constructor(
        public firstName: string = '',
        public lastName: string = '',
        public age: number = 0,
        public gender: string = '',
        public company: { title: string } = { title: '' },
        public address: { state: string } = { state: '' }
    ){}
}