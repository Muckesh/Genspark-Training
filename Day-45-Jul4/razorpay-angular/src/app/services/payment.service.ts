import { Injectable } from '@angular/core';

declare var Razorpay: any;

export interface PaymentResult {
  status: 'success' | 'failed' | 'cancelled';
  paymentId?: string;
  message: string;
}

@Injectable({ providedIn: 'root' })
export class PaymentService {
  initiatePayment(
    orderId: string,
    amount: number,
    name: string,
    email: string,
    contact: string,
    callback: (result: PaymentResult) => void
  ) {
    const options = {
      key: 'rzp_test_YBIFFeXjtQgfao',
      amount: amount,
      currency: 'INR',
      name: 'UPI Payment Simulator',
      description: 'Test Transaction',
    //   order_id: orderId,
      handler: (response: any) => {
        callback({
          status: 'success',
          paymentId: response.razorpay_payment_id,
          message: 'Payment completed successfully!'
        });
      },
      modal: {
        ondismiss: () => {
         callback({
            status:'cancelled',
            message: 'Payment cancelled by the user.'
         });
        }
    },
      prefill: {
        name:name,
        email:email,
        contact:contact,
      },
      notes: {
        address: 'Test address'
      },
      theme: {
        color: '#3399cc'
      },
      method: {
        upi: true
      },
      _: {
        integration: 'manual'
      }
    };

    const rzp = new Razorpay(options);

    rzp.on('payment.failed', (response: any) => {
      callback({
        status: 'failed',
        message: `Payment failed: ${response.error.description}`
      });
    });

    rzp.on('payment.cancelled', () => {
      callback({
        status: 'cancelled',
        message: 'Payment was cancelled by the user'
      });
    });

    rzp.open();
  }
}

