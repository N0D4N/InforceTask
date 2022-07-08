import {Component, OnInit} from '@angular/core';
import {TokenService} from "../services/token.service";
import {UrlService} from "../services/url.service";
import {map, Observable} from "rxjs";
import {ShortenedUrl} from "../contracts/responses/shortened-url";

@Component({
  selector: 'app-urls',
  templateUrl: './urls.component.html',
  styleUrls: ['./urls.component.css']

})
export class UrlsComponent implements OnInit {

  isLoggedIn: boolean = false;
  urls$: Observable<ShortenedUrl[]> | undefined;
  newUrl: string = '';
  userId: string = '';
  isAdmin: boolean = false;

  constructor(private tokenService: TokenService, private urlService: UrlService) {
  }

  static isValidUrl(destination: string) {
    try {
      const url = new URL(destination);
      return url.protocol.startsWith("http");

    } catch (_) {
      return false;
    }
  }

  ngOnInit(): void {
    this.isLoggedIn = this.tokenService.isLoggedIn();
    this.urls$ = this.urlService.getAllShortenedUrls().pipe(map(v => v.data));
    const user = this.tokenService.getSession()?.user;
    if (user) {
      this.userId = user.id!;
      this.isAdmin = user.isAdmin!;
    }
  }

  deleteUrl(shortenedUrl: ShortenedUrl) {
    this.urlService.deleteShortenedUrl({id: shortenedUrl.id}).subscribe(() => this.urls$ = this.urlService.getAllShortenedUrls().pipe(map(v => v.data)));
  }

  createUrl() {
    if (this.newUrl && UrlsComponent.isValidUrl(this.newUrl)) {
      this.urlService.createShortenedUrl({destination: this.newUrl}).subscribe(() => this.urls$ = this.urlService.getAllShortenedUrls().pipe(map(v => v.data)));
    }
  }
}
