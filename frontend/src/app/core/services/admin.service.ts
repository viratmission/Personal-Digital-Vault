import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { AdminDashboard, AdminUser } from '../models/admin.model';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  private readonly apiUrl = '/api/Admin';

  constructor(private http: HttpClient) {}

  getDashboard(): Observable<AdminDashboard> {
    return this.http.get<AdminDashboard>(
      `${this.apiUrl}/dashboard`
    );
  }

  getUsers(): Observable<AdminUser[]> {
    return this.http.get<AdminUser[]>(
      `${this.apiUrl}/users`
    );
  }

  deleteUser(userId: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(
      `${this.apiUrl}/users/${userId}`
    );
  }
}