import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { ImageCleanupService, CleanupResponse } from './image-cleanup.service';
import { environment } from '../../environments/environment';

describe('ImageCleanupService', () => {
  let service: ImageCleanupService;
  let httpMock: HttpTestingController;

  const apiUrl = `${environment.apiUrl}/propertyimages/admin/images/cleanup`;

  const mockResponse: CleanupResponse = {
    message: 'Cleanup successful',
    imagesCleaned: 5,
    timestamp: new Date().toISOString()
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ImageCleanupService]
    });

    service = TestBed.inject(ImageCleanupService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should clean up deleted images and return response', () => {
    service.cleanupDeletedImages().subscribe(response => {
      expect(response).toEqual(mockResponse);
      expect(response.imagesCleaned).toBe(5);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('DELETE');
    req.flush(mockResponse);
  });

  it('should throw error on failed cleanup', () => {
    const mockError = {
      status: 500,
      statusText: 'Server Error',
      error: { message: 'Internal Server Error' }
    };

    service.cleanupDeletedImages().subscribe({
      next: () => fail('should have thrown an error'),
      error: (err: Error) => {
        expect(err.message).toBe('Internal Server Error');
      }
    });

    const req = httpMock.expectOne(apiUrl);
    req.flush(mockError.error, mockError);
  });
});
