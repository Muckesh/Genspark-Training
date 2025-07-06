import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class NoAuthGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean {
    if (this.authService.isAuthenticated()) {
      // Redirect to home or role-specific dashboard
      const role = this.authService.getCurrentUser()?.role;
      switch (role) {
        case 'Admin':
          this.router.navigate(['/admin']);
          break;
        case 'Agent':
          this.router.navigate(['/agent']);
          break;
        case 'Buyer':
          this.router.navigate(['/buyer']);
          break;
        default:
          this.router.navigate(['/']);
      }
      return false;
    }
    return true;
  }
}
