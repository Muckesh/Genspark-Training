import { Component } from '@angular/core';

import { RouterOutlet } from '@angular/router';
import { Sidebar } from '../../sidebar/sidebar';
import { Navbar } from '../../navbar/navbar';

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
