import { Component, ElementRef, HostListener, OnDestroy, OnInit, ViewChild, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ProfileService } from '../../../core/services/profile.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, FormsModule],
  templateUrl: './app-shell.component.html',
  styleUrl: './app-shell.component.css'
})
export class AppShellComponent implements OnInit, OnDestroy {
  private readonly authService = inject(AuthService);
  private readonly profileService = inject(ProfileService);
  private readonly router = inject(Router);
  private readonly destroy$ = new Subject<void>();

  readonly photoStorageKey = 'pdv_profile_photo';
  profilePhoto = '';
  fullName = 'Vault User';
  notificationsOpen = false;
  topbarSearchTerm = '';

  @ViewChild('topbarSearchInput') topbarSearchInput?: ElementRef<HTMLInputElement>;

  navItems = [
    { label: 'Dashboard', icon: '⌂', route: '/dashboard' },
    { label: 'My Vault', icon: '▣', route: '/vault' },
    { label: 'Credentials', icon: '▤', route: '/credentials' },
    { label: 'Search', icon: '⌕', route: '/search' },
    { label: 'Profile', icon: '◯', route: '/profile' }
  ];

  ngOnInit(): void {
    this.loadProfilePhoto();
    this.loadProfileName();
    window.addEventListener('pdv-profile-photo-changed', this.onProfileChanged);
    window.addEventListener('pdv-profile-changed', this.onProfileChanged);
  }

  private readonly onProfileChanged = (): void => {
    this.loadProfilePhoto();
    this.loadProfileName();
  };

  loadProfilePhoto(): void {
    this.profilePhoto = localStorage.getItem(this.photoStorageKey) ?? '';
  }

  loadProfileName(): void {
    this.profileService.getProfile()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: profile => {
          this.fullName = profile.fullName?.trim() || 'Vault User';

          // Load the photo belonging to the currently logged-in user.
          // This makes the avatar appear immediately after login without needing a refresh.
          const userPhotoKey = `pdv_profile_photo_${profile.id}`;
          this.profilePhoto = localStorage.getItem(userPhotoKey) ?? '';
        },
        error: () => {
          this.fullName = 'Vault User';
        }
      });
  }

  @HostListener('document:keydown', ['$event'])
  handleGlobalShortcut(event: KeyboardEvent): void {
    if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'k') {
      event.preventDefault();
      event.stopPropagation();

      const input = this.topbarSearchInput?.nativeElement;

      if (input) {
        input.focus();
        input.select();
      } else {
        this.router.navigate(['/search']);
      }
    }
  }

  onTopbarSearch(): void {
    const term = this.topbarSearchTerm.trim();

    if (term) {
      this.router.navigate(['/search'], {
        queryParams: { term }
      });
    } else {
      this.router.navigate(['/search']);
    }
  }

  @HostListener('document:click', ['$event'])
  closeNotifications(event: Event): void {
    const target = event.target as HTMLElement;
    if (!target.closest('[data-notification-area]')) {
      this.notificationsOpen = false;
    }
  }

  toggleNotifications(event: Event): void {
    event.stopPropagation();
    this.notificationsOpen = !this.notificationsOpen;
  }

  goToProfile(): void {
    this.notificationsOpen = false;
    this.router.navigate(['/profile']);
  }

  logout(): void {
    this.authService.logout();
    localStorage.removeItem('pdv_profile_photo');
    this.router.navigate(['/login']);
  }

  get displayName(): string {
    return this.fullName;
  }

  get initials(): string {
    const name = this.fullName.trim();
    if (!name) return 'U';
    return name.split(/\s+/).slice(0, 2).map(part => part[0]).join('').toUpperCase();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    window.removeEventListener('pdv-profile-photo-changed', this.onProfileChanged);
    window.removeEventListener('pdv-profile-changed', this.onProfileChanged);
  }
}
