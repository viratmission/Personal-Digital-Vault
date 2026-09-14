import { Component, OnDestroy, inject } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { filter, Subject, takeUntil } from 'rxjs';
import { AuthService } from './core/services/auth.service';
import { AppShellComponent } from './shared/components/app-shell/app-shell.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, AppShellComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnDestroy {
  private readonly router = inject(Router);
  readonly authService = inject(AuthService);
  private readonly destroy$ = new Subject<void>();

  isPublicPage = true;

  constructor() {
    this.updateLayout(this.router.url);
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      takeUntil(this.destroy$)
    ).subscribe(event => this.updateLayout((event as NavigationEnd).urlAfterRedirects));
  }

  private updateLayout(url: string): void {
    this.isPublicPage = url.startsWith('/login') || url.startsWith('/register') || url.startsWith('/forbidden') || url.startsWith('/404');
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}