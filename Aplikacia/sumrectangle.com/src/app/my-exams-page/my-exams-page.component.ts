import { Component, OnInit } from '@angular/core';
import {AngularFire, FirebaseListObservable} from 'angularfire2';
import {Router} from '@angular/router';
import {AuthService} from '../providers/auth.service';

@Component({
  selector: 'app-my-exams-page',
  templateUrl: './my-exams-page.component.html',
  styleUrls: ['./my-exams-page.component.css']
})
export class MyExamsPageComponent implements OnInit {
  errorMessage: {};
  isLoadedPage: boolean;
  fName: string;
  lName: string;
  isTeacher: boolean;
  uId: string;

  private items: FirebaseListObservable<any[]>;
  public zoznamUloh: Array<any> = [];

  constructor(private angularFire: AngularFire, public authService: AuthService, private router: Router) {
    this.isLoadedPage = false;
    this.authService.angularFire.auth.subscribe(
      (auth) => {
        if (auth !== null) {
          this.items = angularFire.database.list('/EXAMS');
          this.zobrazUlohy();
          let users = this.angularFire.database.object('/USERS/' + auth.uid);
          users.subscribe(snapshot => {
            this.isLoadedPage = true;
            this.uId = auth.uid;
            this.fName = snapshot.firstName;
            this.lName = snapshot.lastName;
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
  public zobrazUlohy() {
    this.items.subscribe( snapshot => {
      let items = [];
      snapshot.forEach( item => {
        if ( item.teacherID === this.uId) {
          items.push(item);
        }
      });
      this.zoznamUloh = items;
    });
  }
  public deleteExam(id: string) {
    if (this.authService.PopupAlert('úlohu')) {
      let _uloha = this.angularFire.database.object('/EXAMS/' + id);
      _uloha.remove();
    }
  }

  ngOnInit() {
  }

}
