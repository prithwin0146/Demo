import { authGuard } from './auth-guard';
import { Routes } from '@angular/router';

import { Login } from './login/login';
import { RegisterComponent } from './register/register';
import { LayoutComponent } from './shared/layout/layout.component';
import { EmployeeListComponent } from './employees/employee-list/employee-list';

import { AddEmployeeComponent } from './employees/add-employee/add-employee';
import { EditEmployeeComponent } from './employees/edit-employee/edit-employee';
import { ProjectListComponent } from './projects/project-list/project-list';
import { ProjectViewComponent } from './projects/project-view/project-view';
import { AddProjectComponent } from './projects/add-project/add-project';
import { EditProjectComponent } from './projects/edit-project/edit-project';
import { DepartmentListComponent } from './departments/department-list/department-list';
import { AddDepartmentComponent } from './departments/add-department/add-department';
import { EditDepartmentComponent } from './departments/edit-department/edit-department';
import { DepartmentViewComponent } from './departments/department-view/department-view';


export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  { path: 'login', component: Login },

  {
    path: '',
    component: LayoutComponent,
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
      { path: 'projects', component: ProjectListComponent },
      { path: 'projects/add', component: AddProjectComponent },
      { path: 'projects/edit/:id', component: EditProjectComponent },
      { path: 'projects/:id', component: ProjectViewComponent },
      { path: 'departments', component: DepartmentListComponent },
      { path: 'departments/add', component: AddDepartmentComponent },
      { path: 'departments/edit/:id', component: EditDepartmentComponent },
      { path: 'departments/:id', component: DepartmentViewComponent },
    ]
  },

  { path: '**', redirectTo: 'login' }
];


