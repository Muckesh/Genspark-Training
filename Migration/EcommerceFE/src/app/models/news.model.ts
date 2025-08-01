export interface NewsRequest {
  userId?: number;
  title: string;
  shortDescription: string;
  content: string;
  status?: number;
  image: File | null;
  createdDate?: Date;
}

export interface NewsResponse {
  newsId: number;
  userId?: number;
  title: string;
  shortDescription: string;
  content: string;
  image: string;
  createdDate?: Date;
  status?: number;
}
