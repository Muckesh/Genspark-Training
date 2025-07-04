import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { PaymentForm } from './payment-form';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { PaymentResult, PaymentService } from '../services/payment.service';
import { By } from '@angular/platform-browser';

describe('PaymentFormComponent', () => {
  let component: PaymentForm;
  let fixture: ComponentFixture<PaymentForm>;
  let mockPaymentService: jasmine.SpyObj<PaymentService>;

  beforeEach(waitForAsync(() => {
    mockPaymentService = jasmine.createSpyObj('PaymentService', ['initiatePayment']);

    TestBed.configureTestingModule({
      imports: [CommonModule, ReactiveFormsModule,FormsModule],
      providers: [
        FormBuilder,
        { provide: PaymentService, useValue: mockPaymentService }
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PaymentForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form with empty values', () => {
    expect(component.paymentForm.value).toEqual({
      amount: '',
      name: '',
      email: '',
      contact: ''
    });
  });

  describe('Form Validation', () => {

    it('should mark amount as invalid when less than 1', () => {
      const amountControl = component.paymentForm.get('amount');
      amountControl?.setValue(0);
      expect(amountControl?.valid).toBeFalsy();
      expect(amountControl?.errors?.['min']).toBeTruthy();
    });

    it('should mark name as invalid when empty', () => {
      const nameControl = component.paymentForm.get('name');
      nameControl?.setValue('');
      expect(nameControl?.valid).toBeFalsy();
      expect(nameControl?.errors?.['required']).toBeTruthy();
    });

    it('should mark email as invalid when format is wrong', () => {
      const emailControl = component.paymentForm.get('email');
      emailControl?.setValue('invalid-email');
      expect(emailControl?.valid).toBeFalsy();
      expect(emailControl?.errors?.['email']).toBeTruthy();
    });

  });

  describe('onSubmit()', () => {
    beforeEach(() => {
      component.paymentForm.setValue({
        amount: 100,
        name: 'Test User',
        email: 'test@example.com',
        contact: '9876543210'
      });
    });

    it('should set isProcessing to true when submitting', () => {
      component.onSubmit();
      expect(component.isProcessing).toBeTrue();
    });

    it('should reset paymentResult when submitting', () => {
      component.paymentResult = { status: 'success', paymentId: '123', message: 'test' };
      component.onSubmit();
      expect(component.paymentResult).toBeNull();
    });
  });

  describe('handlePaymentResponse()', () => {
    it('should set isProcessing to false', () => {
      component.isProcessing = true;
      component.handlePaymentResponse({ status: 'success', message: 'test' });
      expect(component.isProcessing).toBeFalse();
    });

    it('should call resetForm', () => {
      spyOn(component, 'resetForm');
      component.handlePaymentResponse({ status: 'success', message: 'test' });
      expect(component.resetForm).toHaveBeenCalled();
    });
  });


  describe('Template', () => {
    it('should disable submit button when form is invalid', () => {
      component.paymentForm.setValue({
        amount: '',
        name: '',
        email: '',
        contact: ''
      });
      fixture.detectChanges();
      
      const button = fixture.debugElement.query(By.css('button[type="submit"]')).nativeElement;
      expect(button.disabled).toBeTrue();
    });

    it('should enable submit button when form is valid', () => {
      component.paymentForm.setValue({
        amount: 100,
        name: 'Test',
        email: 'test@test.com',
        contact: '9876543210'
      });
      fixture.detectChanges();
      
      const button = fixture.debugElement.query(By.css('button[type="submit"]')).nativeElement;
      expect(button.disabled).toBeFalse();
    });

    it('should show processing text when isProcessing is true', () => {
      component.isProcessing = true;
      fixture.detectChanges();
      
      const button = fixture.debugElement.query(By.css('button[type="submit"]')).nativeElement;
      expect(button.textContent).toContain('Processing...');
    });
  });

  describe('loadRazorpayScript()', () => {
    it('should load Razorpay script', () => {
      spyOn(document.body, 'appendChild');
      component.loadRazorpayScript();
      
      expect(document.body.appendChild).toHaveBeenCalled();
      const script = (document.body.appendChild as jasmine.Spy).calls.first().args[0];
      expect(script.src).toBe('https://checkout.razorpay.com/v1/checkout.js');
    });
  });
});