export interface Profile {
  id: number;
  fullName: string;
  email: string;
}

export interface UpdateProfileRequest {
  fullName: string;
  email: string;
}