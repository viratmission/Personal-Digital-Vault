import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Credential } from '../models/credential.model';

@Injectable({
  providedIn: 'root'
})
export class CredentialService {

  private readonly apiUrl = '/api/Credential';

  constructor(private http: HttpClient) {}

  getCredentials(): Observable<Credential[]> {
    return this.http.get<Credential[]>(this.apiUrl);
  }

  getCredential(id: number): Observable<Credential> {
    return this.http.get<Credential>(`${this.apiUrl}/${id}`);
  }

  createCredential(sensitiveValue: string): Observable<Credential> {
    return this.http.post<Credential>(
      this.apiUrl,
      { sensitiveValue }
    );
  }

  updateCredential(
    id: number,
    sensitiveValue: string
  ): Observable<Credential> {
    return this.http.put<Credential>(
      `${this.apiUrl}/${id}`,
      { sensitiveValue }
    );
  }

  deleteCredential(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}