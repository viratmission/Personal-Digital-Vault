import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { FolderService } from '../../../core/services/folder.service';
import { DocumentService } from '../../../core/services/document.service';
import { CredentialService } from '../../../core/services/credential.service';
import { Folder } from '../../../core/models/folder.model';
import { VaultDocument } from '../../../core/models/document.model';
import { Credential } from '../../../core/models/credential.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  private readonly folderService = inject(FolderService);
  private readonly documentService = inject(DocumentService);
  private readonly credentialService = inject(CredentialService);

  folders: Folder[] = [];
  documents: VaultDocument[] = [];
  credentials: Credential[] = [];
  loading = true;
  errorMessage = '';

  ngOnInit(): void {
    forkJoin({
      folders: this.folderService.getFolders(),
      documents: this.documentService.getDocuments(),
      credentials: this.credentialService.getCredentials()
    }).subscribe({
      next: data => {
        this.folders = data.folders;
        this.documents = data.documents;
        this.credentials = data.credentials;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'Some vault information could not be loaded.';
      }
    });
  }

  get totalStorage(): number { return this.documents.reduce((sum, d) => sum + (d.fileSize || 0), 0); }
  get recentDocuments(): VaultDocument[] { return [...this.documents].sort((a,b) => new Date(b.uploadedAt).getTime() - new Date(a.uploadedAt).getTime()).slice(0,5); }

  formatSize(bytes: number): string {
    if (!bytes) return '0 B';
    const units = ['B','KB','MB','GB']; let i=0, value=bytes;
    while(value>=1024 && i<units.length-1){ value/=1024; i++; }
    return `${value < 10 && i > 0 ? value.toFixed(1) : Math.round(value)} ${units[i]}`;
  }

  formatDate(value: string): string { return new Date(value).toLocaleDateString(undefined,{month:'short',day:'numeric',year:'numeric'}); }
  fileType(name: string): string { const ext=name.split('.').pop()?.toUpperCase() || 'FILE'; return ext.slice(0,4); }
}