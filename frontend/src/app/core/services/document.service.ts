import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { VaultDocument } from '../models/document.model';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private readonly apiUrl = '/api/Documents';

  constructor(private http: HttpClient) {}

  getDocuments(): Observable<VaultDocument[]> {
    return this.http.get<VaultDocument[]>(this.apiUrl);
  }

  uploadDocument(
    file: File,
    folderId: number
  ): Observable<VaultDocument> {
    const formData = new FormData();

    formData.append('file', file);
    formData.append('folderId', folderId.toString());

    return this.http.post<VaultDocument>(
      this.apiUrl,
      formData
    );
  }

  downloadDocument(id: number): Observable<Blob> {
    return this.http.get(
      `${this.apiUrl}/${id}/download`,
      {
        responseType: 'blob'
      }
    );
  }

  renameDocument(
    id: number,
    fileName: string
  ): Observable<VaultDocument> {
    return this.http.put<VaultDocument>(
      `${this.apiUrl}/${id}`,
      {
        fileName
      }
    );
  }

  deleteDocument(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}