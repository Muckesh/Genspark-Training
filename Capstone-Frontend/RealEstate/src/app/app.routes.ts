import { Routes } from '@angular/router';
import { AdminDashboard } from './components/admin/admin-dashboard/admin-dashboard';
import { AdminEditUser } from './components/admin/admin-edit-user/admin-edit-user';
import { AdminImageCleanup } from './components/admin/admin-image-cleanup/admin-image-cleanup';
import { AdminLayout } from './components/admin/admin-layout/admin-layout';
import { AdminListingsManagement } from './components/admin/admin-listings-management/admin-listings-management';
import { AdminUsersManagement } from './components/admin/admin-users-management/admin-users-management';
import { AdminViewUser } from './components/admin/admin-view-user/admin-view-user';
import { AgentDashboard } from './components/agent/agent-dashboard/agent-dashboard';
import { AgentInquiries } from './components/agent/agent-inquiries/agent-inquiries';
import { AgentLayout } from './components/agent/agent-layout/agent-layout';
import { CreateListing } from './components/agent/create-listing/create-listing';
import { EditListing } from './components/agent/edit-listing/edit-listing';
import { MyListings } from './components/agent/my-listings/my-listings';
import { LoginForm } from './components/auth/login-form/login-form';
import { RegisterAgent } from './components/auth/register-agent/register-agent';
import { RegisterBuyer } from './components/auth/register-buyer/register-buyer';
import { BuyerDashboard } from './components/buyer/buyer-dashboard/buyer-dashboard';
import { BuyerLayout } from './components/buyer/buyer-layout/buyer-layout';
import { MyInquiries } from './components/buyer/my-inquiries/my-inquiries';
import { BrowseListings } from './components/shared/browse-listings/browse-listings';
import { InquiryMessaging } from './components/shared/inquiry-messaging/inquiry-messaging';
import { Profile } from './components/shared/profile/profile';
import { PropertyDetail } from './components/shared/property-detail/property-detail';
import { Unauthorized } from './components/shared/unauthorized/unauthorized';
import { AuthGuard } from './guards/auth.guard';
import { RoleGuard } from './guards/role.guard';
import { Home } from './components/shared/home/home';
import { NoAuthGuard } from './guards/no-auth.guard';
import { Notifications } from './components/shared/notifications/notifications';

export const routes: Routes = [
    {path:'login',component:LoginForm, canActivate:[NoAuthGuard]},
    {path:'',component:LoginForm, canActivate:[NoAuthGuard]},
    
    {path:'register/buyer',component:RegisterBuyer, canActivate:[NoAuthGuard]},
    {path:'register/agent',component:RegisterAgent, canActivate:[NoAuthGuard]},
    {path:'notifications',component:Notifications},
    
    {path:'unauthorized',component:Unauthorized,canActivate:[AuthGuard]},
    
    { path: 'agent', component: AgentLayout,canActivate:[AuthGuard,RoleGuard],data:{roles:['Agent']}, children: [
        { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
        { path: 'dashboard', component: AgentDashboard },
        { path: 'listings', component: MyListings },
        { path: 'browse-listings', component: BrowseListings },
        { path: 'create', component: CreateListing },
        { path: 'property/:id',component:PropertyDetail},
        { path: 'edit/:id',component: EditListing},
        { path: 'inquiries', component: AgentInquiries },
        {path: 'inquiry/:id',component:InquiryMessaging},
        { path: 'profile',component:Profile},
        // {path:'message',component:InquiryMessaging}
        ]
    },
    {path:'admin',component:AdminLayout,canActivate:[AuthGuard,RoleGuard],data:{roles:['Admin']},children:[
            { path: 'dashboard', component: AdminDashboard },
            { path: 'users', component: AdminUsersManagement },
            { path: 'listings', component: AdminListingsManagement },
            { path: 'browse-listings', component: BrowseListings },
            { path: 'create', component: CreateListing },
            { path: 'property/:id',component:PropertyDetail},
            { path: 'listings/edit/:id',component: EditListing},
            {path:'image-cleanup',component:AdminImageCleanup},
            {path:'users/edit/:id',component:AdminEditUser},
            {path:'users/view/:id',component:AdminViewUser},
            { path: 'profile',component:Profile},
            { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
        ]
    },
    {path:'buyer',component:BuyerLayout,canActivate:[AuthGuard,RoleGuard],data:{roles:['Buyer']},children:[
            { path: 'dashboard', component: BuyerDashboard },
            { path: 'browse-listings', component: BrowseListings },
            { path: 'property/:id',component:PropertyDetail},
            { path: 'my-inquiries', component: MyInquiries },
            { path: 'profile' ,component: Profile},
            {path: 'inquiry/:id',component:InquiryMessaging},
            {path:'message',component:InquiryMessaging},
            { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
        ]
    }
];
