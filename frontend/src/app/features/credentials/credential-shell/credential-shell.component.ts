import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CredentialService } from '../../../core/services/credential.service';
import { Credential } from '../../../core/models/credential.model';

@Component({
  selector: 'app-credential-shell',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './credential-shell.component.html',
  styleUrl: './credential-shell.component.css'
})
export class CredentialShellComponent implements OnInit {

  credentials: Credential[] = [];

  sensitiveValue = '';

  editingId: number | null = null;
  editingValue = '';

  loading = false;
  errorMessage = '';
  successMessage = '';

  constructor(private credentialService: CredentialService) {}

  ngOnInit(): void {
    this.loadCredentials();
  }

  loadCredentials(): void {
    this.loading = true;
    this.errorMessage = '';

    this.credentialService.getCredentials().subscribe({
      next: (data) => {
        this.credentials = data;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load credentials.';
        this.loading = false;
      }
    });
  }

  addCredential(): void {
    if (!this.sensitiveValue.trim()) {
      this.errorMessage = 'Please enter a sensitive value.';
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.credentialService.createCredential(this.sensitiveValue).subscribe({
      next: () => {
        this.sensitiveValue = '';
        this.successMessage = 'Credential added successfully.';
        this.loading = false;
        this.loadCredentials();
      },
      error: () => {
        this.errorMessage = 'Failed to add credential.';
        this.loading = false;
      }
    });
  }

  startEdit(credential: Credential): void {
    this.editingId = credential.id;
    this.editingValue = '';
    this.errorMessage = '';
    this.successMessage = '';
  }

  cancelEdit(): void {
    this.editingId = null;
    this.editingValue = '';
  }

  updateCredential(): void {
    if (this.editingId === null) {
      return;
    }

    if (!this.editingValue.trim()) {
      this.errorMessage = 'Please enter a new sensitive value.';
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.credentialService
      .updateCredential(this.editingId, this.editingValue)
      .subscribe({
        next: () => {
          this.editingId = null;
          this.editingValue = '';
          this.successMessage = 'Credential updated successfully.';
          this.loading = false;
          this.loadCredentials();
        },
        error: () => {
          this.errorMessage = 'Failed to update credential.';
          this.loading = false;
        }
      });
  }

  deleteCredential(id: number): void {
    if (!confirm('Are you sure you want to delete this credential?')) {
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.credentialService.deleteCredential(id).subscribe({
      next: () => {
        this.successMessage = 'Credential deleted successfully.';
        this.loading = false;
        this.loadCredentials();
      },
      error: () => {
        this.errorMessage = 'Failed to delete credential.';
        this.loading = false;
      }
    });
  }
}