import {Routes} from '@angular/router';
import { LoginPageComponent } from '../app/login-page/login-page.component';
import { RegistrationPageComponent } from '../app/registration-page/registration-page.component';
import { PageNotFoundComponent } from '../app/page-not-found/page-not-found.component';
import { MyClassesPageComponent } from '../app/my-classes-page/my-classes-page.component';
import { MyExamsPageComponent } from '../app/my-exams-page/my-exams-page.component';
import { CreateClassPageComponent } from '../app/create-class-page/create-class-page.component';
import { OpenClassPageComponent } from '../app/open-class-page/open-class-page.component';
import { CreateExamPageComponent } from '../app/create-exam-page/create-exam-page.component';
import { OpenExamPageComponent } from '../app/open-exam-page/open-exam-page.component';
import { ShowExamPageComponent } from '../app/show-exam-page/show-exam-page.component';
import { EditExamPageComponent } from '../app/edit-exam-page/edit-exam-page.component';



export const routesConfig: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full'},
  { path: 'login', component: LoginPageComponent },
  { path: 'registration', component: RegistrationPageComponent },
  { path: 'classes', component: MyClassesPageComponent },
  { path: 'exams', component: MyExamsPageComponent },
  { path: 'createclass', component: CreateClassPageComponent },
  { path: 'updateclass/:id', component: CreateClassPageComponent },
  { path: 'openclass/:id', component: OpenClassPageComponent },
  { path: 'createexam', component: CreateExamPageComponent },
  { path: 'openexam/:id', component: OpenExamPageComponent },
  { path: 'showexam/:id/:pid', component: ShowExamPageComponent },
  { path: 'editexam/:id/:pid', component: EditExamPageComponent },
  { path: '**', component: PageNotFoundComponent }
];
