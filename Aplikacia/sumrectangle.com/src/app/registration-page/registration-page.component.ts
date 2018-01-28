import { Component, OnInit } from '@angular/core';
import {Router} from '@angular/router';
import {AuthService} from '../providers/auth.service';
import {AngularFire} from 'angularfire2';

@Component({
  selector: 'app-registration-page',
  templateUrl: './registration-page.component.html',
  styleUrls: ['./registration-page.component.css']
})
export class RegistrationPageComponent implements OnInit {
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

  register(userEmail: string, userPassword: string, userPassword2: string, userFirstName: string, userLastName: string) {

    if ( userEmail === '' || userEmail === null && userPassword === '' || userPassword === null &&
      userPassword2 === '' || userPassword2 === null && userFirstName === '' || userFirstName === null &&
      userLastName === '' || userLastName === null
    ) {
      this.errorMessage = this.authService.getErrorMsg('error', { code: 'registraciaPovinneUdaje' } );
    }else if ( userPassword !== userPassword2 ) {
      this.errorMessage = this.authService.getErrorMsg('error', { code: 'registraciaNezhodneHesla' } );
    }else if ( userPassword.length < 6 ) {
      this.errorMessage = this.authService.getErrorMsg('error', { code: 'registraciaMinDlzkaHesla' } );
    }else {
      this.authService.registerUserWithEmailAndPassword(userEmail, userPassword, userFirstName, userLastName).then(() => {
        this.router.navigate(['classes']);
      }).catch((error) => {
        this.errorMessage = this.authService.getErrorMsg('error', error);
      });
    }
  }
}
