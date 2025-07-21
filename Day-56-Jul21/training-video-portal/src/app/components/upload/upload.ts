import { Component } from '@angular/core';
import { UploadTrainingVideo } from '../../models/training-video';
import { VideoService } from '../../services/video.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-upload',
  imports: [FormsModule,CommonModule],
  templateUrl: './upload.html',
  styleUrl: './upload.css'
})
export class Upload {
  title = '';
  description = '';
  selectedFile!: File;
  previewUrl: string | ArrayBuffer | null = null;
  isUploading = false;
  successMessage = '';

  constructor(private videoService: VideoService) {}

  onFileSelected(event: Event): void {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput?.files?.length) {
      this.selectedFile = fileInput.files[0];

      // Live preview
      const reader = new FileReader();
      reader.onload = () => this.previewUrl = reader.result;
      reader.readAsDataURL(this.selectedFile);
    }
  }

  uploadVideo(): void {
    if (!this.title || !this.description || !this.selectedFile) return;

    const formData = new FormData();
    formData.append('Title', this.title);
    formData.append('Description', this.description);
    formData.append('Video', this.selectedFile);

    this.isUploading = true;
    this.videoService.uploadVideo(formData).subscribe({
      next: (response) => {
        this.successMessage = `Video "${response.title}" uploaded successfully!`;
        this.isUploading = false;
        this.previewUrl = null;
        this.title = '';
        this.description = '';
      },
      error: (err) => {
        console.error('Upload failed:', err);
        this.isUploading = false;
      }
    });
  }
}
