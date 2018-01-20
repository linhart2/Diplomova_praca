import { Component, OnInit } from '@angular/core';
import {Router} from '@angular/router';
import {AuthService} from '../providers/auth.service';
import {AngularFire, FirebaseListObservable } from 'angularfire2';
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

  private items: FirebaseListObservable<any[]>;
  public zoznamTried: Array<any> = [];

  private itemsZoznamUloh: FirebaseListObservable<any[]>;
  public zoznamUloh: Array<any> = [];
  public zoznamZobrazPriradeneUlohy: Array<any> = [];
  public zoznamMojichUloh: Array<any> = [];

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

  public zobrazZoznamPriradenychUloh(classId: string) {
    let nacitajZoznamPriradenychUloh = true;
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
      nacitajZoznamPriradenychUloh = false;
    });
    this.zoznamZobrazPriradeneUlohy = priradeneUlohy;
  }

  public ulozPridaneUlohy(classId: string) {

  }

  public zobrazZoznamUlohNaPridanie(classId: string) {
    let nacitajZoznamUlohnaPridanie = true;
    let _zoznamPriradenychUloh = this.angularFire.database.list('/TABLES/' + classId);
    let zoznamPUloh = this.authService.ConvertToArray(_zoznamPriradenychUloh);
    this.itemsZoznamUloh.subscribe(snapshot => {
      let ulohy = [];
      snapshot.forEach( uloha => {
        if (uloha.teacherID === this.uId) {
          if (!zoznamPUloh.some(x => x.$key === uloha.$key)) {
            ulohy.push(uloha);
          }
        }
        nacitajZoznamUlohnaPridanie = false;
      });
      this.zoznamUloh = ulohy;
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
