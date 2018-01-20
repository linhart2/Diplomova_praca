import { Component, OnInit } from '@angular/core';
import {Uloha} from '../create-exam-page/Uloha';
import {AngularFire, FirebaseListObservable} from 'angularfire2';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthService} from '../providers/auth.service';
declare var jquery: any;
declare var $: any;

@Component({
  selector: 'app-create-exam-page',
  templateUrl: './create-exam-page.component.html',
  styleUrls: ['./create-exam-page.component.css']
})
export class CreateExamPageComponent implements OnInit {
  errorMessage: {};
  isLoadedPage: boolean;
  fName: string;
  lName: string;
  isTeacher: boolean;
  uId: string;
  public uloha: Uloha;
  private zoznamCisel = [];
  priklad = {};
  prikladJson;
  prikladyJson = {};
  prikladyJsonPc = 1;
  prikladyArrayVykresli = [];

  isRectVisible = false;

  items: FirebaseListObservable<any[]>;

  constructor(private angularFire: AngularFire, public authService: AuthService, private router: Router, private route: ActivatedRoute) {
    this.drawNumberForExam();
    this.isLoadedPage = false;
    this.authService.angularFire.auth.subscribe(
      (auth) => {
        if (auth !== null) {
          this.items = angularFire.database.list('/EXAMS');
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

  createTaskSablona = 0;
  box = null;
  setTab(num: number) {
    this.createTaskSablona = num;
    this.generateEmptyPriklad(num);
    this.isRectVisible = true;
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

  generateEmptyPriklad(input) {
    let pom;
    switch (input) {
      case 2:
        pom = 3;
        break;
      case 3:
        pom = 6;
        break;
      case 4:
        pom = 10;
        break;
    }
    for (let i = 1; i <= pom; i++) {
      this.priklad[ 'Slot_' + i] = null;
    }
  }
  removeTask() {
    this.setTab(0);
    this.isRectVisible = false;
  }
  controlTask() {
    let pom = true;
    for (let pr in this.priklad) {
      if ( this.priklad[pr] === null ) {
        $('#' + pr).css('background-color', 'red');
        pom = false;
      }
    }
    if ( pom ) {
      return true;
    }
    return false;
  }
  submitTask() {
    if (this.controlTask()) {
      this.setTab(0);
      this.prikladyJson[this.prikladyJsonPc.toString()] = this.priklad;
      this.prikladyArrayVykresli.push({key: this.prikladyJsonPc, value: JSON.stringify(this.priklad)});
      this.isRectVisible = false;
      this.prikladyJsonPc++;
      delete this.priklad;
      this.priklad = {};
    }
  }
  deleteTask(id) {
    console.log('delete');
    let prikladyArrayVykresliPom = [];
    for (let i = 0; i < this.prikladyArrayVykresli.length; i++) {
      if (this.prikladyArrayVykresli[i].key !== id ) {
        prikladyArrayVykresliPom.push(this.prikladyArrayVykresli[i]);
      }
    }
    this.prikladyArrayVykresli = prikladyArrayVykresliPom;
    delete this.prikladyJson[id];
  }
  saveExam(examsName: string) {
    if (examsName.length >= 4 ) {
      this.removeTask();
      this.items.push(new Uloha( examsName, this.uId, this.prikladyJson));
      this.priklad = {};
      this.prikladJson;
      this.prikladyJson = {};
      this.prikladyJsonPc = 1;
      this.prikladyArrayVykresli = [];
      $('#ExamsName').val('');
      this.errorMessage = this.authService.getErrorMsg('success', { code: 'OwnText', text: 'Úloha bola úspešne vytvorená' } );
      setTimeout(() => {this.router.navigate(['exams']); }, 2000);
    }else {
      this.errorMessage = this.authService.getErrorMsg('error',
        { code: 'OwnText', text: 'Názov úlohy je krátky. Minimálna dĺžka je 4 znaky.' } );
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
