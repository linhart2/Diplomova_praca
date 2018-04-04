import { Component, OnInit } from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthService} from '../providers/auth.service';
import {AngularFire, FirebaseListObservable} from 'angularfire2';
import {forEach} from '@angular/router/src/utils/collection';
declare var jquery: any;
declare var $: any;

@Component({
  selector: 'app-class-statistics-page',
  templateUrl: './class-statistics-page.component.html',
  styleUrls: ['./class-statistics-page.component.css']
})
export class ClassStatisticsPageComponent implements OnInit {
  errorMessage: {};
  isLoadedPage: boolean;
  isTeacher: boolean;
  cid: string;
  private zoznamStatistikZiakovTriedy: FirebaseListObservable<any[]>;
  menoTriedy: string;
  zoznamStatistikZiakov: Array<any> = [];
  zoznamMienZiakov: {};
  vsetkyPriklady: number;
  spravnePriklady: number;
  nespravnePriklady: number;

  constructor(private angularFire: AngularFire, public authService: AuthService, private router: Router, private route: ActivatedRoute) {
    this.route.params.subscribe(params => {
      this.cid = params['cid'];
    });
    this.isLoadedPage = false;
    this.authService.angularFire.auth.subscribe(
      (auth) => {
        if (auth !== null) {
          this.zoznamStatistikZiakovTriedy = this.angularFire.database.list('/STATISTICS/' + this.cid);
          this.angularFire.database.object('/CLASSES/' + this.cid + '/className').subscribe(className => {
              this.menoTriedy = className.$value;
          });
          this.zobrazStatistiky();
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

  zobrazStatistiky() {
    this.zoznamMienZiakov = {};
    this.zoznamStatistikZiakovTriedy.subscribe( zoznamZiakov => {
      var zoznam = [];
      var vsetkyPriklady = 0;
      var spravnePriklady = 0;
      var nespravnePriklady = 0;
      for ( var i = 0; i < zoznamZiakov.length; i++) {
        this.angularFire.database.object('/USERS/' + zoznamZiakov[i].$key).subscribe(ziak => {
          this.zoznamMienZiakov[ziak.$key] = ziak.firstName + ' ' + ziak.lastName;
        });
        vsetkyPriklady += (zoznamZiakov[i].SPRAVNE + zoznamZiakov[i].NESPRAVNE);
        spravnePriklady += (zoznamZiakov[i].SPRAVNE);
        nespravnePriklady += (zoznamZiakov[i].NESPRAVNE);
        zoznam.push(zoznamZiakov[i]);
      }
      this.vsetkyPriklady = vsetkyPriklady;
      this.spravnePriklady = spravnePriklady;
      this.nespravnePriklady = nespravnePriklady;
      this.zoznamStatistikZiakov = zoznam;
    });

  }

  ngOnInit() {
  }

}
