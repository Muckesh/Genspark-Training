import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { TrainingVideo, UploadTrainingVideo } from "../models/training-video";
import { Observable } from "rxjs";

@Injectable()
export class VideoService{

    private readonly baseUrl = "http://localhost:5188/api/video"

    constructor(private http: HttpClient) { }

    uploadVideo(formData: FormData): Observable<UploadTrainingVideo> {
    return this.http.post<UploadTrainingVideo>(`${this.baseUrl}/upload`, formData);
  }

    getAllVideos() {
        return this.http.get<TrainingVideo[]>(this.baseUrl);
    }

    getVideo(id : string) {
        return this.http.get(`${this.baseUrl}/${id}/stream`);
    }
}