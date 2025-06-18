import { Routes } from '@angular/router';
import { Users } from './users/users';
import { Dashboard } from './dashboard/dashboard';

export const routes: Routes = [
    {path:'',redirectTo: 'users', pathMatch:'full'},
    {path:'users', component: Users},
    {path:'dashboard',component:Dashboard}
];
