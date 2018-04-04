import { Component, OnInit } from '@angular/core';
import {AngularFire} from 'angularfire2';
import {AuthService} from '../providers/auth.service';
import {ActivatedRoute, Router} from '@angular/router';

@Component({
  selector: 'app-student-statistics-page',
  templateUrl: './student-statistics-page.component.html',
  styleUrls: ['./student-statistics-page.component.css']
})
export class StudentStatisticsPageComponent implements OnInit {
  errorMessage: {};
  isLoadedPage: boolean;
  isTeacher: boolean;
  sid: string;

  constructor(private angularFire: AngularFire, public authService: AuthService, private router: Router, private route: ActivatedRoute) {
    this.route.params.subscribe(params => {
      this.sid = params['sid'];
    });
    this.isLoadedPage = false;
    this.authService.angularFire.auth.subscribe(
      (auth) => {
        if (auth !== null) {
          let users = this.angularFire.database.object('/USERS/' + auth.uid);
          users.subscribe(snapshot => {
            this.isLoadedPage = true;
            this.isTeacher = snapshot.isTeacher;
            if (!snapshot.isTeacher) {
              this.router.navigate(['ssss']);
            }
          });
        }
      }
    );
    this.errorMessage = {};
  }

  ngOnInit() {
  }

}
