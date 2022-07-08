import {Injectable} from '@angular/core';
import {HTTP_INTERCEPTORS, HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {catchError, EMPTY, Observable, tap, throwError} from 'rxjs';
import {TokenService} from '../services/token.service';
import {UserService} from '../services/user.service';
import {Router} from '@angular/router';
import {RefreshTokenRequest} from "../contracts/requests/refresh-token-request";
import {FailedResponse} from "../contracts/responses/failed-response";

@Injectable()
export class FailInterceptor implements HttpInterceptor {

  isRefreshingToken: boolean = false;

  constructor(private tokenService: TokenService, private userService: UserService, private router: Router) {
  }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request)
      .pipe(
        tap(response => console.log(JSON.stringify(response))),
        catchError((error: HttpErrorResponse) => {

          console.log(JSON.stringify(error));
          const session = this.tokenService.getSession();
          if (error.status === 401 && session != null && !this.tokenService.isLoggedIn() && !this.isRefreshingToken) {
            this.isRefreshingToken = true;
            console.log('Access Token is expired, we need to renew it');
            const refreshRequest: RefreshTokenRequest = {
              refreshToken: session.tokens.refreshToken,
              accessToken: session.tokens.accessToken
            }
            this.userService.refreshToken(refreshRequest).subscribe({
              next: data => {
                console.info('Tokens renewed, we will save them into the local storage');
                this.tokenService.saveSession(data);
              },
              error: () => {
              },
              complete: () => {
                this.isRefreshingToken = false
              }
            });
          } else if (error.status === 400 && error.error.errorCode === 'invalid_grant') {
            console.log('the refresh token has expired, the user must login again');
            this.tokenService.logout();
            this.router.navigate(['login']);


          } else {
            let errorResponse: FailedResponse = error.error;
            console.log(JSON.stringify(errorResponse));

            return throwError(() => errorResponse);
          }

          return EMPTY;
        })
      );
  }
}

export const FailInterceptorProvider = {provide: HTTP_INTERCEPTORS, useClass: FailInterceptor, multi: true};
