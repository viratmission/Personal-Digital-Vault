import {
  Component,
  OnDestroy,
  OnInit,
  inject
} from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { Router } from '@angular/router';

import {
  Subject,
  finalize,
  takeUntil
} from 'rxjs';

import { ProfileService } from '../../../core/services/profile.service';
import { AuthService } from '../../../core/services/auth.service';
import { UpdateProfileRequest } from '../../../core/models/profile.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit, OnDestroy {

  private readonly fb = inject(FormBuilder);
  private readonly profileService = inject(ProfileService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  private readonly destroy$ = new Subject<void>();

  isLoading = true;
  isSaving = false;

  errorMessage = '';
  successMessage = '';

  profileForm = this.fb.nonNullable.group({
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
    ]
  });


  ngOnInit(): void {
    this.loadProfile();
  }


  loadProfile(): void {
    this.errorMessage = '';
    this.isLoading = true;

    this.profileService
      .getProfile()
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe({
        next: profile => {
          this.profileForm.patchValue({
            fullName: profile.fullName,
            email: profile.email
          });
        },

        error: error => {
          if (
            typeof error.error === 'string' &&
            error.error.trim().length > 0
          ) {
            this.errorMessage = error.error;
          } else {
            this.errorMessage =
              'Unable to load profile. Please try again.';
          }
        }
      });
  }


  saveProfile(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    const formValue = this.profileForm.getRawValue();

    const request: UpdateProfileRequest = {
      fullName: formValue.fullName.trim(),
      email: formValue.email.trim()
    };

    this.isSaving = true;

    this.profileService
      .updateProfile(request)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.isSaving = false;
        })
      )
      .subscribe({
        next: response => {
          this.successMessage = response;
        },

        error: error => {
          if (
            typeof error.error === 'string' &&
            error.error.trim().length > 0
          ) {
            this.errorMessage = error.error;
          } else {
            this.errorMessage =
              'Unable to update profile. Please try again.';
          }
        }
      });
  }


  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }


  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}