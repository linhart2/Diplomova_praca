import {Component, Input} from '@angular/core';
import { AngularFire, FirebaseListObservable } from 'angularfire2';
import { Router } from '@angular/router';
import { AuthService } from './providers/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent {
  errorMessage: {};
  isLoadedPage: boolean;
  fName: string;
  lName: string;
  isTeacher: boolean;
  isLogged: boolean;
  uId: string;

  mainHeader: boolean;

  constructor(private angularFire: AngularFire, public authService: AuthService, private router: Router) {
    this.mainHeader = false;
    this.isLogged = false;
    this.isLoadedPage = false;
    this.authService.angularFire.auth.subscribe(
      (auth) => {
        if (auth !== null) {
          this.isLogged = true;
          let users = this.angularFire.database.object('/USERS/' + auth.uid);
          users.subscribe(snapshot => {
            this.isLoadedPage = true;
            this.uId = auth.uid;
            this.fName = snapshot.firstName;
            this.lName = snapshot.lastName;
            this.isTeacher = snapshot.isTeacher;
            if (snapshot.isTeacher) {
              this.isLogged = true;
              this.mainHeader = true;
            }else {
              this.isLogged = true;
              this.mainHeader = false;
            }
          });
        }
      }
    );
  }

  logout() {
    this.authService.logout();
    this.authService.angularFire.auth.subscribe(auth => {
      if ( auth === null ) {
        this.isLogged = false;
        this.mainHeader = false;
        this.router.navigate(['login']);
      }
    });
  }


}
