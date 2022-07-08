import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {AllShortenedUrlsResponse} from "../contracts/responses/all-shortened-urls-response";
import {environment} from "../../environments/environment";
import {ShortenedUrlResponse} from "../contracts/responses/shortened-url-response";
import {CreateShortenedUrlResponse} from "../contracts/responses/create-shortened-url-response";
import {CreateShortenedUrlRequest} from '../contracts/requests/create-shortened-url-request';
import {DeleteUrlRequest} from "../contracts/requests/delete-url-request";

@Injectable({
  providedIn: 'root'
})
export class UrlService {

  constructor(private httpClient: HttpClient) {
  }

  getAllShortenedUrls(): Observable<AllShortenedUrlsResponse> {
    return this.httpClient.get<AllShortenedUrlsResponse>(`${environment.apiUrl}/Urls/all`);
  }

  getShortenedUrlById(id: string): Observable<ShortenedUrlResponse> {
    return this.httpClient.get<ShortenedUrlResponse>(`${environment.apiUrl}/Urls/url/${id}`);
  }

  createShortenedUrl(request: CreateShortenedUrlRequest): Observable<CreateShortenedUrlResponse> {
    return this.httpClient.post<CreateShortenedUrlResponse>(`${environment.apiUrl}/Urls/url`, request);
  }

  deleteShortenedUrl(request: DeleteUrlRequest) {
    return this.httpClient.delete(`${environment.apiUrl}/Urls/url/${request.id}`);
  }
}
