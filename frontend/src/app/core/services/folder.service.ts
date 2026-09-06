import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Folder } from '../models/folder.model';

@Injectable({
  providedIn: 'root'
})
export class FolderService {
  private readonly apiUrl = '/api/Folders';

  constructor(private http: HttpClient) {}

  getFolders(): Observable<Folder[]> {
    return this.http.get<Folder[]>(this.apiUrl);
  }

  createFolder(name: string): Observable<Folder> {
    return this.http.post<Folder>(this.apiUrl, {
      name
    });
  }

  renameFolder(id: number, name: string): Observable<Folder> {
    return this.http.put<Folder>(
      `${this.apiUrl}/${id}`,
      {
        name
      }
    );
  }

  deleteFolder(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}