import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export const confirmPasswordValidator:ValidatorFn=(group:AbstractControl):ValidationErrors|null=>{
    const password = group.get('password')?.value;
    const confim = group.get('confirmPassword')?.value;

    return password ==confim ? null:{passwordMismatch:true};
}