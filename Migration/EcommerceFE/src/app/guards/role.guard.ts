import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from "@angular/router";
import { AuthService } from "../services/auth.service";

@Injectable()
export class RoleGuard implements CanActivate{
    constructor(private authService:AuthService,private router:Router){}

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        const expectedRoles = route.data['roles'] as Array<string>;
        const user = this.authService.getCurrentUser();
        console.log(`User role: ${user?.role}, Expected roles: ${expectedRoles}`);
        
        if(user && expectedRoles.includes(user.role)){
            return true;
        }

        this.router.navigate(['unauthorized']);
        return false;
    }
}