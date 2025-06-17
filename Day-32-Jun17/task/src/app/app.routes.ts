import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Products } from './products/products';
import { Product } from './product/product';
import { AuthGuard } from './auth-guard';

export const routes: Routes = [
    {path: 'login', component:Login},
    {
        path:'products',
        canActivate: [AuthGuard],
        children:[
            {path:'',component:Products},
            {path:':id',component:Product}
        ]
    },
    { path: '', redirectTo: 'products', pathMatch: 'full' },
    { path: '**', redirectTo: 'products' }
];
