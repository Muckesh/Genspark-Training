import { Component, TemplateRef, ViewChild } from '@angular/core';
import { NewsService } from '../../../services/news.service';
import { NewsResponse } from '../../../models/news.model';
import { CommonModule } from '@angular/common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-news-list',
  imports: [CommonModule],
  templateUrl: './news-list.html',
  styleUrl: './news-list.css'
})
export class NewsList {
  newsList: NewsResponse[] = [];
  selectedNews:NewsResponse | null = null;
  loading: boolean = false;
  error: string = '';

  @ViewChild('newsDetailsModal') newsDetailsModal!: TemplateRef<any>;

  constructor(private newsService: NewsService,private modalService:NgbModal) {}

  ngOnInit(): void {
    this.loadNews();
  }

  loadNews(): void {
    this.loading = true;
    this.newsService.getAll().subscribe({
      next: (data) => {
        this.newsList = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load news:', err);
        this.error = 'Failed to load news';
        this.loading = false;
      }
    });
  }

  openNewsModal(news:NewsResponse){
    this.selectedNews = news;
    this.modalService.open(this.newsDetailsModal,{size:'lg'});
  }
}
