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

  private itemsZobrazPriradeneUlohy: FirebaseListObservable<any[]>;
  public zoznamZobrazPriradeneUlohy: Array<any> = [];

  constructor(private angularFire: AngularFire, public authService: AuthService, private router: Router) {
    this.isLoadedPage = false;
    this.authService.angularFire.auth.subscribe(
      (auth) => {
        if (auth !== null) {
          this.items = angularFire.database.list('/CLASSES');
          this.itemsZoznamUloh = angularFire.database.list('/EXAMS');
          this.itemsZobrazPriradeneUlohy = angularFire.database.list('/');
          this.zobrazTriedy();
          this.zobrazZoznamUlohNaPridanie();
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

  public zobrazZoznamUlohNaPridanie() {
    this.itemsZoznamUloh.subscribe(snapshot => {
      let ulohy = [];
      snapshot.forEach( uloha => {
        if (uloha.teacherID === this.uId) {
          ulohy.push(uloha);
        }
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
