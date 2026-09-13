import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app-shell.component.html',
  styleUrl: './app-shell.component.css'
})
export class AppShellComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  navItems = [
    { label: 'Dashboard', icon: '⌂', route: '/dashboard' },
    { label: 'My Vault', icon: '▣', route: '/vault' },
    { label: 'Credentials', icon: '▤', route: '/credentials' },
    { label: 'Search', icon: '⌕', route: '/search' },
    { label: 'Profile', icon: '◯', route: '/profile' }
  ];

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  get displayName(): string {
    return 'Vault User';
  }
}