import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  Profile,
  UpdateProfileRequest
} from '../models/profile.model';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {

  private readonly profileUrl =
    `${environment.apiUrl}/Profile`;

  constructor(private http: HttpClient) {}

  getProfile(): Observable<Profile> {
    return this.http.get<Profile>(
      this.profileUrl
    );
  }

  updateProfile(
    request: UpdateProfileRequest
  ): Observable<string> {

    return this.http.put(
      this.profileUrl,
      request,
      {
        responseType: 'text'
      }
    );
  }
}