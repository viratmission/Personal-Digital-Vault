import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  LoginRequest,
  LoginResponse,
  RegisterRequest
} from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly tokenStorageKey = 'pdv_jwt';

  private token: string | null =
    sessionStorage.getItem(this.tokenStorageKey);

  constructor(private http: HttpClient) {}

  register(request: RegisterRequest): Observable<string> {
    return this.http.post(
      `${environment.apiUrl}/Auth/register`,
      request,
      {
        responseType: 'text'
      }
    );
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(
        `${environment.apiUrl}/Auth/login`,
        request
      )
      .pipe(
        tap(response => {
          this.setToken(response.token);
        })
      );
  }

  logout(): void {
    this.token = null;
    sessionStorage.removeItem(this.tokenStorageKey);
  }

  getToken(): string | null {
    return this.token;
  }

  isLoggedIn(): boolean {
    return this.token !== null;
  }

  private setToken(token: string): void {
    this.token = token;
    sessionStorage.setItem(this.tokenStorageKey, token);
  }
}