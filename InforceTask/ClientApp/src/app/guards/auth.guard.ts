import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from '@angular/router';
import {catchError, EMPTY, map, Observable} from 'rxjs';

import {TokenService} from '../services/token.service';
import {User} from "../contracts/responses/user";
import {FailedResponse} from "../contracts/responses/failed-response";

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private router: Router, private tokenService: TokenService) {
  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    const session = this.tokenService.getSession();
    if (session == null) {
      this.router.navigate(['/login']);
      return false;
    }

    if (!this.tokenService.isLoggedIn()) {

      console.log(`session is expired, let's renew the tokens`);
      // refresh token
      return this.checkSession(session);
    }
    return true;
  }

  checkSession(session: User): Observable<boolean> {
    return this.tokenService.refreshToken({accessToken: session.tokens.accessToken, refreshToken: session.tokens.refreshToken}).pipe(
      map(data => {
        console.log(`refreshToken response is ${JSON.stringify(data)}`);
        this.tokenService.saveSession(data);
        return true;
      }),
      catchError((error: FailedResponse) => {
        console.log(`inside checkSession ${JSON.stringify(error)}`);
        this.router.navigate(['/login']);
        return EMPTY;
      })
    );

  }

}
