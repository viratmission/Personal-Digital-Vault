import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { SearchService } from '../../../core/services/search.service';
import { SearchResult } from '../../../core/models/search.model';

@Component({
  standalone: true,
  selector: 'app-search',
  imports: [FormsModule],
  templateUrl: './search.component.html',
  styleUrl: './search.component.css'
})
export class SearchComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);

  searchTerm = '';
  itemType = '';

  results: SearchResult[] = [];

  isLoading = false;
  errorMessage = '';

  constructor(private searchService: SearchService) {}

  ngOnInit(): void {
    const term = this.route.snapshot.queryParamMap.get('term');
    if (term) {
      this.searchTerm = term;
      this.onSearch();
    }
  }

  onSearch(): void {
    const term = this.searchTerm.trim();

    if (!term) {
      this.results = [];
      this.errorMessage = '';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.searchService.search(term, this.itemType)
      .subscribe({
        next: (results) => {
          this.results = results;
          this.isLoading = false;
        },
        error: () => {
          this.results = [];
          this.isLoading = false;
          this.errorMessage = 'Unable to complete the search.';
        }
      });
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.itemType = '';
    this.results = [];
    this.errorMessage = '';
  }
}