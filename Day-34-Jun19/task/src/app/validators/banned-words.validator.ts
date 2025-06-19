import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";


export function bannedWordsValidator():ValidatorFn{
    const bannedWords = ['admin','root'];
    return (control:AbstractControl):ValidationErrors|null=>{
        if(!control.value) return null;
        const hasBanned = bannedWords.some(word=>
            control.value.toLowerCase().includes(word.toLowerCase())
        );
        return hasBanned ? {bannedWord:true}:null;
    };
}