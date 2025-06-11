import { Component, OnInit } from '@angular/core';

interface Customer{
  name:string;
  email:string;
  phone:string;
  address:string;
  likeCount:0;
  dislikeCount:0;
}

@Component({
  selector: 'app-customer-details',
  imports: [],
  templateUrl: './customer-details.html',
  styleUrl: './customer-details.css'
})
export class CustomerDetails {
  customer:Customer =
    {
    name: "John Doe",
    email: "john.doe@example.com",
    phone: "+1-202-555-0156",
    address: "123 Main St, Springfield",
    likeCount:0,
    dislikeCount:0
  };

  like(){
    this.customer.likeCount++;
  }

  dislike(){
    this.customer.dislikeCount++;
  }

}
