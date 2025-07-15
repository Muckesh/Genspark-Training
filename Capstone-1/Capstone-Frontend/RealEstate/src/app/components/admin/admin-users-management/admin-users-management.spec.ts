import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { AdminUsersManagement } from './admin-users-management';
import { UserService } from '../../../services/user.service';
import { PagedResult } from '../../../models/paged-result.model';
import { User } from '../../../models/user.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { of, throwError } from 'rxjs';

describe('AdminUsersManagement', () => {
  let component: AdminUsersManagement;
  let fixture: ComponentFixture<AdminUsersManagement>;
  let userServiceSpy: jasmine.SpyObj<UserService>;

  const mockUsers: User[] = [
    { id: '1', name: 'Admin User', email: 'admin@test.com', role: 'Admin', isDeleted:false},
    { id: '2', name: 'Agent User', email: 'agent@test.com', role: 'Agent', isDeleted:false},
    { id: '3', name: 'Buyer User', email: 'buyer@test.com', role: 'Buyer', isDeleted:false}
  ];

  const mockPagedResult: PagedResult<User> = {
    items: mockUsers,
    pageNumber: 1,
    pageSize: 10,
    totalCount: 3,
  };

  beforeEach(async () => {
    userServiceSpy = jasmine.createSpyObj('UserService', [
      'getAllUsers',
      'deleteUser'
    ]);

    userServiceSpy.getAllUsers.and.returnValue(of(mockPagedResult));
    userServiceSpy.deleteUser.and.returnValue(of({}));

    await TestBed.configureTestingModule({
      imports: [AdminUsersManagement, CommonModule, FormsModule, RouterLink],
      providers: [
        { provide: UserService, useValue: userServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminUsersManagement);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load users and update stats on init', fakeAsync(() => {
    component.ngOnInit();
    tick(300); // Account for debounceTime

    expect(userServiceSpy.getAllUsers).toHaveBeenCalled();
    expect(component.users.length).toBe(3);
    expect(component.filteredUsers.length).toBe(3);
    expect(component.stats[0].value).toBe(3); // Total Users
    expect(component.stats[1].value).toBe(1); // Agents
    expect(component.stats[2].value).toBe(1); // Buyers
    expect(component.stats[3].value).toBe(1); // Admins
    expect(component.isLoading).toBeFalse();
  }));


});