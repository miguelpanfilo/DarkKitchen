import { Directive, Input, TemplateRef, ViewContainerRef } from
  '@angular/core';
  import { AuthService } from '../../core/services/auth.service';

  @Directive({
    selector: '[appHasRole]',
    standalone: true
  })
  export class HasRoleDirective {

    constructor(
      private templateRef: TemplateRef<any>,
      private viewContainer: ViewContainerRef,
      private authService: AuthService
    ) {}

    @Input() set appHasRole(allowedRoles: string[]) {
      const role = this.authService.getRole();
      this.viewContainer.clear();
      if (role && allowedRoles.includes(role)) {
        this.viewContainer.createEmbeddedView(this.templateRef);
      }
    }
  }