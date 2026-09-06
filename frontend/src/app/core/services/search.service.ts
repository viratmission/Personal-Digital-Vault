import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { SearchResult } from '../models/search.model';

@Injectable({
  providedIn: 'root'
})
export class SearchService {

  private readonly apiUrl = '/api/Search';

  constructor(private http: HttpClient) {}

  search(
    searchTerm: string,
    itemType?: string
  ): Observable<SearchResult[]> {

    let params = new HttpParams()
      .set('searchTerm', searchTerm);

    if (itemType) {
      params = params.set('itemType', itemType);
    }

    return this.http.get<SearchResult[]>(
      this.apiUrl,
      { params }
    );
  }
}