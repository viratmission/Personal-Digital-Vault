import { Component, OnDestroy, inject } from '@angular/core';
import { FormBuilder,ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  Subject,
  finalize,
  takeUntil
} from 'rxjs';

import { AuthService } from '../../../core/services/auth.service';
import { RegisterRequest } from '../../../core/models/auth.model';
import {
  passwordMatchValidator,
  passwordStrengthValidator
} from './register.validators';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [  ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);

  private readonly destroy$ = new Subject<void>();

  isSubmitting = false;
  errorMessage = '';
  successMessage = '';
  showPassword = false;
  showConfirmPassword = false;

  registerForm = this.fb.nonNullable.group(
    {
      fullName: [
        '',
        [
          Validators.required,
          Validators.maxLength(100)
        ]
      ],

      email: [
        '',
        [
          Validators.required,
          Validators.email
        ]
      ],

      password: [
        '',
        [
          Validators.required,
          passwordStrengthValidator
        ]
      ],

      confirmPassword: [
        '',
        [
          Validators.required
        ]
      ]
    },
    {
      validators: passwordMatchValidator
    }
  );

  submit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const formValue = this.registerForm.getRawValue();

    const request: RegisterRequest = {
      fullName: formValue.fullName.trim(),
      email: formValue.email.trim(),
      password: formValue.password
    };

    this.isSubmitting = true;

    this.authService
      .register(request)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.isSubmitting = false;
        })
      )
      .subscribe({
        next: response => {
          this.successMessage = response;
          this.registerForm.reset();
        },

        error: error => {
          if (
            typeof error.error === 'string' &&
            error.error.trim().length > 0
          ) {
            this.errorMessage = error.error;
          } else {
            this.errorMessage =
              'Registration failed. Please try again.';
          }
        }
      });
  }

  togglePasswordVisibility(): void {
  this.showPassword = !this.showPassword;
  }

  toggleConfirmPasswordVisibility(): void {
  this.showConfirmPassword = !this.showConfirmPassword;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}