import { Injectable } from '@angular/core';
import {TokenService} from "./token.service";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {AboutPageContentsResponse} from "../contracts/responses/about-page-contents-response";
import {environment} from "../../environments/environment";
import {EditAboutPageContentRequest} from "../contracts/requests/edit-about-page-content-request";

@Injectable({
  providedIn: 'root'
})
export class AboutService {

  constructor(private tokenService: TokenService, private httpClient: HttpClient) { }

  isEditable():boolean{
    return this.tokenService.getSession()?.user.isAdmin ?? false;
  }

  getCurrentAboutPageContents(): Observable<AboutPageContentsResponse>{
    return this.httpClient.get<AboutPageContentsResponse>(`${environment.apiUrl}/AboutPage/current`);
  }

  updateAboutPageContents(content: EditAboutPageContentRequest): Observable<AboutPageContentsResponse>{
    if(!this.isEditable()){
      return this.getCurrentAboutPageContents();
    }
    else {
      return this.httpClient.post<AboutPageContentsResponse>(`${environment.apiUrl}/AboutPage/edit`, content);
    }
  }
}
