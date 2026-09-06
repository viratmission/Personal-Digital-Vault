import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../../core/services/admin.service';
import { AdminDashboard, AdminUser } from '../../../core/models/admin.model';

@Component({
  selector: 'app-admin-shell',
  imports: [],
  templateUrl: './admin-shell.component.html',
  styleUrl: './admin-shell.component.css'
})
export class AdminShellComponent implements OnInit {

  dashboard: AdminDashboard | null = null;
  users: AdminUser[] = [];

  isLoading = true;
  errorMessage = '';

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadAdminData();
  }

  loadAdminData(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.adminService.getDashboard().subscribe({
      next: (data) => {
        this.dashboard = data;
        this.loadUsers();
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Unable to load admin dashboard.';
      }
    });
  }

  loadUsers(): void {
    this.adminService.getUsers().subscribe({
      next: (users) => {
        this.users = users;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Unable to load users.';
      }
    });
  }

  deleteUser(userId: number): void {
    const confirmed = window.confirm(
      'Are you sure you want to delete this user?'
    );

    if (!confirmed) {
      return;
    }

    this.adminService.deleteUser(userId).subscribe({
      next: () => {
        this.users = this.users.filter(user => user.id !== userId);
        this.loadAdminData();
      },
      error: () => {
        this.errorMessage = 'Unable to delete the user.';
      }
    });
  }
}