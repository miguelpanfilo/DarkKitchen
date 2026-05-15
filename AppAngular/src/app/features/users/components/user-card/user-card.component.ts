import { Component, Input, Output, EventEmitter } from
  '@angular/core';
  import { EditButtonComponent } from
  '../../../../shared/components/edit-button/edit-button.component';
  import { DeleteButtonComponent } from
  '../../../../shared/components/delete-button/delete-button.component';

  @Component({
    selector: 'app-user-card',
    imports: [EditButtonComponent, DeleteButtonComponent],
    templateUrl: './user-card.component.html',
    styleUrl: './user-card.component.css'
  })
  export class UserCardComponent {
    @Input() user: any;
    @Output() edit = new EventEmitter<any>();
    @Output() delete = new EventEmitter<any>();
    @Output() changePassword = new EventEmitter<any>();

    onEdit() {
      this.edit.emit(this.user);
    }

    onDelete() {
      this.delete.emit(this.user);
    }

    onChangePassword() {
      this.changePassword.emit(this.user);
    }
  }