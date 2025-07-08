import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { AdminEditUser } from './admin-edit-user';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../../services/user.service';
import { BuyerService } from '../../../services/buyer.service';
import { AgentService } from '../../../services/agent.service';
import { of, throwError } from 'rxjs';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { User } from '../../../models/user.model';

describe('AdminEditUser', () => {
  let component: AdminEditUser;
  let fixture: ComponentFixture<AdminEditUser>;
  let mockUserService: any;
  let mockBuyerService: any;
  let mockAgentService: any;
  let mockRouter: any;

  const testUserId = '123';

  const mockUser = {
    id: testUserId,
    role: 'Buyer',
    name: 'Test User',
    email: 'test@example.com',
    isDeleted: false,
    createdAt: new Date(),
    buyerProfile: {
      phone: '1234567890',
      preferredLocation: 'City',
      budget: 100000
    },
    agentProfile: undefined
  };

  beforeEach(async () => {
    mockUserService = {
      getAllUsers: jasmine.createSpy().and.returnValue(of({
        items: [mockUser]
      }))
    };

    mockBuyerService = {
      updateBuyer: jasmine.createSpy().and.returnValue(of({}))
    };

    mockAgentService = {
      updateAgent: jasmine.createSpy().and.returnValue(of({}))
    };

    mockRouter = {
      navigate: jasmine.createSpy()
    };

    await TestBed.configureTestingModule({
      imports: [AdminEditUser,ReactiveFormsModule, FormsModule, CommonModule],
      providers: [
        { provide: UserService, useValue: mockUserService },
        { provide: BuyerService, useValue: mockBuyerService },
        { provide: AgentService, useValue: mockAgentService },
        { provide: Router, useValue: mockRouter },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              params: { id: testUserId }
            }
          }
        }
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminEditUser);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component and load user', () => {
    expect(component).toBeTruthy();
    expect(component.user).toEqual(mockUser as User);
    expect(component.profileForm).toBeDefined();
    expect(component.isBuyer).toBeTrue();
  });

  it('should submit buyer profile form successfully', () => {
    component.profileForm.setValue({
      phone: '9876543210',
      preferredLocation: 'New City',
      budget: 500000
    });

    component.onSubmit();

    expect(mockBuyerService.updateBuyer).toHaveBeenCalledWith(testUserId, {
      phone: '9876543210',
      preferredLocation: 'New City',
      budget: 500000
    });
  });

  it('should handle error if user loading fails', fakeAsync(() => {
    mockUserService.getAllUsers.and.returnValue(throwError(() => new Error('Network Error')));

    // Re-trigger init
    component.ngOnInit();
    tick();

    expect(component.error).toBe('Failed to load user details. Please try again.');
  }));
});
