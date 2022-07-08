import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {LoginRequest} from "../contracts/requests/login-request";
import {Observable} from "rxjs";
import {TokenResponse} from "../contracts/responses/token-response";
import {environment} from "../../environments/environment";
import {RefreshTokenRequest} from "../contracts/requests/refresh-token-request";

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClient: HttpClient) {
  }

  login(loginRequest: LoginRequest): Observable<TokenResponse> {
    return this.httpClient.post<TokenResponse>(`${environment.apiUrl}/User/login`, loginRequest);
  }

  refreshToken(request: RefreshTokenRequest): Observable<TokenResponse> {
    return this.httpClient.post<TokenResponse>(`${environment.apiUrl}/User/refresh`, request);
  }
}
