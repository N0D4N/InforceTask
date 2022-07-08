import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {UrlService} from "../services/url.service";

@Component({
  selector: 'app-redirect',
  templateUrl: './redirect.component.html',
  styleUrls: ['./redirect.component.css']
})
export class RedirectComponent implements OnInit {

  id: string = '';

  constructor(private route: ActivatedRoute, private urlService: UrlService, private router: Router) {
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.id = params['id'];
    })
    this.urlService.getShortenedUrlById(this.id).subscribe((res) => {
      document.location.href = res.data.destination;
      console.log(res.data.destination);
    });
  }

}
