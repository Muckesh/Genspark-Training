export interface ContactRequest {
  name: string;
  email: string;
  phone: string;
  content: string;
  recaptchaToken: string;
}

export interface ContactResponse {
  id: number;
  name: string;
  email: string;
  phone: string;
  content: string;
}

export interface CaptchaResponse {
  success: boolean;
  errorCodes: string[];
}