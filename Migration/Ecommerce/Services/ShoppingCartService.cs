using Ecommerce.Interfaces;
using Ecommerce.Models;
using Ecommerce.Models.DTOs;

namespace Ecommerce.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IOrderService _orderService;
        private readonly IRepository<int, Order> _orderRepository;
        private readonly IRepository<int, Product> _productRepository;

        public ShoppingCartService(IOrderService orderService, IRepository<int, Order> orderRepository, IRepository<int, Product> productRepository)
        {
            _orderService = orderService;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }
        public async Task<OrderResponseDto> PlaceOrderAsync(CheckoutDto dto)
        {
            var orderDetails = new List<OrderDetail>();

            foreach (var cartItem in dto.Items)
            {
                var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                if (product == null) throw new Exception($"Product not found: ID {cartItem.ProductId}");

                orderDetails.Add(new OrderDetail
                {
                    ProductID = product.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = product.Price
                });
            }

            var order = new Order
            {
                OrderName = dto.OrderName,
                CustomerName = dto.CustomerName,
                CustomerPhone = dto.CustomerPhone,
                CustomerEmail = dto.CustomerEmail,
                CustomerAddress = dto.CustomerAddress,
                PaymentType = dto.PaymentType,
                Status = "Processing",
                OrderDate = DateTime.UtcNow,
                OrderDetails = orderDetails
            };

            var savedOrder = await _orderRepository.AddAsync(order);

            return new OrderResponseDto
            {
                OrderID = savedOrder.OrderID,
                OrderName = savedOrder.OrderName,
                CustomerName = savedOrder.CustomerName,
                CustomerPhone = savedOrder.CustomerPhone,
                CustomerEmail = savedOrder.CustomerEmail,
                CustomerAddress = savedOrder.CustomerAddress,
                OrderDate = savedOrder.OrderDate,
                PaymentType = savedOrder.PaymentType,
                Status = savedOrder.Status,
                OrderDetails = savedOrder.OrderDetails.Select(d => new OrderDetailRequestDto
                {
                    ProductID = d.ProductID,
                    Quantity = d.Quantity,
                    Price = d.Price
                }).ToList()
            };
        }
            
        

    }
}