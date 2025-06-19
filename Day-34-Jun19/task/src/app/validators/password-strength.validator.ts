import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export const passwordStrengthValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const value: string = control.value;
  if (!value) return null;

  let hasNumber = false;
  let hasSymbol = false;

  const symbols = '!@#$%^&*(),.?":{}|<>';

  for (let i = 0; i < value.length; i++) {
    const char = value[i];

    if (!hasNumber && char >= '0' && char <= '9') {
      hasNumber = true;
    }

    if (!hasSymbol && symbols.includes(char)) {
      hasSymbol = true;
    }

    if (hasNumber && hasSymbol) {
      break;
    }
  }

  const valid = hasNumber && hasSymbol;
  return !valid ? { weakPassword: true } : null;
};
