import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { Ng2DatetimePickerModule } from 'ng2-datetime-picker';


// firebase modules
import { firebaseConfig } from '../environments/firebase.config';
import { routesConfig } from '../environments/routes.config';

import { AngularFireModule } from 'angularfire2';

import { AppComponent } from './app.component';

import { AuthService } from './providers/auth.service';
import { LoginPageComponent } from './login-page/login-page.component';

import { RouterModule, Routes } from '@angular/router';

import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { RegistrationPageComponent } from './registration-page/registration-page.component';
import { MyClassesPageComponent } from './my-classes-page/my-classes-page.component';
import { CreateClassPageComponent } from './create-class-page/create-class-page.component';
import { OpenClassPageComponent } from './open-class-page/open-class-page.component';
import { CreateExamPageComponent } from './create-exam-page/create-exam-page.component';
import { MyExamsPageComponent } from './my-exams-page/my-exams-page.component';
import { OpenExamPageComponent } from './open-exam-page/open-exam-page.component';
import { ShowExamPageComponent } from './show-exam-page/show-exam-page.component';
import { EditExamPageComponent } from './edit-exam-page/edit-exam-page.component';
import { ActualScreenPageComponent } from './actual-screen-page/actual-screen-page.component';
import { ClassStatisticsPageComponent } from './class-statistics-page/class-statistics-page.component';
import { StudentStatisticsPageComponent } from './student-statistics-page/student-statistics-page.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginPageComponent,
    PageNotFoundComponent,
    RegistrationPageComponent,
    MyClassesPageComponent,
    CreateClassPageComponent,
    OpenClassPageComponent,
    CreateExamPageComponent,
    MyExamsPageComponent,
    OpenExamPageComponent,
    ShowExamPageComponent,
    EditExamPageComponent,
    ActualScreenPageComponent,
    ClassStatisticsPageComponent,
    StudentStatisticsPageComponent,

  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule,
    AngularFireModule.initializeApp(firebaseConfig),
    RouterModule.forRoot(routesConfig),
  ],
  providers: [AuthService],
  bootstrap: [AppComponent],
})
export class AppModule { }
