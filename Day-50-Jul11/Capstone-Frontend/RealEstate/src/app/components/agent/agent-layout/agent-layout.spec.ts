import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AgentLayout } from './agent-layout';
import { ActivatedRoute, RouterOutlet } from '@angular/router';
import { Navbar } from '../../shared/navbar/navbar';
import { Sidebar } from '../../shared/sidebar/sidebar';
import { AuthService } from '../../../services/auth.service';

describe('AgentLayout', () => {
  let component: AgentLayout;
  let fixture: ComponentFixture<AgentLayout>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  const mockUser = {
    id: 'agent1',
    name: 'John Doe',
    email: 'john@example.com',
    role: 'Agent'
  };

  const activatedRouteMock = {
    snapshot:{
      params:{}
    }
  };

  beforeEach(async () => {
    authServiceSpy = jasmine.createSpyObj('AuthService', [
      'getCurrentUser'
    ]);

    authServiceSpy.getCurrentUser.and.returnValue(mockUser);

    await TestBed.configureTestingModule({
      imports: [AgentLayout, RouterOutlet, Sidebar, Navbar],
      providers: [
        {provide: AuthService, useValue: authServiceSpy},
        {provide: ActivatedRoute,useValue:activatedRouteMock}
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AgentLayout);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with sidebar not collapsed', () => {
    expect(component.isSidebarCollapsed).toBeFalse();
  });

  it('should update sidebar collapsed state when toggled', () => {
    // First toggle to collapsed
    component.onSidebarToggle(true);
    expect(component.isSidebarCollapsed).toBeTrue();

    // Then toggle back to expanded
    component.onSidebarToggle(false);
    expect(component.isSidebarCollapsed).toBeFalse();
  });

});