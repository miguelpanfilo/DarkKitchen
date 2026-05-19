import { Component, Input, Output, EventEmitter } from '@angular/core';
import { EditButtonComponent } from '../../../../shared/components/edit-button/edit-button.component';
import { DeleteButtonComponent } from '../../../../shared/components/delete-button/delete-button.component';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-user-card',
  imports: [EditButtonComponent, DeleteButtonComponent],
  templateUrl: './user-card.component.html',
  styleUrl: './user-card.component.css'
})
export class UserCardComponent {
  @Input() user!: User;
  @Output() edit = new EventEmitter<User>();
  @Output() delete = new EventEmitter<User>();
  @Output() changePassword = new EventEmitter<User>();

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