import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { Folder } from '../../../core/models/folder.model';
import { FolderService } from '../../../core/services/folder.service';
import { VaultDocument } from '../../../core/models/document.model';
import { DocumentService } from '../../../core/services/document.service';

@Component({
  selector: 'app-vault-shell',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './vault-shell.component.html',
  styleUrl: './vault-shell.component.css'
})
export class VaultShellComponent implements OnInit {
  folders: Folder[] = [];

  newFolderName = '';

  editingFolderId: number | null = null;
  editingFolderName = '';

  deletingFolderId: number | null = null;

  isLoading = false;
  isCreatingFolder = false;
  isRenamingFolder = false;

  errorMessage = '';
  successMessage = '';

  documents: VaultDocument[] = [];
  isLoadingDocuments = false;
  documentErrorMessage = '';

  selectedFile: File | null = null;
  selectedFolderId: number | null = null;

  isUploadingDocument = false;
  documentSuccessMessage = '';

  editingDocumentId: number | null = null;
  editingDocumentFileName = '';
  isRenamingDocument = false;

  deletingDocumentId: number | null = null;

  showDeleteModal = false;
  deleteTargetType: 'document' | 'folder' | null = null;
  deleteTargetId: number | null = null;
  deleteTargetName = '';

  constructor(
    private folderService: FolderService,
    private documentService: DocumentService
  ) {}

  ngOnInit(): void {
    this.loadFolders();
    this.loadDocuments();
  }

  loadFolders(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.folderService.getFolders().subscribe({
      next: (folders) => {
        this.folders = folders;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Failed to load folders:', error);
        this.errorMessage = 'Unable to load folders.';
        this.isLoading = false;
      }
    });
  }

  createFolder(): void {
    const folderName = this.newFolderName.trim();

    if (!folderName) {
      this.errorMessage = 'Folder name is required.';
      return;
    }

    this.isCreatingFolder = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.folderService.createFolder(folderName).subscribe({
      next: (createdFolder) => {
        this.folders.push(createdFolder);

        this.newFolderName = '';
        this.successMessage = 'Folder created successfully.';
        this.isCreatingFolder = false;
      },
      error: (error) => {
        console.error('Failed to create folder:', error);
        this.errorMessage = 'Unable to create folder.';
        this.isCreatingFolder = false;
      }
    });
  }

  startRename(folder: Folder): void {
    this.editingFolderId = folder.id;
    this.editingFolderName = folder.name;

    this.errorMessage = '';
    this.successMessage = '';
  }

  cancelRename(): void {
    this.editingFolderId = null;
    this.editingFolderName = '';
  }

  saveRename(folderId: number): void {
    const folderName = this.editingFolderName.trim();

    if (!folderName) {
      this.errorMessage = 'Folder name is required.';
      return;
    }

    this.isRenamingFolder = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.folderService.renameFolder(folderId, folderName).subscribe({
      next: (updatedFolder) => {
        const index = this.folders.findIndex(
          folder => folder.id === folderId
        );

        if (index !== -1) {
          this.folders[index] = updatedFolder;
        }

        this.editingFolderId = null;
        this.editingFolderName = '';

        this.successMessage = 'Folder renamed successfully.';
        this.isRenamingFolder = false;
      },
      error: (error) => {
        console.error('Failed to rename folder:', error);
        this.errorMessage = 'Unable to rename folder.';
        this.isRenamingFolder = false;
      }
    });
  }

  deleteFolder(folder: Folder): void {
    this.openDeleteModal('folder', folder.id, folder.name);
  }

  loadDocuments(): void {
  this.isLoadingDocuments = true;
  this.documentErrorMessage = '';

  this.documentService.getDocuments().subscribe({
    next: (documents) => {
      this.documents = documents;
      this.isLoadingDocuments = false;
    },
    error: (error) => {
      console.error('Failed to load documents:', error);
      this.documentErrorMessage = 'Unable to load documents.';
      this.isLoadingDocuments = false;
    }
  });
}

onFileSelected(event: Event): void {
  const input = event.target as HTMLInputElement;

  if (input.files && input.files.length > 0) {
    this.selectedFile = input.files[0];
  } else {
    this.selectedFile = null;
  }
}

uploadDocument(): void {
  if (!this.selectedFile) {
    this.documentErrorMessage = 'Please select a file.';
    return;
  }

  if (this.selectedFolderId === null) {
    this.documentErrorMessage = 'Please select a folder.';
    return;
  }

  this.isUploadingDocument = true;
  this.documentErrorMessage = '';
  this.documentSuccessMessage = '';

  this.documentService.uploadDocument(
    this.selectedFile,
    this.selectedFolderId
  ).subscribe({
    next: (uploadedDocument) => {
      this.documents.push(uploadedDocument);

      this.selectedFile = null;
      this.selectedFolderId = null;

      this.documentSuccessMessage =
        'Document uploaded successfully.';

      this.isUploadingDocument = false;
    },
    error: (error) => {
      console.error('Failed to upload document:', error);

      this.documentErrorMessage =
        'Unable to upload document.';

      this.isUploadingDocument = false;
    }
  });
}

downloadDocument(vaultDocument: VaultDocument): void {
  this.documentErrorMessage = '';
  this.documentSuccessMessage = '';

  this.documentService.downloadDocument(vaultDocument.id).subscribe({
    next: (blob) => {
      const url = window.URL.createObjectURL(blob);

      const anchor = window.document.createElement('a');
      anchor.href = url;
      anchor.download = vaultDocument.fileName;

      anchor.click();

      window.URL.revokeObjectURL(url);

      this.documentSuccessMessage =
        'Document downloaded successfully.';
    },
    error: (error) => {
      console.error('Failed to download document:', error);

      this.documentErrorMessage =
        'Unable to download document.';
    }
  });
}

