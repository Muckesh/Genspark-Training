import { Routes } from '@angular/router';
import { Login } from './login/login';
import { DashboardLogin } from './dashboard-login/dashboard-login';

export const routes: Routes = [
    {path:"",component:Login},
    {path:"login-dashboard",component:DashboardLogin}
];
