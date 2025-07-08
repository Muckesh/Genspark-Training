import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { PropertyImageService } from './property-image.service';
import { environment } from '../../environments/environment';

describe('PropertyImageService', () => {
  let service: PropertyImageService;
  let httpMock: HttpTestingController;

  const baseUrl = `${environment.apiUrl}/propertyimages`;
  const uploadUrl = `${environment.apiUrl}/propertylistings/images/upload`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PropertyImageService]
    });

    service = TestBed.inject(PropertyImageService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch image URLs', () => {
    const mockResponse = {
      $values: ['url1.jpg', 'url2.jpg']
    };

    service.getImageUrls('listing123').subscribe(urls => {
      expect(urls.length).toBe(2);
      expect(urls).toEqual(['url1.jpg', 'url2.jpg']);
    });

    const req = httpMock.expectOne(`${baseUrl}/urls/listing123`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should return empty array if no image URLs are returned', () => {
    service.getImageUrls('listing123').subscribe(urls => {
      expect(urls).toEqual([]);
    });

    const req = httpMock.expectOne(`${baseUrl}/urls/listing123`);
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('should upload an image', () => {
    const mockFile = new File(['dummy content'], 'test.png', { type: 'image/png' });

    service.uploadImage('listing123', mockFile).subscribe(res => {
      expect(res).toEqual({ success: true });
    });

    const req = httpMock.expectOne(uploadUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body instanceof FormData).toBeTrue();
    expect(req.request.body.get('ImageFile')).toBeTruthy();
    expect(req.request.body.get('PropertyListingId')).toBe('listing123');

    req.flush({ success: true });
  });

  it('should delete an image by ID', () => {
    service.deleteImage('img123').subscribe(res => {
      expect(res).toEqual({ message: 'Deleted' });
    });

    const req = httpMock.expectOne(`${baseUrl}/img123`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ message: 'Deleted' });
  });
});
