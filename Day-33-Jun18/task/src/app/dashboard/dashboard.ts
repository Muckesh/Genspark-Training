import { Component, OnInit } from '@angular/core';
import { AgCharts } from 'ag-charts-angular';
import { count } from 'rxjs';
import { AgBarSeriesOptions, AgChartOptions } from "ag-charts-community";
import { ChartType, GoogleChartsModule } from 'angular-google-charts';
import { UserService } from '../services/user.service';


@Component({
  selector: 'app-dashboard',
  imports: [GoogleChartsModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit {
  
  genderChart = {
    title: 'Gender Distribution',
    type: ChartType.PieChart,
    data: [] as [string, number][],
    columnNames: ['Gender', 'Count'],
    options: { pieHole: 0.4 },
    width: 600,
    height: 400
  };

  roleChart = {
    title: 'Role Distribution',
    type: ChartType.ColumnChart,
    data: [] as [string, number][],
    columnNames: ['Role', 'Count'],
    options: { is3D: true },
    width: 700,
    height: 400
  };

  stateChart = {
    title: 'State-wise Users',
    type: ChartType.GeoChart,
    data: [] as [string, number][],
    columnNames: ['State', 'Users'],
    options: { region: 'US', displayMode: 'regions', resolution: 'provinces' },
    width: 800,
    height: 500
  };

  constructor(private userService:UserService){}

  ngOnInit(): void {
    this.userService.getUsers().subscribe((res:any)=>{
      const users = res.users;

      // gender data
      const genderMap = new Map<string,number>();
      users.forEach((u:any) => {
        genderMap.set(u.gender,(genderMap.get(u.gender) || 0)+1);
      });
      this.genderChart.data=Array.from(genderMap.entries());

      // role data
      const roleMap = new Map<string,number>();
      users.forEach((u:any)=>{
        const role =u.company?.title || 'Unknown';
        roleMap.set(role,(roleMap.get(role)||0)+1);
      });
      this.roleChart.data=Array.from(roleMap.entries());

      // state data
      const stateMap = new Map<string,number>();
      users.forEach((u:any) => {
        const state = u.address?.state || 'Unknown';
        stateMap.set(state,(stateMap.get(state) || 0)+1);
      });
      this.stateChart.data=Array.from(stateMap.entries());


    });
  }

}
