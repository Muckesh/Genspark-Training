import { Routes } from '@angular/router';
import { FileUploadComponent } from './file-upload-component/file-upload-component';
import { Products } from './products/products';

export const routes: Routes = [
    {path:'file',component:FileUploadComponent},
    {path:'products',component:Products},
];
