import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { AdminViewUser } from './admin-view-user';
import { UserService } from '../../../services/user.service';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { User } from '../../../models/user.model';
import { PagedResult } from '../../../models/paged-result.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing'; // Add this
import { AgentService } from '../../../services/agent.service';
import { BuyerService } from '../../../services/buyer.service';

describe('AdminViewUser', () => {
  let component: AdminViewUser;
  let fixture: ComponentFixture<AdminViewUser>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let buyerServiceSpy: jasmine.SpyObj<BuyerService>; // Add this
  let agentServiceSpy: jasmine.SpyObj<AgentService>; // Add this

  const mockUser: User = { 
    id: '123', 
    name: 'Test User', 
    email: 'test@example.com', 
    role: 'Buyer' ,
    isDeleted:false
  };

  const mockPagedResult: PagedResult<User> = {
    items: [mockUser],
    pageNumber: 1,
    pageSize: 10,
    totalCount: 1,
  };

  beforeEach(async () => {
    userServiceSpy = jasmine.createSpyObj('UserService', ['getAllUsers']);
    buyerServiceSpy = jasmine.createSpyObj('BuyerService2', ['getBuyer']); // Add this
    agentServiceSpy = jasmine.createSpyObj('AgentService2', ['getAgent']); // Add this

    await TestBed.configureTestingModule({
      imports: [
        CommonModule,
        FormsModule,
        RouterTestingModule.withRoutes([]),
        HttpClientTestingModule, // Add this for HttpClient
        AdminViewUser
      ],
      providers: [
        { provide: UserService, useValue: userServiceSpy },
        { provide: BuyerService, useValue: buyerServiceSpy }, // Add this
        { provide: AgentService, useValue: agentServiceSpy }, // Add this
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              params: { id: '123' }
            }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminViewUser);
    component = fixture.componentInstance;
    userServiceSpy.getAllUsers.and.returnValue(of(mockPagedResult));
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load user details on initialization', fakeAsync(() => {
    component.ngOnInit();
    tick();

    expect(userServiceSpy.getAllUsers).toHaveBeenCalled();
    expect(component.user).toEqual(mockUser);
    expect(component.isLoading).toBeFalse();
  }));

});