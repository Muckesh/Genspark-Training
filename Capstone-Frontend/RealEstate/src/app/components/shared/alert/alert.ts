import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
@Component({
  selector: 'app-alert',
  imports: [CommonModule],
  templateUrl: './alert.html',
  styleUrl: './alert.css'
})
export class Alert {
  @Input() type: 'success' | 'danger' | 'warning' | 'info' = 'info';
  @Input() message: string = '';
  @Input() dismissible: boolean = true;
  isVisible = true;

  get alertClass() {
    return {
      'alert-success': this.type === 'success',
      'alert-danger': this.type === 'danger',
      'alert-warning': this.type === 'warning',
      'alert-info': this.type === 'info'
    };
  }

  dismiss() {
    this.isVisible = false;
  }


}
