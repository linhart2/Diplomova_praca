import {Component, OnInit} from '@angular/core';

import {Router} from '@angular/router';
import {AuthService} from '../providers/auth.service';
import {AngularFire} from 'angularfire2';

@Component({
    selector: 'app-login-page',
    templateUrl: './login-page.component.html',
    styleUrls: ['./login-page.component.css']
})
export class LoginPageComponent implements OnInit {
    errorMessage: {};
    isLoadedPage: boolean;
    constructor(private angularFire: AngularFire, public authService: AuthService, private router: Router) {
      this.isLoadedPage = false;
      this.authService.angularFire.auth.subscribe(
        (auth) => {
          if (auth !== null) {
            let users = this.angularFire.database.object('/USERS/' + auth.uid);
            users.subscribe(snapshot => {
              if (snapshot.isTeacher) {
                this.router.navigate(['classes']);
              }else {
                this.router.navigate(['ssss']);
              }
            });
          }
          this.isLoadedPage = true;
        }
      );
        this.errorMessage = {};
    }

    ngOnInit() {
    }

    login(userEmail: string, userPassword: string) {
        this.authService.loginWithEmailAndPassword(userEmail, userPassword).then((data) => {
            let users = this.angularFire.database.object('/USERS/' + data.uid);
        }).catch((error) => {
          this.errorMessage = this.authService.getErrorMsg('error', error);
        });
    }
}
