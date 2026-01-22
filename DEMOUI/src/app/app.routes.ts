import { authGuard } from './auth-guard';
import { Routes } from '@angular/router';

import { Login } from './login/login';
import { RegisterComponent } from './register/register';
import { EmployeeListComponent } from './employees/employee-list/employee-list';

import { AddEmployeeComponent } from './employees/add-employee/add-employee';
import { EditEmployeeComponent } from './employees/edit-employee/edit-employee';


export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  { path: 'login', component: Login },

  { 
    path: '', 
    canActivateChild: [authGuard], 
    children: [
      { path: 'register', component: RegisterComponent },
      {
        path: 'employees',
        component: EmployeeListComponent,
        runGuardsAndResolvers: 'always'
      },
      { path: 'employees/add', component: AddEmployeeComponent },
      { path: 'employees/edit/:id', component: EditEmployeeComponent },
    ] 
  },
  
  { path: '**', redirectTo: 'login' }
];


