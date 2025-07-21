import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { VideoList } from './components/video-list/video-list';
import { Upload } from './components/upload/upload';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,VideoList,Upload],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'training-video-portal';
}
