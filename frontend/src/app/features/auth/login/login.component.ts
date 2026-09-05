import { Component, OnDestroy, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import {
  Subject,
  finalize,
  takeUntil
} from 'rxjs';

import { AuthService } from '../../../core/services/auth.service';
import { LoginRequest } from '../../../core/models/auth.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnDestroy {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  private readonly destroy$ = new Subject<void>();

  email = '';
  password = '';

  isSubmitting = false;
  errorMessage = '';
  showPassword = false;
  submit(): void {
    this.errorMessage = '';

    const request: LoginRequest = {
      email: this.email.trim(),
      password: this.password
    };

    this.isSubmitting = true;

    this.authService
      .login(request)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.isSubmitting = false;
        })
      )
      .subscribe({
        next: () => {
          this.router.navigate(['/vault']);
        },

        error: error => {
          if (
            typeof error.error === 'string' &&
            error.error.trim().length > 0
          ) {
            this.errorMessage = error.error;
          } else {
            this.errorMessage =
              'Login failed. Please try again.';
          }
        }
      });
  }
  togglePasswordVisibility(): void {
  this.showPassword = !this.showPassword;
}

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}