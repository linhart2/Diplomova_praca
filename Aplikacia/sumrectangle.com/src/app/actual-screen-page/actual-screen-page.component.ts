import { Component, OnInit } from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthService} from '../providers/auth.service';
import {AngularFire, FirebaseListObservable} from 'angularfire2';
declare var jquery: any;
declare var $: any;


@Component({
  selector: 'app-actual-screen-page',
  templateUrl: './actual-screen-page.component.html',
  styleUrls: ['./actual-screen-page.component.css']
})
export class ActualScreenPageComponent implements OnInit {
  errorMessage: {};
  isLoadedPage: boolean;
  isTeacher: boolean;
  examId: string;
  uid: string;
  private prikladShow: FirebaseListObservable<any[]>;
  createTaskSablona = 0;
  menoZiaka: string;
  private casAktualizacie: string
  pocetMoznosti: number;

  constructor(private angularFire: AngularFire, public authService: AuthService, private router: Router, private route: ActivatedRoute) {
    this.route.params.subscribe(params => {
    this.uid = params['uid'];
    });
    this.isLoadedPage = false;
    this.authService.angularFire.auth.subscribe(
      (auth) => {
        if (auth !== null) {
          this.prikladShow = this.angularFire.database.list('/USERS/' + this.uid + '/ACTUAL_SCREEN/SCREEN');
          this.angularFire.database.object('/USERS/' + this.uid + '/firstName').subscribe(firstName => {
            this.angularFire.database.object('/USERS/' + this.uid + '/lastName').subscribe(lastName => {
              this.menoZiaka = firstName.$value + ' ' + lastName.$value;
            });
          });
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
    if (this.createTaskSablona === 0 ) {
      this.createTaskSablona = num;
    }
    if ( num !== this.createTaskSablona ) {
      window.location.reload();
    }
    this.createTaskSablona = num;
  }
  zobrazPriklad() {
    this.prikladShow.subscribe( snapshot => {
      this.angularFire.database.object('/USERS/' + this.uid + '/ACTUAL_SCREEN/DATE').subscribe(datum => {
        this.casAktualizacie = datum.$value;
      });
      this.nastavSablonu(snapshot);
      this.vyplnSablonu(snapshot);
    });
  }



  vyplnSablonu(priklad) {
      for (let i = 0; i < priklad.length; i++) {
        $(document).ready(function() {
          $('#' + priklad[i].$key).empty();
        });
        if ( priklad[i].$value !== 'null' ) {
        $(document).ready(function() {
          $("<img style='width: 59px;height: 59px; left: -1px;top: -1px;' src=../../assets/img/Number/"+ priklad[i].$value +".jpg alt=Cislo_"+ priklad[i].$value +"></a>").appendTo('#' + priklad[i].$key);
        });
      }
    }
  }

  public nastavSablonu(obj) {
    this.pocetMoznosti = obj.length;
    switch (obj.length) {
      case 6:
      case 7:
        this.setTab(2);
        break;
      case 12:
      case 13:
        this.setTab(3);
        break;
      case 19:
      case 20:
      case 23:
      case 24:
        this.setTab(4);
        break;
    }
  }

  ngOnInit() {
  }

}
