import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout.component';
import { LoginPageComponent } from './features/auth/pages/login-page/login-page.component';
import { HomePageComponent } from './features/auth/pages/home-page/home-page.component';
import { WelcomePageComponent } from './features/auth/pages/welcome-page/welcome-page.component';
import { RegisterClientPageComponent } from './features/users/pages/register-client-page/register-client-page.component';
import { RegisterInternalUserPageComponent } from './features/users/pages/register-internal-user-page/register-internal-user-page.component';
import { EditUserPageComponent } from './features/users/pages/edit-user-page/edit-user-page.component';
import { ChangePasswordPageComponent } from './features/users/pages/change-password-page/change-password-page.component';
import { UsersListPageComponent } from './features/users/pages/users-list-page/users-list-page.component';
import { AuditLogsPageComponent } from './features/audit-logs/pages/audit-logs-page/audit-logs-page.component';
import { ReportsPageComponent } from './features/reports/pages/reports-page/reports-page.component';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';
import { OrdersListPageComponent } from './features/orders/pages/orders-list-page/orders-list-page.component';
import { OrderDetailPageComponent } from './features/orders/pages/order-detail-page/order-detail-page.component';
import { CreateOrderPageComponent } from './features/orders/pages/create-order-page/create-order-page.component';
import { ShippingTypesPageComponent } from './features/shipping-types/pages/shipping-types-page/shipping-types-page.component';
import { ProductsListPageComponent } from './features/products/pages/products-list-page/products-list-page.component';
import { CreateProductPageComponent } from './features/products/pages/create-product-page/create-product-page.component';
import { EditProductPageComponent } from './features/products/pages/edit-product-page/edit-product-page.component';
import { PromotionsListPageComponent } from './features/promotions/pages/promotions-list-page/promotions-list-page.component';
import { EditPromotionPageComponent } from './features/promotions/pages/edit-promotion-page/edit-promotion-page.component';
import { CreatePromotionPageComponent } from './features/promotions/pages/create-promotion-page/create-promotion-page.component';
import { ProductImportersPageComponent } from './features/product-importers/pages/product-importers-page/product-importers-page.component';

export const routes: Routes = [
  {
    path: '', component: AuthLayoutComponent,
    children: [
      { path: '', redirectTo: 'welcome', pathMatch: 'full' },
      { path: 'welcome', component: WelcomePageComponent },
      { path: 'login', component: LoginPageComponent },
      { path: 'register-client', component: RegisterClientPageComponent },
    ]
  },
  {
    path: 'home',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    canActivateChild: [authGuard],
    children: [
      { path: '', component: HomePageComponent },
      {
        path: 'users', component: UsersListPageComponent, canActivate:
          [roleGuard(['Administrative'])]
      },
      {
        path: 'reports', component: ReportsPageComponent, canActivate:
          [roleGuard(['Administrative'])]
      },
      {
        path: 'register-internal-user', component:
          RegisterInternalUserPageComponent, canActivate:
          [roleGuard(['Administrative'])]
      },
      {
        path: 'edit-user', component: EditUserPageComponent,
        canActivate: [roleGuard(['Administrative'])]
      },
      {
        path: 'change-password', component:
          ChangePasswordPageComponent, canActivate:
          [roleGuard(['Administrative'])]
      },
      {
        path: 'audit-logs', component: AuditLogsPageComponent,
        canActivate: [roleGuard(['Administrative'])]
      },
      { path: 'orders', component: OrdersListPageComponent },
      { path: 'orders/create', component: CreateOrderPageComponent, 
        canActivate: [roleGuard(['Client'])] 
      },
      { path: 'orders/:id', component: OrderDetailPageComponent },
      { path: 'shipping-types', component: ShippingTypesPageComponent,
        canActivate: [roleGuard(['Administrative'])]
      },
      { path: 'products', component: ProductsListPageComponent },
      {
        path: 'products/create', component: CreateProductPageComponent,
        canActivate: [roleGuard(['Administrative'])]
      },
      {
        path: 'products/edit/:id', component: EditProductPageComponent,
        canActivate: [roleGuard(['Administrative'])]
      },
      { path: 'promotions', component: PromotionsListPageComponent },
      {
        path: 'promotions/create', component: CreatePromotionPageComponent,
        canActivate: [roleGuard(['Administrative'])]
      },
      {
        path: 'promotions/edit/:id', component: EditPromotionPageComponent,
        canActivate: [roleGuard(['Administrative'])]
      },
      { path: 'product-importers', component: ProductImportersPageComponent },
    ]
  },
  { path: '**', component: NotFoundComponent }
];
