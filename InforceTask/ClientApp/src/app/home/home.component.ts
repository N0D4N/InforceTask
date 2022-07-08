import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {AboutPageContentsResponse} from "../contracts/responses/about-page-contents-response";
import {environment} from "../../environments/environment";
import {Observable, of} from "rxjs";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit{

  response: AboutPageContentsResponse = {
    content: '',
    editNumber: 1,
    LastEditDateTime: new Date()
  }
  constructor(private httpClient: HttpClient) {
  }

  ngOnInit(): void {
    this.httpClient.get<AboutPageContentsResponse>(`${environment.apiUrl}/AboutPage/current`).subscribe(res => {
       this.response = res;
    });
  }
}
