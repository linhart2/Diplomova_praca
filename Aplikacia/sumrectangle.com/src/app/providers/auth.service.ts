import { Injectable } from '@angular/core';

import { AngularFire, AuthProviders, AuthMethods } from 'angularfire2';
import set = Reflect.set;

@Injectable()
export class AuthService {

  constructor(public angularFire: AngularFire) {

  }

  registerUserWithEmailAndPassword(userEmail: string, userPassword: string, userFirstName: string, userLastName: string) {
      return this.angularFire.auth.createUser({
          email: userEmail,
          password: userPassword
      }).then((user) => {
          return this.angularFire.database.object(`/USERS/${user.uid}`).update({
              email: userEmail,
              firstName: userFirstName,
              lastName: userLastName,
              isTeacher: true
          });
      });
  }

  loginWithEmailAndPassword(userEmail: string, userPassword: string){
      return this.angularFire.auth.login({
              email: userEmail,
              password: userPassword,
          },
          {
              provider: AuthProviders.Password,
              method: AuthMethods.Password,
          });
  }

  logout() {
      return this.angularFire.auth.logout();
  }

  setErrorMessage(error) {
    if (error.code === 'auth/wrong-password') {
      return'Nesprávne zadané prihlasovacie heslo';
    } else if (error.code === 'auth/invalid-email') {
      return'Nesprávne zadaný prihlasovací email';
    } else if (error.code === 'auth/weak-password') {
      return 'Slabé heslo';
    } else if (error === 'wrong_pin') {
      return 'Nespraven zadany PIN';
    } else if (error.code === 'registraciaPovinneUdaje') {
      return 'Nevyplnili ste všetky povinné údaje';
    } else if (error.code === 'registraciaNezhodneHesla') {
      return 'Nezhodné heslá';
    } else if (error.code === 'registraciaMinDlzkaHesla') {
      return 'Minimálna dĺžka hesla je 6 znakov';
    } else if (error.code === 'OwnText') {
      return error.text;
    } else {
      return 'Nesprávne zadané prihlasovacie údaje';
    }
  }

  getErrorMsg(typeMsg: string, error) {
    return {
      type: 'flash ' + typeMsg,
      msg: this.setErrorMessage(error)
    };
  }
  PopupAlert(text: string) {
    if (confirm('Naozaj chcete odstranit ' + text) === true) {
      return true;
    } else {
      return false;
    }
  }

  ConvertToArray(object) {
    let pole = [];
    object.subscribe( obj => {
      obj.forEach( o => {
        pole.push(o);
      });
    });
    return pole;
  }

}
