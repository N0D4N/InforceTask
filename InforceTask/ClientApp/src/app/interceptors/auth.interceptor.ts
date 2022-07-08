import {Injectable} from '@angular/core';
import {HTTP_INTERCEPTORS, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Observable} from 'rxjs';
import {environment} from "../../environments/environment";
import {TokenService} from "../services/token.service";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private tokenService: TokenService) {
  }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const requestingApis = request.url.startsWith(environment.apiUrl);
    const isLoggedIn = this.tokenService.isLoggedIn();

    if (isLoggedIn && requestingApis) {
      const session = this.tokenService.getSession();
      if (session) {
        request = request.clone({headers: request.headers.set('Authorization', `Bearer ${session.tokens.accessToken}`)});
      }

    }
    return next.handle(request);
  }
}

export const AuthInterceptorProvider = {provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true};
