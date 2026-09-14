import {
  AbstractControl,
  ValidationErrors,
  ValidatorFn
} from '@angular/forms';

export const passwordStrengthValidator: ValidatorFn =
  (control: AbstractControl): ValidationErrors | null => {

    const value = control.value as string;

    if (!value) {
      return null;
    }

    const hasUpperCase = /[A-Z]/.test(value);
    const hasLowerCase = /[a-z]/.test(value);
    const hasNumber = /\d/.test(value);
    const hasSpecialCharacter = /[^A-Za-z0-9]/.test(value);
    const hasMinimumLength = value.length >= 8;

    const isStrong =
      hasUpperCase &&
      hasLowerCase &&
      hasNumber &&
      hasSpecialCharacter &&
      hasMinimumLength;

    return isStrong
      ? null
      : { passwordStrength: true };
  };


export const passwordMatchValidator: ValidatorFn =
  (control: AbstractControl): ValidationErrors | null => {

    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;

    if (!password || !confirmPassword) {
      return null;
    }

    return password === confirmPassword
      ? null
      : { passwordMismatch: true };
  };