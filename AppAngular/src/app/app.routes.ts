  import { Routes } from '@angular/router';
  import { MainLayoutComponent } from
  './layouts/main-layout/main-layout.component';
  import { LoginPageComponent } from
  './features/auth/pages/login-page/login-page.component';
  import { HomePageComponent } from
  './features/auth/pages/home-page/home-page.component';
  import { RegisterClientPageComponent } from './features/users/pages/register-client-page/register-client-page.component';
  import { RegisterInternalUserPageComponent } from
  './features/users/pages/register-internal-user-page/register-internal-user-page.component';
  import { EditUserPageComponent } from
  './features/users/pages/edit-user-page/edit-user-page.component';
  import { ChangePasswordPageComponent } from './features/users/pages/change-password-page/change-password-page.component';
  import { AuditLogsPageComponent } from './features/audit-logs/pages/audit-logs-page/audit-logs-page.component';
  import { UsersListPageComponent } from
  './features/users/pages/users-list-page/users-list-page.component';

  export const routes: Routes = [
    {
      path: '',
      component: MainLayoutComponent,
      children: [
        { path: '', component: HomePageComponent },
        { path: 'register-internal-user', component:
  RegisterInternalUserPageComponent },
        { path: 'edit-user', component: EditUserPageComponent },
        { path: 'change-password', component:
  ChangePasswordPageComponent },
        { path: 'audit-logs', component: AuditLogsPageComponent },
        { path: 'users', component: UsersListPageComponent }
      ]
    },
    { path: 'login', component: LoginPageComponent },
    { path: 'register-client', component: RegisterClientPageComponent }
  ];