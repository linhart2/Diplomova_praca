import { Component, OnInit } from '@angular/core';
import {AngularFire, FirebaseListObservable} from 'angularfire2';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthService} from '../providers/auth.service';

@Component({
  selector: 'app-open-class-page',
  templateUrl: './open-class-page.component.html',
  styleUrls: ['./open-class-page.component.css']
})
export class OpenClassPageComponent implements OnInit {
  errorMessage: {};
  isLoadedPage: boolean;
  isTeacher: boolean;
  classId: string;
  className: string;
  classPasswd: string;

  studentInClass: FirebaseListObservable<any[]>;
  public zoznamZiakov: Array<any> = [];


  constructor(private angularFire: AngularFire, public authService: AuthService, private router: Router, private route: ActivatedRoute) {
    this.route.params.subscribe(params => {
      this.classId = params['id'];
    });
    this.isLoadedPage = false;
    this.authService.angularFire.auth.subscribe(
      (auth) => {
        if (auth !== null) {
          let ClassInfo = angularFire.database.object('/CLASSES/' + this.classId );
          ClassInfo.subscribe(snap => {
            this.className = snap.className;
            this.classPasswd = snap.heslo;
          });
          this.studentInClass = angularFire.database.list('/CLASSES/' + this.classId + '/STUDENTS_IN_CLASS');
          this.zobrazZiakov();
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

  public zobrazZiakov() {
    this.studentInClass.subscribe( snapshot => {
      let items = [];
      snapshot.forEach( item => {
          items.push(item);
      });
      this.zoznamZiakov = items;
    });
  }

  ngOnInit() {
  }

}
