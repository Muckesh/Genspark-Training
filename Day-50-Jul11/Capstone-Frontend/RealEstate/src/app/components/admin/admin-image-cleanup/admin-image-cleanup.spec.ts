import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { AdminImageCleanup } from './admin-image-cleanup';
import { CleanupResponse, ImageCleanupService } from '../../../services/image-cleanup.service';
import { CommonModule } from '@angular/common';

describe('AdminImageCleanup', () => {
  let component: AdminImageCleanup;
  let fixture: ComponentFixture<AdminImageCleanup>;
  let cleanupServiceSpy: jasmine.SpyObj<ImageCleanupService>;

  beforeEach(waitForAsync(() => {
    cleanupServiceSpy = jasmine.createSpyObj('ImageCleanupService', ['cleanupDeletedImages']);

    TestBed.configureTestingModule({
      imports: [AdminImageCleanup, CommonModule],
      providers: [
        { provide: ImageCleanupService, useValue: cleanupServiceSpy }
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminImageCleanup);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should run cleanup successfully', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    cleanupServiceSpy.cleanupDeletedImages.and.returnValue(of({
      message: 'Cleaned up successfully',
      imagesCleaned: 5
    } as CleanupResponse));

    component.runCleanup();

    expect(component.isCleaning).toBeFalse();
    expect(component.resultMessage).toBe('Cleaned up successfully');
    expect(component.cleanedCount).toBe(5);
    expect(component.errorMessage).toBe('');
  });

  it('should handle error during cleanup', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    cleanupServiceSpy.cleanupDeletedImages.and.returnValue(throwError(() => ({
      error: { message: 'Cleanup failed' }
    })));

    component.runCleanup();

    expect(component.isCleaning).toBeFalse();
    expect(component.resultMessage).toBe('');
    expect(component.errorMessage).toBe('Cleanup failed');
  });

  it('should cancel cleanup if confirm returns false', () => {
    spyOn(window, 'confirm').and.returnValue(false);

    component.runCleanup();

    expect(component.errorMessage).toBe('Cleanup action cancelled');
    expect(component.isCleaning).toBeFalse();
    expect(cleanupServiceSpy.cleanupDeletedImages).not.toHaveBeenCalled();
  });
});
