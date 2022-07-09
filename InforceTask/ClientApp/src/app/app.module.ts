import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import {RouterModule} from '@angular/router';

import {AppComponent} from './app.component';
import {NavMenuComponent} from './nav-menu/nav-menu.component';
import {AboutComponent} from './about/about.component';
import {AuthInterceptorProvider} from "./interceptors/auth.interceptor";
import {FailInterceptorProvider} from "./interceptors/fail.interceptor";
import {UrlsComponent} from './urls/urls.component';
import {LoginComponent} from './login/login.component';
import {UrlInfoComponent} from './url-info/url-info.component';
import {RedirectComponent} from './redirect/redirect.component';
import {AutosizeModule} from "ngx-autosize";
import {AuthGuard} from "./guards/auth.guard";

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    AboutComponent,
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
      {path: '', component: AboutComponent, pathMatch: 'full'},
      {path: 'login', component: LoginComponent},
      {path: 'urls', component: UrlsComponent},
      {path: 'url', component: UrlInfoComponent, canActivate: [AuthGuard]},
      {path: 'rdr', component: RedirectComponent}
    ]),
    AutosizeModule
  ],
  providers: [
    AuthInterceptorProvider,
    FailInterceptorProvider
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
