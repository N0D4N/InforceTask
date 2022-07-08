import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {TokenService} from '../services/token.service';
import {UserService} from '../services/user.service';
import {LoginRequest} from "../contracts/requests/login-request";
import {FailedResponse} from "../contracts/responses/failed-response";

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

  isLoggedIn = false;
  isLoginFailed = false;
  error: FailedResponse = {errors: []};

  constructor(private userService: UserService, private tokenService: TokenService, private router: Router) {
  }

  ngOnInit(): void {
    let isLoggedIn = this.tokenService.isLoggedIn();
    console.log(`isLoggedIn: ${isLoggedIn}`);
    if (isLoggedIn) {
      this.isLoggedIn = true;

      this.router.navigate(['urls']);
    }
  }

  onSubmit(): void {

    this.userService.login(this.loginRequest).subscribe({
      next: (data => {
        console.debug(`logged in successfully ${data}`);
        this.tokenService.saveSession(data);
        this.isLoggedIn = true;
        this.isLoginFailed = false;
        this.reloadPage();
      }),
      error: ((error: FailedResponse) => {
        this.error = error;
        this.isLoggedIn = false;
        this.isLoginFailed = true;
      })

    });
  }

  reloadPage(): void {
    window.location.reload();
  }


}
