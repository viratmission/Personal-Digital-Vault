import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, finalize, takeUntil } from 'rxjs';
import { ProfileService } from '../../../core/services/profile.service';
import { AuthService } from '../../../core/services/auth.service';
import { UpdateProfileRequest } from '../../../core/models/profile.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit, OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly profileService = inject(ProfileService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroy$ = new Subject<void>();
  private profilePhotoStorageKey = 'pdv_profile_photo';

  isLoading = true;
  isSaving = false;
  errorMessage = '';
  successMessage = '';
  profilePhoto = '';

  profileForm = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.pattern(/^[^\s@]+@(gmail|hotmail)\.com$/i)]]
  });

  ngOnInit(): void {
    this.profilePhoto = localStorage.getItem(this.profilePhotoStorageKey) ?? '';
    this.loadProfile();
  }

  loadProfile(): void {
    this.errorMessage = '';
    this.isLoading = true;
    this.profileService.getProfile().pipe(takeUntil(this.destroy$), finalize(() => this.isLoading = false)).subscribe({
      next: profile => {
        this.profileForm.patchValue({ fullName: profile.fullName, email: profile.email });
        this.profilePhotoStorageKey = `pdv_profile_photo_${profile.id}`;
        this.profilePhoto = localStorage.getItem(this.profilePhotoStorageKey) ?? this.profilePhoto;
      },
      error: error => {
        this.errorMessage = typeof error.error === 'string' && error.error.trim() ? error.error : 'Unable to load profile. Please try again.';
      }
    });
  }

  onPhotoSelected(event: Event): void {
    this.errorMessage = '';
    this.successMessage = '';
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    const allowedTypes = ['image/jpeg', 'image/png', 'image/webp'];
    if (!allowedTypes.includes(file.type)) {
      this.errorMessage = 'Please choose a JPG, PNG or WebP image.';
      input.value = '';
      return;
    }

    if (file.size > 2 * 1024 * 1024) {
      this.errorMessage = 'Profile photo must be 2 MB or smaller.';
      input.value = '';
      return;
    }

    const reader = new FileReader();
    reader.onload = () => {
      this.profilePhoto = String(reader.result);
      localStorage.setItem(this.profilePhotoStorageKey, this.profilePhoto);
      window.dispatchEvent(new Event('pdv-profile-photo-changed'));
      this.successMessage = 'Profile photo updated successfully.';
      input.value = '';
    };
    reader.onerror = () => {
      this.errorMessage = 'Unable to read the selected image.';
      input.value = '';
    };
    reader.readAsDataURL(file);
  }

  removePhoto(): void {
    this.profilePhoto = '';
    localStorage.removeItem(this.profilePhotoStorageKey);
    window.dispatchEvent(new Event('pdv-profile-photo-changed'));
    this.successMessage = 'Profile photo removed.';
    this.errorMessage = '';
  }

  get initials(): string {
    const name = this.profileForm.controls.fullName.value.trim();
    if (!name) return 'U';
    return name.split(/\s+/).slice(0, 2).map(part => part[0]).join('').toUpperCase();
  }

  saveProfile(): void {
    this.errorMessage = '';
    this.successMessage = '';
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }
    const value = this.profileForm.getRawValue();
    const request: UpdateProfileRequest = { fullName: value.fullName.trim(), email: value.email.trim() };
    this.isSaving = true;
    this.profileService.updateProfile(request).pipe(takeUntil(this.destroy$), finalize(() => this.isSaving = false)).subscribe({
      next: response => {
        this.successMessage = response;
        window.dispatchEvent(new Event('pdv-profile-changed'));
      },
      error: error => this.errorMessage = typeof error.error === 'string' && error.error.trim() ? error.error : 'Unable to update profile. Please try again.'
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
