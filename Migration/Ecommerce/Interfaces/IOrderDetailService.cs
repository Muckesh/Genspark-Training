using Ecommerce.Models.DTOs;

namespace Ecommerce.Interfaces
{
    public interface IOrderDetailService
    {
        Task<OrderDetailResponseDto> CreateOrderDetail(OrderDetailRequestDto orderDetail);
        Task<IEnumerable<OrderDetailResponseDto>> GetAllOrderDetails();
        Task<OrderDetailResponseDto> GetOrderDetailById(int id);
        Task<OrderDetailResponseDto> UpdateOrderDetail(int id, OrderDetailRequestDto updateDto);
        // Task<OrderDetailResponseDto> DeleteOrderDetail(int id);
    }
}