import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Sidebar } from '../sidebar/sidebar';
import { Navbar } from '../navbar/navbar';

@Component({
  selector: 'app-buyer-layout',
  imports: [RouterOutlet,Navbar,CommonModule,Sidebar],
  templateUrl: './buyer-layout.html',
  styleUrl: './buyer-layout.css'
})
export class BuyerLayout {
  isSidebarCollapsed = false;

  onSidebarToggle(collapsed: boolean) {
    this.isSidebarCollapsed = collapsed;
  }

}
