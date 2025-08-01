import { Component, OnInit } from '@angular/core';
import { OrderResponseDto } from '../../../models/order.model';
import { OrderService } from '../../../services/order.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-order',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './order.html',
  styleUrls: ['./order.css']
})
export class Order implements OnInit {
  orders: OrderResponseDto[] = [];
  currentPage = 1;
  pageSize = 10;
  totalOrders = 0;
  isExporting = false;
  selectedOrder: OrderResponseDto | null = null;

  constructor(
    private orderService: OrderService,
    private modalService: NgbModal
  ) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.orderService.getAll(this.currentPage, this.pageSize).subscribe({
      next: (data) => {
        this.orders = data;
        this.totalOrders = data.length;
      },
      error: (err) => console.error('Failed to load orders', err)
    });
  }

  exportToPdf(): void {
    this.isExporting = true;
    this.orderService.exportToPdf().subscribe({
      next: (blob) => {
        this.downloadFile(blob, `orders_${new Date().toISOString().slice(0, 10)}.pdf`);
        this.isExporting = false;
      },
      error: (err) => {
        console.error('PDF export failed', err);
        this.isExporting = false;
      }
    });
  }

  private downloadFile(blob: Blob, fileName: string) {
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadOrders();
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'processing':
        return 'badge bg-warning text-dark';
      case 'completed':
        return 'badge bg-success';
      case 'cancelled':
        return 'badge bg-danger';
      default:
        return 'badge bg-secondary';
    }
  }

  calculateOrderTotal(order: OrderResponseDto): number {
    return order.orderDetails.reduce((total, item) => total + (item.price * item.quantity), 0);
  }

  getPageNumbers(): number[] {
    const pageCount = Math.ceil(this.totalOrders / this.pageSize);
    return Array.from({ length: pageCount }, (_, i) => i + 1);
  }

  viewOrderDetails(order: OrderResponseDto, content: any): void {
    this.selectedOrder = order;
    this.modalService.open(content, { size: 'lg', centered: true });
  }
}