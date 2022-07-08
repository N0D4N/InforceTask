import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {UrlService} from "../services/url.service";
import {ShortenedUrl} from "../contracts/responses/shortened-url";
import {TokenService} from "../services/token.service";

@Component({
  selector: 'app-url-info',
  templateUrl: './url-info.component.html',
  styleUrls: ['./url-info.component.css']
})
export class UrlInfoComponent implements OnInit {

  id: string = '';
  urlInfo: ShortenedUrl = {
    id: '',
    destination: '',
    creationDateUnixTimestampInSeconds: -1,
    creator: {
      id: '',
      username: '',
      isAdmin: false
    }
  };
  creationDate: string = '';
  isLoggedIn: boolean = false;

  constructor(private route: ActivatedRoute, private urlService: UrlService, private tokenService: TokenService) {
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.id = params['id'];
    })
    this.urlService.getShortenedUrlById(this.id).subscribe((res) => {
      this.urlInfo = res.data;
      this.creationDate = new Date(this.urlInfo.creationDateUnixTimestampInSeconds).toString();
    });
    this.isLoggedIn = this.tokenService.isLoggedIn();
  }

}
