import { TestBed } from '@angular/core/testing';
import { ModalService } from './modal.service';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { InquiryForm } from '../components/buyer/inquiry-form/inquiry-form';
import { EventEmitter } from '@angular/core';

describe('ModalService', () => {
  let service: ModalService;
  let ngbModalSpy: jasmine.SpyObj<NgbModal>;
  let modalRefMock: jasmine.SpyObj<NgbModalRef>;

  beforeEach(() => {
    // Mock modal reference with emitters
    modalRefMock = jasmine.createSpyObj<NgbModalRef>('NgbModalRef', ['close', 'dismiss'], {
      componentInstance: {
        inquirySubmitted: new EventEmitter<void>(),
        cancel: new EventEmitter<void>(),
        listingId: ''
      }
    });

    // Mock NgbModal service
    ngbModalSpy = jasmine.createSpyObj<NgbModal>('NgbModal', ['open']);
    ngbModalSpy.open.and.returnValue(modalRefMock);

    TestBed.configureTestingModule({
      providers: [
        ModalService,
        { provide: NgbModal, useValue: ngbModalSpy }
      ]
    });

    service = TestBed.inject(ModalService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should open InquiryForm modal with listingId and handle submit', () => {
    service.openInquiryModal('listing123');

    // Verify modal opened with InquiryForm
    expect(ngbModalSpy.open).toHaveBeenCalledWith(InquiryForm);

    // Check listingId assignment
    expect(modalRefMock.componentInstance.listingId).toBe('listing123');

    // Spy on modalRef methods
    // const closeSpy = spyOn(modalRefMock, 'close');

    // Trigger submit event
    modalRefMock.componentInstance.inquirySubmitted.emit();

    expect(modalRefMock.close).toHaveBeenCalled();
  });

  it('should dismiss modal on cancel', () => {
    service.openInquiryModal('listingXYZ');

    // Spy on dismiss method
    // const dismissSpy = spyOn(modalRefMock, 'dismiss');

    // Trigger cancel event
    modalRefMock.componentInstance.cancel.emit();

    expect(modalRefMock.dismiss).toHaveBeenCalled();
  });
});
