import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ImageCleanupService } from '../../../services/image-cleanup.service';

@Component({
  selector: 'app-admin-image-cleanup',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-image-cleanup.html',
  styleUrls: ['./admin-image-cleanup.css']
})
export class AdminImageCleanup {
  isCleaning = false;
  resultMessage = '';
  errorMessage = '';
  cleanedCount=0;

  constructor(private cleanupService: ImageCleanupService) {}

  runCleanup(): void {
    this.isCleaning = true;
    this.resultMessage = '';
    this.errorMessage = '';

    if(confirm("Are you sure you want to cleanup the images?")){
        this.cleanupService.cleanupDeletedImages().subscribe({
        next: (response) => {
          this.resultMessage = response.message;
          this.cleanedCount=response.imagesCleaned;
          this.isCleaning = false;
        },
        error: (err) => {
          this.errorMessage = err.error?.message || 'An error occurred during cleanup';
          this.isCleaning = false;
        }
      });
    }else{
      this.errorMessage="Cleanup action cancelled";
      this.isCleaning=false;
    }

    
  }
}