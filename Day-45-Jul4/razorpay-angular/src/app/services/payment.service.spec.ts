import { TestBed } from '@angular/core/testing';
import { PaymentService, PaymentResult } from './payment.service';

declare var window:any;

describe('PaymentService', () => {
  let service: PaymentService;
  let mockRazorpay: any;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PaymentService);
    
    // Mock Razorpay global object
    mockRazorpay = jasmine.createSpyObj('Razorpay', ['open', 'on', 'close']);
    window.Razorpay = jasmine.createSpy().and.returnValue(mockRazorpay);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('initiatePayment', () => {
    const testOrderId = 'order_123';
    const testAmount = 1000;
    const testName = 'Test User';
    const testEmail = 'test@example.com';
    const testContact = '9876543210';
    const mockCallback = jasmine.createSpy('callback');

    beforeEach(() => {
      mockCallback.calls.reset();
    });

    it('should initialize Razorpay with correct options', () => {
      service.initiatePayment(testOrderId, testAmount, testName, testEmail, testContact, mockCallback);
      
      expect(window.Razorpay).toHaveBeenCalledWith(jasmine.objectContaining({
        key: 'rzp_test_YBIFFeXjtQgfao',
        amount: testAmount,
        currency: 'INR',
        name: 'UPI Payment Simulator',
        description: 'Test Transaction',
        prefill: {
          name: testName,
          email: testEmail,
          contact: testContact
        },
        method: {
          upi: true
        }
      }));
    });

    it('should call open on Razorpay instance', () => {
      service.initiatePayment(testOrderId, testAmount, testName, testEmail, testContact, mockCallback);
      expect(mockRazorpay.open).toHaveBeenCalled();
    });

    it('should call callback with success when payment succeeds', () => {
      const testPaymentId = 'pay_123';
      service.initiatePayment(testOrderId, testAmount, testName, testEmail, testContact, mockCallback);
      
      // Trigger handler
      const handler = (window.Razorpay as jasmine.Spy).calls.mostRecent().args[0].handler;
      handler({ razorpay_payment_id: testPaymentId });
      
      expect(mockCallback).toHaveBeenCalledWith({
        status: 'success',
        paymentId: testPaymentId,
        message: 'Payment completed successfully!'
      } as PaymentResult);
    });

    it('should call callback with failed when payment fails', () => {
      const testError = { description: 'Insufficient funds' };
      service.initiatePayment(testOrderId, testAmount, testName, testEmail, testContact, mockCallback);
      
      // Trigger payment.failed event
      const failedCallback = mockRazorpay.on.calls.argsFor(0)[1];
      failedCallback({ error: testError });
      
      expect(mockCallback).toHaveBeenCalledWith({
        status: 'failed',
        message: `Payment failed: ${testError.description}`
      } as PaymentResult);
    });

    it('should call callback with cancelled when payment is cancelled via button', () => {
      service.initiatePayment(testOrderId, testAmount, testName, testEmail, testContact, mockCallback);
      
      // Trigger payment.cancelled event
      const cancelledCallback = mockRazorpay.on.calls.argsFor(1)[1];
      cancelledCallback();
      
      expect(mockCallback).toHaveBeenCalledWith({
        status: 'cancelled',
        message: 'Payment was cancelled by the user'
      } as PaymentResult);
    });

    it('should call callback with cancelled when modal is dismissed', () => {
      service.initiatePayment(testOrderId, testAmount, testName, testEmail, testContact, mockCallback);
      
      // Trigger modal ondismiss
      const ondismiss = (window.Razorpay as jasmine.Spy).calls.mostRecent().args[0].modal.ondismiss;
      ondismiss();
      
      expect(mockCallback).toHaveBeenCalledWith({
        status: 'cancelled',
        message: 'Payment cancelled by the user.'
      } as PaymentResult);
    });

  });
});