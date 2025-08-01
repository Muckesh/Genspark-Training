import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NewsService } from '../../../services/news.service';
import { NewsRequest, NewsResponse } from '../../../models/news.model';

@Component({
  selector: 'app-news',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './news.html',
  styleUrls: ['./news.css']
})
export class News implements OnInit {
  newsList: NewsResponse[] = [];
  isEditMode = false;
  editingNewsId: number | null = null;
  imagePreview: string | null = null;
  fileInput: HTMLInputElement | null = null;
  currentImageUrl: string | null = null;
  showForm = false; 
  isExporting=false;

  newsFormData: NewsRequest = {
    userId: 1,
    title: '',
    shortDescription: '',
    content: '',
    status: 1,
    image: null,
  };

  constructor(private newsService: NewsService) {}

  ngOnInit(): void {
    this.loadNews();
    this.fileInput = document.querySelector('#newsImageInput') as HTMLInputElement;
  }

  loadNews() {
    this.newsService.getAll().subscribe({
      next: data => this.newsList = data,
      error: err => console.error('Failed to load news', err)
    });
  }

  exportToExcel(){
    this.isExporting=true;
    this.newsService.exportToExcel().subscribe({
      next:(blob)=>{
        this.downloadFile(blob,"News_Export.xlsx");
        this.isExporting=false;
      },
      error: (err) => {
        console.error('Export to Excel failed', err);
        alert('Failed to export to Excel');
        this.isExporting = false;
      }
    });
  }

  exportToCSV() {
    this.isExporting = true;
    this.newsService.exportToCsv().subscribe({
      next: (blob) => {
        this.downloadFile(blob, 'News_Export.csv');
        this.isExporting = false;
      },
      error: (err) => {
        console.error('Export to CSV failed', err);
        alert('Failed to export to CSV');
        this.isExporting = false;
      }
    });
  }

  private downloadFile(blob:Blob,fileName:string){
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
  }

  showCreateForm() {
    this.resetForm();
    this.showForm = true;
    this.isEditMode = false;
  }

  onImageSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.newsFormData.image = file;
      this.currentImageUrl = null;

      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  onSubmit() {
    if (this.isEditMode && this.editingNewsId) {
      const updateData: NewsRequest = {
        ...this.newsFormData,
        userId: this.newsFormData.userId || 1,
        status: this.newsFormData.status || 1,
        image: this.newsFormData.image || null
      };

      this.newsService.update(this.editingNewsId, updateData).subscribe({
        next: () => {
          this.loadNews();
          this.resetForm();
          alert('News updated successfully!');
        },
        error: err => {
          console.error('Update failed', err);
          alert(`Update failed: ${err.error?.message || err.message}`);
        }
      });
    } else {
      const createData: NewsRequest = {
        ...this.newsFormData,
      };

      this.newsService.create(createData).subscribe({
        next: () => {
          this.loadNews();
          this.resetForm();
          alert('News created successfully!');
        },
        error: err => {
          console.error('Create failed', err);
          alert(`Create failed: ${err.error?.message || err.message}`);
        }
      });
    }
  }

  // private createFileFromUrl(url: string | null): File | null {
  //   if (!url) return null;
  //   return new File([], 'existing-image.jpg', { type: 'image/jpeg' });
  // }

  editNews(news: NewsResponse) {
    this.isEditMode = true;
    this.showForm = true;
    this.editingNewsId = news.newsId;
    this.currentImageUrl = news.image;
    this.imagePreview = news.image;
    
    this.newsFormData = {
      userId: news.userId || 1,
      title: news.title,
      shortDescription: news.shortDescription,
      content: news.content,
      status: news.status || 1,
      image: null,
    };
  }

  deleteNews(id: number) {
    if (confirm("Are you sure you want to delete this news?")) {
      this.newsService.delete(id).subscribe({
        next: () => {
          this.loadNews();
          alert('News deleted successfully!');
        },
        error: err => {
          console.error('Delete failed', err);
          alert(`Delete failed: ${err.error?.message || err.message}`);
        }
      });
    }
  }

  resetForm() {
    this.isEditMode = false;
    this.showForm = false;
    this.editingNewsId = null;
    this.currentImageUrl = null;
    this.newsFormData = {
      userId: 1,
      title: '',
      shortDescription: '',
      content: '',
      status: 1,
      image: null,
    };
    this.imagePreview = null;
    
    if (this.fileInput) {
      this.fileInput.value = '';
    }
  }

  cancelForm() {
    this.resetForm();
  }
}