using Ecommerce.Models;
using Ecommerce.Models.DTOs;

namespace Ecommerce.Interfaces
{
    public interface IColorService
    {
        Task<ColorResponseDto> CreateColor(ColorRequestDto newColor);
        Task<IEnumerable<Color>> GetAllColors();
        Task<ColorResponseDto> GetColorById(int id);
        Task<ColorResponseDto> UpdateColor(int id, ColorRequestDto updateDto);
        Task<ColorResponseDto> DeleteColor(int id);

    }
}