import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FileUploadComponent } from './file-upload-component';
import { JsonPipe } from '@angular/common';
import { BulkInsertService } from '../services/BulkInsertService';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('FileUploadComponent', () => {
  let component: FileUploadComponent;
  let fixture: ComponentFixture<FileUploadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FileUploadComponent,JsonPipe],
      providers:[BulkInsertService,provideHttpClient(),provideHttpClientTesting()]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FileUploadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
