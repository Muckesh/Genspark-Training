import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminLayout } from './admin-layout';
import { ActivatedRoute, RouterOutlet } from '@angular/router';
import { Sidebar } from '../../shared/sidebar/sidebar';
import { Navbar } from '../../shared/navbar/navbar';
import { AuthService } from '../../../services/auth.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('AdminLayout', () => {
  let component: AdminLayout;
  let fixture: ComponentFixture<AdminLayout>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['getCurrentUser']);
    authServiceSpy.getCurrentUser.and.returnValue({
      id: '1',
      name: 'Mock Admin',
      role: 'Admin',
      email: 'admin@example.com'
    });

    const mockActivatedRoute = {
      snapshot:{
        params:{}
      }
    };

    await TestBed.configureTestingModule({
      imports: [AdminLayout, RouterOutlet, Sidebar, Navbar],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminLayout);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with sidebar not collapsed', () => {
    expect(component.isSidebarCollapsed).toBeFalse();
  });

  it('should update sidebar collapsed state when onSidebarToggle is called', () => {
    component.onSidebarToggle(true);
    expect(component.isSidebarCollapsed).toBeTrue();

    component.onSidebarToggle(false);
    expect(component.isSidebarCollapsed).toBeFalse();
  });
});