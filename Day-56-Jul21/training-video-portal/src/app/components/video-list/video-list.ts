import { Component, OnInit } from '@angular/core';
import { VideoService } from '../../services/video.service';
import { TrainingVideo } from '../../models/training-video';
import { CommonModule, DatePipe } from '@angular/common';

@Component({
  selector: 'app-video-list',
  imports: [DatePipe,CommonModule],
  templateUrl: './video-list.html',
  styleUrl: './video-list.css'
})
export class VideoList implements OnInit {
  videos: TrainingVideo[] = [];

  constructor(private videoService: VideoService) {}

  ngOnInit(): void {
    this.videoService.getAllVideos().subscribe({
      next: (data) => this.videos = data,
      error: (err) => console.error('Failed to fetch videos:', err)
    });
  }
}
