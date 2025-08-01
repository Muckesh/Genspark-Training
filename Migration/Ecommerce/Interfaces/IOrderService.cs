using Ecommerce.Models.DTOs;

namespace Ecommerce.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrder(OrderRequestDto order);
        Task<IEnumerable<OrderResponseDto>> GetAllOrders(int page = 1, int pageSize = 10);
        Task<OrderResponseDto> GetOrderById(int id);
        Task<OrderResponseDto> UpdateOrder(int id, OrderRequestDto updateDto);
        Task<OrderResponseDto> DeleteOrder(int id);
        byte[] GenerateOrderListPdf(IEnumerable<OrderResponseDto> orders);

    }
}