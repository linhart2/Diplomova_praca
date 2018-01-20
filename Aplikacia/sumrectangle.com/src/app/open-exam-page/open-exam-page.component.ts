import { Component, OnInit } from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthService} from '../providers/auth.service';
import {AngularFire, FirebaseListObservable} from 'angularfire2';

@Component({
  selector: 'app-open-exam-page',
  templateUrl: './open-exam-page.component.html',
  styleUrls: ['./open-exam-page.component.css']
})
export class OpenExamPageComponent implements OnInit {
  errorMessage: {};
  isLoadedPage: boolean;
  isTeacher: boolean;
  examId: string;
  examName: string;

  prikladyVUlohe: FirebaseListObservable<any[]>;
  public zoznamPrikladov: Array<any> = [];


  constructor(private angularFire: AngularFire, public authService: AuthService, private router: Router, private route: ActivatedRoute) {
    this.route.params.subscribe(params => {
      this.examId = params['id'];
    });
    this.isLoadedPage = false;
    this.authService.angularFire.auth.subscribe(
      (auth) => {
        if (auth !== null) {
          let examInfo = angularFire.database.object('/EXAMS/' + this.examId );
          examInfo.subscribe(snap => {
            this.examName = snap.nazovUlohy;
          });
          this.prikladyVUlohe = angularFire.database.list('/EXAMS/' + this.examId + '/priklady');
          this.zobrazPriklady();
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

  public zobrazPriklady() {
    this.prikladyVUlohe.subscribe( snapshot => {
      let items = [];
      snapshot.forEach( item => {
        items.push(item);
      });
      this.zoznamPrikladov = items;
    });
  }

  public deletePriklad(id: string) {
    if (this.authService.PopupAlert('príklad')) {
      let _uloha = this.angularFire.database.object('/EXAMS/' + this.examId + '/priklady/' + id);
      _uloha.remove();
    }
  }

  ngOnInit() {
  }

}
