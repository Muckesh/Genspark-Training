import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Products } from './products';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ProductService } from '../services/product.service';

describe('Products', () => {
  let component: Products;
  let fixture: ComponentFixture<Products>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Products],
      providers:[ProductService,provideHttpClient(),provideHttpClientTesting()]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Products);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
