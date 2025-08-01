using Ecommerce.Interfaces;
using Ecommerce.Models;
using Ecommerce.Models.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Ecommerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<int, Order> _orderRepository;
        public OrderService(IRepository<int, Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<OrderResponseDto> CreateOrder(OrderRequestDto order)
        {
            Order newOrder = new Order
            {
                OrderName = order.OrderName,
                OrderDate = order.OrderDate,
                PaymentType = order.PaymentType,
                Status = order.Status,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                CustomerEmail = order.CustomerEmail,
                CustomerAddress = order.CustomerAddress,
                OrderDetails = order.OrderDetails.Select(d => new OrderDetail
                {
                    ProductID = d.ProductID,
                    Quantity = d.Quantity,
                    Price = d.Price
                }).ToList()
            };

            newOrder = await _orderRepository.AddAsync(newOrder);

            return new OrderResponseDto
            {
                OrderID = newOrder.OrderID,
                OrderName = newOrder.OrderName,
                OrderDate = newOrder.OrderDate,
                PaymentType = newOrder.PaymentType,
                Status = newOrder.Status,
                CustomerName = newOrder.CustomerName,
                CustomerPhone = newOrder.CustomerPhone,
                CustomerEmail = newOrder.CustomerEmail,
                CustomerAddress = newOrder.CustomerAddress,
                OrderDetails = newOrder.OrderDetails.Select(od => new OrderDetailRequestDto
                {
                    ProductID = od.ProductID,
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList()
            };
        }

        public async Task<OrderResponseDto> DeleteOrder(int id)
        {
            var order = await _orderRepository.DeleteAsync(id);
            return new OrderResponseDto
            {
                OrderID = order.OrderID,
                OrderName = order.OrderName,
                OrderDate = order.OrderDate,
                PaymentType = order.PaymentType,
                Status = order.Status,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                CustomerEmail = order.CustomerEmail,
                CustomerAddress = order.CustomerAddress
            };
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllOrders(int page = 1, int pageSize = 2)
        {
            var orders = await _orderRepository.GetAllAsync();
            // var paginated = orders.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            // orders = orders.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return orders.Select(order => new OrderResponseDto
            {
                OrderID = order.OrderID,
                OrderName = order.OrderName,
                OrderDate = order.OrderDate,
                PaymentType = order.PaymentType,
                Status = order.Status,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                CustomerEmail = order.CustomerEmail,
                CustomerAddress = order.CustomerAddress,
                OrderDetails = order.OrderDetails!=null ? order.OrderDetails.Select(od => new OrderDetailRequestDto
                {
                    ProductID = od.ProductID,
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList() : new List<OrderDetailRequestDto>()
            });
        }

        public async Task<OrderResponseDto> GetOrderById(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return new OrderResponseDto
            {
                OrderID = order.OrderID,
                OrderName = order.OrderName,
                OrderDate = order.OrderDate,
                PaymentType = order.PaymentType,
                Status = order.Status,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                CustomerEmail = order.CustomerEmail,
                CustomerAddress = order.CustomerAddress,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailRequestDto
                {
                    ProductID = od.ProductID,
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList()
            };
        }

        public async Task<OrderResponseDto> UpdateOrder(int id, OrderRequestDto updateDto)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            order.OrderName = updateDto.OrderName;
            order.OrderDate = updateDto.OrderDate;
            order.PaymentType = updateDto.PaymentType;
            order.Status = updateDto.Status;
            order.CustomerName = updateDto.CustomerName;
            order.CustomerPhone = updateDto.CustomerPhone;
            order.CustomerEmail = updateDto.CustomerEmail;
            order.CustomerAddress = updateDto.CustomerAddress;
            order.OrderDetails = updateDto.OrderDetails.Select(od => new OrderDetail
            {
                ProductID = od.ProductID,
                Quantity = od.Quantity,
                Price = od.Price
            }).ToList();

            var updatedOrder = await _orderRepository.UpdateAsync(id, order);

            return new OrderResponseDto
            {
                OrderID = id,
                OrderName = updatedOrder.OrderName,
                OrderDate = updatedOrder.OrderDate,
                PaymentType = updatedOrder.PaymentType,
                Status = updatedOrder.Status,
                CustomerName = updatedOrder.CustomerName,
                CustomerPhone = updatedOrder.CustomerPhone,
                CustomerEmail = updatedOrder.CustomerEmail,
                CustomerAddress = updatedOrder.CustomerAddress,
                OrderDetails = updatedOrder.OrderDetails.Select(od => new OrderDetailRequestDto
                {
                    ProductID = od.ProductID,
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList()
            };
        }
        
        public byte[] GenerateOrderListPdf(IEnumerable<OrderResponseDto> orders)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.PageColor(Colors.White);

                    page.Header()
                        .Text("Order Listing Report")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40); // ID
                                columns.RelativeColumn();   // Name
                                columns.RelativeColumn();   // Phone
                                columns.RelativeColumn();   // Email
                                columns.RelativeColumn();   // Payment
                                columns.RelativeColumn();   // Status
                                columns.RelativeColumn();   // Date
                            });

                            // Table Header
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("ID").Bold();
                                header.Cell().Element(CellStyle).Text("Customer").Bold();
                                header.Cell().Element(CellStyle).Text("Phone").Bold();
                                header.Cell().Element(CellStyle).Text("Email").Bold();
                                header.Cell().Element(CellStyle).Text("Payment").Bold();
                                header.Cell().Element(CellStyle).Text("Status").Bold();
                                header.Cell().Element(CellStyle).Text("Date").Bold();
                            });

                            // Table Rows
                            foreach (var order in orders)
                            {
                                table.Cell().Element(CellStyle).Text(order.OrderID.ToString());
                                table.Cell().Element(CellStyle).Text(order.CustomerName);
                                table.Cell().Element(CellStyle).Text(order.CustomerPhone);
                                table.Cell().Element(CellStyle).Text(order.CustomerEmail);
                                table.Cell().Element(CellStyle).Text(order.PaymentType);
                                table.Cell().Element(CellStyle).Text(order.Status);
                                table.Cell().Element(CellStyle).Text(order.OrderDate.ToString("dd/MM/yyyy"));
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Generated on {DateTime.Now:dd MMM yyyy HH:mm}");
                });
            }).GeneratePdf();

            static IContainer CellStyle(IContainer container)
            {
                return container
                    .BorderBottom(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .PaddingVertical(4)
                    .PaddingHorizontal(2);
            }
        }
    }
}