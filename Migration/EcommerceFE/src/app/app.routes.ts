import { Routes } from '@angular/router';
import { Login } from './components/auth/login/login';
import { NoAuthGuard } from './guards/no-auth.guard';
import { AuthGuard } from './guards/auth.guard';
import { Home } from './components/shared/home/home';
import { Dashboard } from './components/admin/dashboard/dashboard';
import { RoleGuard } from './guards/role.guard';
import { Category } from './components/admin/category/category';
import { Color } from './components/admin/color/color';
import { Layout } from './components/shared/layout/layout';
import { Model } from './components/admin/model/model';
import { News } from './components/admin/news/news';
import { Contactus } from './components/shared/contactus/contactus';
import { Product } from './components/admin/product/product';
import { Order } from './components/shared/order/order';
import { Cart } from './components/shared/cart/cart';
import { Checkout } from './components/shared/checkout/checkout';
import { Unauthorized } from './components/shared/unauthorized/unauthorized';
import { NewsList } from './components/shared/news-list/news-list';

export const routes: Routes = [
    {path:'login',component:Login, canActivate:[NoAuthGuard]},
    { path: '', component: Layout , canActivate:[AuthGuard], children:[
        { path: '', component: Home },
        { path: 'categories', component: Category },
        {path:'colors',component:Color},
        {path:'models',component:Model},
        {path:'news',component:NewsList},
        {path:'news-management',component:News},
        {path:'contact-us',component:Contactus},
        {path:'products',component:Product},
        {path:'orders',component:Order},
        {path:'payment-success',component:Order},  
        {path:'payment-cancel',component:Order},    
  
        {path:'cart',component:Cart},
        {path:'checkout',component:Checkout}
    ]},
    {path:'unauthorized',component:Unauthorized},
    { path: 'home', component: Home },
    // { path: 'categories', component: Category },
    // { path: 'categories/create', component: CategoryCreateComponent },
    // { path: 'categories/edit/:id', component: CategoryEditComponent },
    // { path: 'categories/details/:id', component: CategoryDetailsComponent },
    {path:'admin',component:Dashboard,canActivate:[AuthGuard,RoleGuard],data:{roles:['Admin']},children:[
        {path:'categories',component:Category},
    ]},
    // {path:'categories',component:Category},
    // {path:'colors',component:Color}
    { path: '**', redirectTo: '' }
    
];
