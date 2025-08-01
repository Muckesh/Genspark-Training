import { ProductResponse } from "./product.model";

export interface OrderResponseDto {
    orderID: number;
    orderName: string;
    orderDate: Date;
    paymentType: string;
    status: string;
    customerName: string;
    customerPhone: string;
    customerEmail: string;
    customerAddress: string;
    payPalOrderId:string|null;
    orderDetails: OrderDetailRequestDto[];
}

export interface OrderDetailRequestDto {
    productID: number;
    quantity: number;
    price: number;
}

export interface OrderRequestDto {
    orderName: string;
    orderDate: Date;
    paymentType: string;
    status: string;
    customerName: string;
    customerPhone: string;
    customerEmail: string;
    customerAddress: string;
    orderDetails: OrderDetailRequestDto[];
}

export interface CartItem {
  product: ProductResponse;
  quantity: number;
}

export interface CheckoutDto{
    orderName: string;
    paymentType: string;
    customerName: string;
    customerPhone: string;
    customerEmail: string;
    customerAddress: string;
    items: CartItem[];
}

export interface CheckoutBEDto{
    orderName: string;
    paymentType: string;
    customerName: string;
    customerPhone: string;
    customerEmail: string;
    customerAddress: string;
    items: CartDto[];
}

export interface CartDto{
    productId: number;
    quantity: number;
}