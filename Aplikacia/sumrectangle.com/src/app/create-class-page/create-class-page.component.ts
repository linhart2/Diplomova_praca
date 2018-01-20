import { Component, OnInit } from '@angular/core';
import {Router, ActivatedRoute } from '@angular/router';
import {AuthService} from '../providers/auth.service';
import {Trieda} from './Trieda';
import {AngularFire, FirebaseListObservable } from 'angularfire2';

@Component({
  selector: 'app-create-class-page',
  templateUrl: './create-class-page.component.html',
  styleUrls: ['./create-class-page.component.css']
})
export class CreateClassPageComponent implements OnInit {
  errorMessage: {};
  isLoadedPage: boolean;
  fName: string;
  lName: string;
  isTeacher: boolean;
  uId: string;
  public trieda: Trieda;
  classId: string;

  items: FirebaseListObservable<any[]>;
  menoTriedy: string;
  hesloTriedy: string;

  constructor(private angularFire: AngularFire, public authService: AuthService, private router: Router, private route: ActivatedRoute) {
    this.route.params.subscribe(params => {
      this.classId = params['id']; // (+) converts string 'id' to a number

      // In a real app: dispatch action to load the details here.
    });
    this.isLoadedPage = false;
    this.authService.angularFire.auth.subscribe(
      (auth) => {
        if (auth !== null) {
          this.items = angularFire.database.list('/CLASSES');
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
          if ( this.classId ) {
            let classData = this.angularFire.database.object('/CLASSES/' + this.classId);
            classData.subscribe(data => {
              this.menoTriedy = data.className;
              this.hesloTriedy = data.heslo;
            });
          }
        }
      }
    );
    this.errorMessage = {};
  }

  addClass(className: string, classPassword: string ) {
    if (this.classId) {
      let classData = this.angularFire.database.object('/CLASSES/' + this.classId);
      classData.update({className: className, heslo: classPassword} );
      this.errorMessage = this.authService.getErrorMsg('success', { code: 'OwnText', text: 'Zmeny boli úspešne zapísané' } );
      setTimeout(() => {this.router.navigate(['classes']); }, 2000);
    }else {
      this.items.push(new Trieda ( className, classPassword, this.uId));
      this.errorMessage = this.authService.getErrorMsg('success', { code: 'OwnText', text: 'Trieda bola úspešne vytvorená' } );
      setTimeout(() => {this.router.navigate(['classes']); }, 2000);
    }

  }

  ngOnInit() {
  }

}
