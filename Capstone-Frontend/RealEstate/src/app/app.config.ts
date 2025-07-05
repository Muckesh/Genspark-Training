import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './interceptor/auth.interceptor';
import { errorInterceptor } from './interceptor/error.interceptor';
import { AuthGuard } from './guards/auth.guard';
import { AgentService } from './services/agent.service';
import { AuthService } from './services/auth.service';
import { BuyerService } from './services/buyer.service';
import { ImageCleanupService } from './services/image-cleanup.service';
import { InquiryService } from './services/inquiry.service';
import { ModalService } from './services/modal.service';
import { PropertyImageService } from './services/property-image.service';
import { PropertyListingService } from './services/property-listing.service';
import { SignalRService } from './services/signalr.service';
import { TokenService } from './services/token.service';
import { UserService } from './services/user.service';
import { RoleGuard } from './guards/role.guard';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([authInterceptor,errorInterceptor])
    ),
    SignalRService,
    AuthService,
    TokenService,
    ModalService,
    AgentService,
    BuyerService,
    ImageCleanupService,
    InquiryService,
    PropertyImageService,
    PropertyListingService,
    UserService,
    AuthGuard,
    RoleGuard
  ]
};
