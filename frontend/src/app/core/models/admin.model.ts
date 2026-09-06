export interface AdminDashboard {
  totalUsers: number;
  totalUploads: number;
  totalStoredFiles: number;
}

export interface AdminUser {
  id: number;
  fullName: string;
  email: string;
  isAdmin: boolean;
}