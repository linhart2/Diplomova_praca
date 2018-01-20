import { Component, OnInit } from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthService} from '../providers/auth.service';
import {AngularFire, FirebaseListObservable} from 'angularfire2';
declare var jquery: any;
declare var $: any;

@Component({
  selector: 'app-show-exam-page',
  templateUrl: './show-exam-page.component.html',
  styleUrls: ['./show-exam-page.component.css']
})
export class ShowExamPageComponent implements OnInit {
  errorMessage: {};
  isLoadedPage: boolean;
  isTeacher: boolean;
  examId: string;
  pid: string;
  private prikladShow: FirebaseListObservable<any[]>;
  createTaskSablona = 0;

  constructor(private angularFire: AngularFire, public authService: AuthService, private router: Router, private route: ActivatedRoute) {
    this.route.params.subscribe(params => {
      this.examId = params['id'];
      this.pid = params['pid'];
    });
    this.isLoadedPage = false;
    this.authService.angularFire.auth.subscribe(
      (auth) => {
        if (auth !== null) {
          this.prikladShow = this.angularFire.database.list('/EXAMS/' + this.examId + '/priklady/' + this.pid);
          this.zobrazPriklad();
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

  setTab(num: number) {
    this.createTaskSablona = num;
  }
  zobrazPriklad() {
    this.prikladShow.subscribe( snapshot => {
      this.nastavSablonu(snapshot);
      this.vyplnSablonu(snapshot);
    });
  }

  vyplnSablonu(priklad) {
    for (let i = 0; i < priklad.length; i++) {
      $(document).ready(function() {
        $('#' + priklad[i].$key).empty();
        $("<img style='width: 59px;height: 59px; left: -1px;top: -1px;' src=../../assets/img/Number/"+ priklad[i].$value +".jpg alt=Cislo_"+ priklad[i].$value +"></a>").appendTo('#' + priklad[i].$key);
      });
    }
  }

  public nastavSablonu(obj) {
    switch (obj.length) {
      case 3:
        this.setTab(2);
        break;
      case 6:
        this.setTab(3);
        break;
      case 10:
        this.setTab(4);
        break;
    }
  }


  ngOnInit() {
  }

}
