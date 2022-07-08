import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import {RouterModule} from '@angular/router';

import {AppComponent} from './app.component';
import {NavMenuComponent} from './nav-menu/nav-menu.component';
import {HomeComponent} from './home/home.component';
import {AuthInterceptorProvider} from "./interceptors/auth.interceptor";
import {FailInterceptorProvider} from "./interceptors/fail.interceptor";
import {UrlsComponent} from './urls/urls.component';
import {LoginComponent} from './login/login.component';
import {UrlInfoComponent} from './url-info/url-info.component';
import {RedirectComponent} from './redirect/redirect.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    UrlsComponent,
    LoginComponent,
    UrlInfoComponent,
    RedirectComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      {path: '', component: HomeComponent, pathMatch: 'full'},
      {path: 'login', component: LoginComponent},
      {path: 'urls', component: UrlsComponent},
      {path: 'url', component: UrlInfoComponent},
      {path: 'rdr', component: RedirectComponent}
    ])
  ],
  providers: [
    AuthInterceptorProvider,
    FailInterceptorProvider
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
