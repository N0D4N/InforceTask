import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {TokenService} from '../services/token.service';
import {UserService} from '../services/user.service';
import {LoginRequest} from "../contracts/requests/login-request";
import {FailedResponse} from "../contracts/responses/failed-response";
import {Form, FormBuilder, FormGroup} from "@angular/forms";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  loginRequest: LoginRequest = {
    username: "",
    password: "",
    shouldBeAdmin: false
  };

  error: FailedResponse = {errors: []};

  constructor(private userService: UserService, private tokenService: TokenService, private router: Router) {
  }

  ngOnInit(): void {
    let isLoggedIn = this.tokenService.isLoggedIn();
    console.log(`isLoggedIn: ${isLoggedIn}`);
    if (isLoggedIn) {
      this.router.navigate(['/']);
    }
  }

  onSubmit(form: FormGroup): void {

    this.userService.login(this.loginRequest).subscribe({
      next: (data => {
        console.debug(`logged in successfully ${data}`);
        this.tokenService.saveSession(data);
        this.reloadPage();
      }),
      error: ((error: FailedResponse) => {
        this.error = error;
      })

    });
  }

  reloadPage(): void {
    window.location.reload();
  }


}