  startDocumentRename(vaultDocument: VaultDocument): void {
  this.editingDocumentId = vaultDocument.id;
  this.editingDocumentFileName = vaultDocument.fileName;

  this.documentErrorMessage = '';
  this.documentSuccessMessage = '';
}

cancelDocumentRename(): void {
  this.editingDocumentId = null;
  this.editingDocumentFileName = '';
}

saveDocumentRename(documentId: number): void {
  const fileName = this.editingDocumentFileName.trim();

  if (!fileName) {
    this.documentErrorMessage = 'File name is required.';
    return;
  }

  this.isRenamingDocument = true;
  this.documentErrorMessage = '';
  this.documentSuccessMessage = '';

  this.documentService
    .renameDocument(documentId, fileName)
    .subscribe({
      next: (updatedDocument) => {
        const index = this.documents.findIndex(
          item => item.id === documentId
        );

        if (index !== -1) {
          this.documents[index] = updatedDocument;
        }

        this.editingDocumentId = null;
        this.editingDocumentFileName = '';

        this.documentSuccessMessage =
          'Document renamed successfully.';

        this.isRenamingDocument = false;
      },
      error: (error) => {
        console.error('Failed to rename document:', error);

        this.documentErrorMessage =
          'Unable to rename document.';

        this.isRenamingDocument = false;
      }
    });
}

deleteDocument(vaultDocument: VaultDocument): void {
  this.openDeleteModal('document', vaultDocument.id, vaultDocument.fileName);
}

openDeleteModal(
  type: 'document' | 'folder',
  id: number,
  name: string
): void {
  this.deleteTargetType = type;
  this.deleteTargetId = id;
  this.deleteTargetName = name;
  this.showDeleteModal = true;
}

closeDeleteModal(): void {
  if (this.deletingFolderId !== null || this.deletingDocumentId !== null) {
    return;
  }

  this.showDeleteModal = false;
  this.deleteTargetType = null;
  this.deleteTargetId = null;
  this.deleteTargetName = '';
}

confirmDelete(): void {
  if (this.deleteTargetType === null || this.deleteTargetId === null) {
    return;
  }

  const type = this.deleteTargetType;
  const id = this.deleteTargetId;

  if (type === 'folder') {
    this.deletingFolderId = id;
    this.errorMessage = '';
    this.successMessage = '';

    this.folderService.deleteFolder(id).subscribe({
      next: () => {
        this.folders = this.folders.filter(item => item.id !== id);
        this.successMessage = 'Folder deleted successfully.';
        this.deletingFolderId = null;
        this.finishDeleteModal();
      },
      error: (error) => {
        console.error('Failed to delete folder:', error);
        this.errorMessage = 'Unable to delete folder.';
        this.deletingFolderId = null;
        this.finishDeleteModal();
      }
    });

    return;
  }

  this.deletingDocumentId = id;
  this.documentErrorMessage = '';
  this.documentSuccessMessage = '';

  this.documentService.deleteDocument(id).subscribe({
    next: () => {
      this.documents = this.documents.filter(item => item.id !== id);
      this.documentSuccessMessage = 'Document deleted successfully.';
      this.deletingDocumentId = null;
      this.finishDeleteModal();
    },
    error: (error) => {
      console.error('Failed to delete document:', error);
      this.documentErrorMessage = 'Unable to delete document.';
      this.deletingDocumentId = null;
      this.finishDeleteModal();
    }
  });
}

private finishDeleteModal(): void {
  this.showDeleteModal = false;
  this.deleteTargetType = null;
  this.deleteTargetId = null;
  this.deleteTargetName = '';
}
}