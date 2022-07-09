import {Injectable} from '@angular/core';
import {UserService} from "./user.service";
import {TokenResponse} from "../contracts/responses/token-response";
import {User} from "../contracts/responses/user";
import {UserResponse} from "../contracts/responses/user-response";
import {Observable} from "rxjs";
import {RefreshTokenRequest} from "../contracts/requests/refresh-token-request";

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  constructor(private userService: UserService) {
  }

  static getJwt(accessToken: string): any {
    const body = accessToken.split('.')[1];
    return JSON.parse(atob(body));

  }

  static getUserFromAccessToken(accessToken: string): UserResponse {
    const json = this.getJwt(accessToken);
    const user: UserResponse = {
      username: json.Username,
      id: json.sub,
      isAdmin: json.Admin == "True"
    };
    return user;
  }

  saveSession(tokenResponse: TokenResponse) {
    window.localStorage.setItem('AT', tokenResponse.data.accessToken);
    window.localStorage.setItem('RT', tokenResponse.data.refreshToken);
  }

  getSession(): User | null {
    const storedAccessToken: string | null = window.localStorage.getItem('AT');

    if (storedAccessToken) {
      const user: User = {
        tokens: {
          accessToken: storedAccessToken!,
          refreshToken: window.localStorage.getItem('RT')!
        },
        user: TokenService.getUserFromAccessToken(storedAccessToken)
      }
      return user;
    }
    return null;
  }

  isLoggedIn(): boolean {
    const session = this.getSession();
    if (!session) {
      return false;
    }

    const jwt = TokenService.getJwt(session.tokens.accessToken);
    const expired = Date.now() > jwt.exp * 1000;
    return !expired;
  }

  refreshToken(request: RefreshTokenRequest): Observable<TokenResponse> {
    return this.userService.refreshToken(request);
  }

  logout() {
    window.localStorage.clear();
  }
}
