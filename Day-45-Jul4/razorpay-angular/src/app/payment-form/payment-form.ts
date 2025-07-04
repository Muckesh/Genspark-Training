import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PaymentService } from '../services/payment.service';


interface PaymentResult{
  status: 'success' | 'failed' | 'cancelled';
  paymentId?:string;
  message:string;
}

@Component({
  selector: 'app-payment-form',
  imports: [CommonModule,ReactiveFormsModule],
  templateUrl: './payment-form.html',
  styleUrl: './payment-form.css'
})
export class PaymentForm implements OnInit{
  paymentForm: FormGroup;
  isProcessing = false;
  paymentResult: PaymentResult | null = null;

  constructor(private fb: FormBuilder, private paymentService: PaymentService) {
    this.paymentForm = this.fb.group({
      amount: ['', [Validators.required, Validators.min(1)]],
      name: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      contact: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]]
    });
  }

  ngOnInit(): void {
    this.loadRazorpayScript();
  }

  loadRazorpayScript(): void {
    const script = document.createElement('script');
    script.src = 'https://checkout.razorpay.com/v1/checkout.js';
    script.onerror = () => console.error('Failed to load Razorpay script');
    document.body.appendChild(script);
  }

  onSubmit(): void {
    if (this.paymentForm.invalid) return;

    this.isProcessing = true;
    this.paymentResult = null;

    const form = this.paymentForm.value;
    console.log(form.name);

    const orderId = `order_${Date.now()}`;
    const amount = form.amount * 100; // Convert to paise

    this.paymentService.initiatePayment(
      orderId,
      amount,
      form.name,
      form.email,
      form.contact,
      (result: PaymentResult) => this.handlePaymentResponse(result)
    );
  }

  handlePaymentResponse(result: PaymentResult): void {
    this.isProcessing = false;
    this.paymentResult = result;
    alert(`${result.message}${result.paymentId ? ` (Payment ID: ${result.paymentId})` : ''}`);
    this.resetForm();
  }

  resetForm(): void {
    this.paymentForm.reset();
    this.paymentResult = null;
  }
  
}
