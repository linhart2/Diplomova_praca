import { Component, OnInit } from '@angular/core';
import {AngularFire, FirebaseListObservable} from 'angularfire2';
import {AuthService} from '../providers/auth.service';
import {ActivatedRoute, Router} from '@angular/router';
declare var jquery: any;
declare var $: any;


@Component({
  selector: 'app-edit-exam-page',
  templateUrl: './edit-exam-page.component.html',
  styleUrls: ['./edit-exam-page.component.css']
})
export class EditExamPageComponent implements OnInit {
  errorMessage: {};
  private zoznamCisel = [];
  isLoadedPage: boolean;
  isTeacher: boolean;
  examId: string;
  pid: string;
  private prikladShow: FirebaseListObservable<any[]>;
  createTaskSablona = 0;

  priklad = {};
  prikladJson;

  constructor(private angularFire: AngularFire, public authService: AuthService, private router: Router, private route: ActivatedRoute) {
    this.drawNumberForExam();
    this.setTab(2);
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

  box = null;
  setTab(num: number) {
    this.createTaskSablona = num;
  }

  getBoxId(boxId: string) {
    /* uchovava cislo otvoreneho boxu */
    this.box = boxId;
  }
  appendNumberIntoBox(number) {
    // prida cisla do boxov v trojuholniku
    $("#"+this.box).empty();
    $("<img style='width: 59px;height: 59px; left: -1px;top: -1px;' src=../../assets/img/Number/"+number+".jpg alt=Cislo_"+number+"></a>").appendTo("#"+this.box);
    this.priklad[this.box] = number;
    this.prikladJson = JSON.stringify(this.priklad);
  }

  zobrazPriklad() {
    this.prikladShow.subscribe( snapshot => {
      this.nastavSablonu(snapshot);
      this.vyplnSablonu(snapshot);
    });
  }

  removeChange() {
    this.router.navigate(['/openexam', this.examId]);
  }

  saveChange() {
    let prikladData = this.angularFire.database.object('/EXAMS/' + this.examId + '/priklady/' + this.pid);
    prikladData.set(this.priklad);
    this.errorMessage = this.authService.getErrorMsg('success', { code: 'OwnText', text: 'Zmeny boli úspešne zapísané' } );
    setTimeout(() => {this.router.navigate(['/openexam', this.examId]); }, 2000);
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

  vyplnSablonu(priklad) {
    for (let i = 0; i < priklad.length; i++) {
      $(document).ready(function() {
        $('#' + priklad[i].$key).empty();
        $("<img style='width: 59px;height: 59px; left: -1px;top: -1px;' src=../../assets/img/Number/"+ priklad[i].$value +".jpg alt=Cislo_"+ priklad[i].$value +"></a>").appendTo('#' + priklad[i].$key);
      });
      this.priklad[priklad[i].$key] = priklad[i].$value;
    }
  }

  drawNumberForExam() {
    //Vyplni modal content Cislami od 1 do 100;
    let num = 1;
    for (let i = 0; i < 10; i++) {
      let row = [];
      for (let j = 0; j < 10; j++) {
        row.push(num);
        num++;
      }
      this.zoznamCisel.push(row);
    }
  }

  ngOnInit() {
  }

}
