import { Component, OnInit } from '@angular/core';
import {Router} from '@angular/router';
import {AuthService} from '../providers/auth.service';
import {AngularFire, FirebaseListObservable } from 'angularfire2';
declare var jquery: any;
declare var $: any;

@Component({
  selector: 'app-my-classes-page',
  templateUrl: './my-classes-page.component.html',
  styleUrls: ['./my-classes-page.component.css']
})
export class MyClassesPageComponent implements OnInit {
  errorMessage: {};
  isLoadedPage: boolean;
  fName: string;
  lName: string;
  isTeacher: boolean;
  uId: string;
  ulohyNaUlozenie = {};
  idTriedy;
  menoTriedy;

  private items: FirebaseListObservable<any[]>;
  public zoznamTried: Array<any> = [];

  private itemsZoznamUloh: FirebaseListObservable<any[]>;
  public zoznamUloh: Array<any> = [];

  public zoznamZobrazPriradeneUlohy: Array<any> = [];
  public zoznamMojichUloh: Array<any> = [];

  nacitajZoznamUlohnaPridanie: boolean;
  nacitajZoznamPriradenychUloh: boolean;

  constructor(private angularFire: AngularFire, public authService: AuthService, private router: Router) {
    this.isLoadedPage = false;
    this.authService.angularFire.auth.subscribe(
      (auth) => {
        if (auth !== null) {
          this.items = angularFire.database.list('/CLASSES');
          this.itemsZoznamUloh = angularFire.database.list('/EXAMS');
          this.zobrazTriedy();
          this.getZoznamMojichUloh();
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

  public zobrazTriedy() {
    this.items.subscribe( snapshot => {
      let items = [];
      snapshot.forEach( item => {
        if ( item.teacherID === this.uId) {
          items.push(item);
        }
      });
      this.zoznamTried = items;
    });
  }

  public pridajPrikladNaTabulu(ev) {
    if (ev.target.checked) {
      this.ulohyNaUlozenie[ev.target.defaultValue] = this.zoznamMojichUloh.find(x => x.$key === ev.target.defaultValue).priklady.length;
    }else {
      delete this.ulohyNaUlozenie[ev.target.defaultValue];
    }
  }

  public ulozPridaneUlohy() {
    if (this.idTriedy) {
    this.angularFire.database.object('/TABLES/' + this.idTriedy).update(this.ulohyNaUlozenie);
    delete this.ulohyNaUlozenie;
    this.ulohyNaUlozenie = {};
    delete this.idTriedy;
    }
  }

  public zmazPridaneUlohy(examId: string ) {
    let classID = document.getElementsByClassName('_zobrazUlohy').item(0).getAttribute('id');
    if (classID === null) {
      return;
    }
    if (this.authService.PopupAlert('ulohu')) {
      let priklad = this.angularFire.database.object('/TABLES/' + classID + '/' + examId);
      priklad.remove();
    }
  }

  public getZoznamMojichUloh() {
    this.itemsZoznamUloh.subscribe(snapshot => {
      let ulohy = [];
      snapshot.forEach( uloha => {
        if (uloha.teacherID === this.uId) {
          ulohy.push(uloha);
        }
      });
      this.zoznamMojichUloh = ulohy;
    });
  }

  public zobrazZoznamPriradenychUloh(classId: string, className: string) {
    this.nacitajZoznamPriradenychUloh = true;
    this.idTriedy = classId;
    this.menoTriedy = className;
    let _zoznamUlohNaTabuli = this.angularFire.database.list('/TABLES/' + classId);
    let priradeneUlohy = [];
    _zoznamUlohNaTabuli.subscribe(prikladyNaTabuli => {
      prikladyNaTabuli.forEach(prikladNaTabuli => {
        this.zoznamMojichUloh.forEach(uloha => {
          if ( uloha.$key ===  prikladNaTabuli.$key) {
            priradeneUlohy.push(uloha);
          }
        });
      });
      this.nacitajZoznamPriradenychUloh = false;
    });
    this.zoznamZobrazPriradeneUlohy = priradeneUlohy;
  }

  public zobrazZoznamUlohNaPridanie(classId: string, className: string) {
    this.nacitajZoznamUlohnaPridanie = true;
    this.idTriedy = classId;
    this.menoTriedy = className;
    let _zoznamPriradenychUloh = this.angularFire.database.list('/TABLES/' + classId);
    _zoznamPriradenychUloh.subscribe(prikladyNaTabuli => {
      this.itemsZoznamUloh.subscribe(snapshot => {
        let ulohy = [];
        snapshot.forEach( uloha => {
          if ((uloha.teacherID === this.uId) && (!prikladyNaTabuli.some(x => x.$key === uloha.$key))) {
            ulohy.push(uloha);
          }
          this.nacitajZoznamUlohnaPridanie = false;
        });
        this.zoznamUloh = ulohy;
      });
    });
  }

  public deleteClass(id: string) {
    if (this.authService.PopupAlert('triedu')) {
    let _trieda = this.angularFire.database.object('/CLASSES/' + id);
    _trieda.remove();
    }
  }

  ngOnInit() {
  }
}
