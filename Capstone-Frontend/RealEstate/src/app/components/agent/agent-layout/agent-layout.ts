import { Component } from '@angular/core';

import { RouterOutlet } from '@angular/router';
import { Navbar } from '../../shared/navbar/navbar';
import { Sidebar } from '../../shared/sidebar/sidebar';


@Component({
  selector: 'app-agent-layout',
  imports: [RouterOutlet,Sidebar,Navbar],
  templateUrl: './agent-layout.html',
  styleUrl: './agent-layout.css'
})
export class AgentLayout {
  isSidebarCollapsed = false;

  onSidebarToggle(collapsed: boolean) {
    this.isSidebarCollapsed = collapsed;
  }

}
