import { Routes } from '@angular/router';
import { LoginForm } from './auth/components/login-form/login-form';
import { Home } from './home/home';
import { RegisterBuyer } from './register-buyer/register-buyer';
import { RegisterAgent } from './register-agent/register-agent';
import { AuthGuard } from './guards/auth.guard';
import { RoleGuard } from './guards/role.guard';
import { Unauthorized } from './unauthorized/unauthorized';
import { BuyerDashboard } from './buyer-dashboard/buyer-dashboard';
import { MyInquiries } from './my-inquiries/my-inquiries';
import { PropertyDetail } from './property-detail/property-detail';
import { Profile } from './profile/profile';
import { AgentDashboard } from './agent/agent-dashboard/agent-dashboard';
import { MyListings } from './agent/my-listings/my-listings';
import { CreateListing } from './agent/create-listing/create-listing';
import { AgentInquiries } from './agent/agent-inquiries/agent-inquiries';
import { EditListing } from './agent/edit-listing/edit-listing';
import { AdminDashboard } from './admin-dashboard/admin-dashboard';
import { AdminLayout } from './admin-layout/admin-layout';
import { AdminListingsManagement } from './admin-listings-management/admin-listings-management';
import { BuyerLayout } from './buyer-layout/buyer-layout';
import { AdminUsersManagement } from './admin-users-management/admin-users-management';
import { BrowseListings } from './browse-listings/browse-listings';
import { AgentLayout } from './agent/agent-layout/agent-layout';
import { InquiryMessaging } from './inquiry-messaging/inquiry-messaging';
import { AdminImageCleanup } from './admin-image-cleanup/admin-image-cleanup';

export const routes: Routes = [
    {path:'login',component:LoginForm},
    // {path:'',component:Home,canActivate:[AuthGuard,RoleGuard],data:{roles:['Agent']}},
    {path:'',component:Home,canActivate:[AuthGuard,RoleGuard],data:{roles:['Agent','Admin','Buyer']}},
    {path:'register/buyer',component:RegisterBuyer},
    {path:'register/agent',component:RegisterAgent},
    {path:'unauthorized',component:Unauthorized,canActivate:[AuthGuard]},
    {path:'dashboard/buyer',component:BuyerDashboard,canActivate:[AuthGuard,RoleGuard],data:{roles:['Buyer']}},
    {path:'my-inquiries',component:MyInquiries,canActivate:[AuthGuard,RoleGuard],data:{roles:['Buyer']}},
    {path:'property/:id',component:PropertyDetail, canActivate:[AuthGuard,RoleGuard],data:{roles:['Admin','Agent','Buyer']},},
    {path:'profile',component:Profile,canActivate:[AuthGuard]},
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
            { path: 'listings/edit/:id',component: EditListing},
            {path:'image-cleanup',component:AdminImageCleanup},
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
