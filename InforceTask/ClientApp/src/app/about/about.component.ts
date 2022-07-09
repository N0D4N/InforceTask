import {Component, Input, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {AboutPageContentsResponse} from "../contracts/responses/about-page-contents-response";
import {environment} from "../../environments/environment";
import {Observable, of} from "rxjs";
import {AboutService} from "../services/about.service";

@Component({
  selector: 'app-about',
  templateUrl: './about.component.html',
})
export class AboutComponent implements OnInit {

  @Input() content: string = '';
  editing: boolean = false;
  editNumber: number = 1;
  lastEditDateTime: Date = new Date();

  constructor(private aboutService: AboutService) {
  }

  ngOnInit(): void {
    this.applyCurrentAboutPage();
  }

  applyCurrentAboutPage(): void {
    this.aboutService.getCurrentAboutPageContents().subscribe(res =>
      this.setStateFromResponse(res)
    );
  }

  updateCurrentAboutPage(): void {
    this.aboutService.updateAboutPageContents({content: this.content}).subscribe(
      res => this.setStateFromResponse(res)
    );
  }

  setStateFromResponse(response: AboutPageContentsResponse): void {
    this.content = response.content;
    this.editNumber = response.editNumber;
    this.lastEditDateTime = response.lastEditDateTime;
  }

  isEditable(): boolean {
    return this.aboutService.isEditable();
  }

  setEditing(): void{
    if(this.isEditable()){
      this.editing = true;
    }
  }

}
