import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { AuthService } from './services/auth.service';
import { authInterceptor } from './interceptors/auth.interceptor';
import { errorInterceptor } from './interceptors/error.interceptor';
import { AuthGuard } from './guards/auth.guard';
import { RoleGuard } from './guards/role.guard';
import { NoAuthGuard } from './guards/no-auth.guard';
import { CategoryService } from './services/category.service';
import { ColorService } from './services/color.service';
import { ModelService } from './services/model.service';
import { NewsService } from './services/news.service';
import { ContactService } from './services/contact.service';
import { ProductService } from './services/product.service';
import { OrderService } from './services/order.service';
import { CartService } from './services/cart.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([authInterceptor,errorInterceptor])
    ),
    AuthGuard,
    RoleGuard,
    NoAuthGuard,
    AuthService,
    CategoryService,
    ColorService,
    ModelService,
    NewsService,
    ContactService,
    ProductService,
    OrderService,
    CartService
  ]
};
