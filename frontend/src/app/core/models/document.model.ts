export interface VaultDocument {
  id: number;
  fileName: string;
  storedFileName: string;
  filePath: string;
  contentType: string;
  fileSize: number;
  uploadedAt: string;
  userId: number;
  folderId: number;
  sha256Hash: string;
}