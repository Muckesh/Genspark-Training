import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

declare var paypal: any;

@Injectable({
  providedIn: 'root'
})
export class PaypalService {
  private paypal: any;

  constructor() {
    this.loadPaypalScript();
  }

  private loadPaypalScript(): void {
    if (!document.querySelector('#paypal-js')) {
      const script = document.createElement('script');
      script.id = 'paypal-js';
      script.src = `https://www.paypal.com/sdk/js?client-id=${environment.paypalClientId}&currency=USD`;
      script.onload = () => {
        this.paypal = paypal;
      };
      document.head.appendChild(script);
    }
  }

  renderButton(elementId: string, amount: number, onApprove: (details: any) => void): void {
    paypal.Buttons({
      createOrder: (data: any, actions: any) => {
        return actions.order.create({
          purchase_units: [{
            amount: {
              value: amount.toString()
            }
          }]
        });
      },
      onApprove: (data: any, actions: any) => {
        return actions.order.capture().then((details: any) => {
          onApprove(details);
        });
      }
    }).render(`#${elementId}`);
  }
}