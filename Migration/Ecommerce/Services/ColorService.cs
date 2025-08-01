using Ecommerce.Interfaces;
using Ecommerce.Models;
using Ecommerce.Models.DTOs;

namespace Ecommerce.Services
{
    public class ColorService : IColorService
    {
        private readonly IRepository<int, Color> _colorRepository;

        public ColorService(IRepository<int, Color> colorRepository)
        {
            _colorRepository = colorRepository;
        }
        public async Task<ColorResponseDto> CreateColor(ColorRequestDto newColor)
        {
            var categories = await _colorRepository.GetAllAsync();
            var existing = categories.SingleOrDefault(c => string.Equals(c.ColorName, newColor.ColorName, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
                throw new Exception("Color Already Exists.");
            Color color = new Color
            {
                ColorName = newColor.ColorName
            };
            color = await _colorRepository.AddAsync(color);
            return new ColorResponseDto
            {
                ColorId = color.ColorId,
                ColorName = color.ColorName
            };
        }

        public async Task<ColorResponseDto> DeleteColor(int id)
        {
            var color = await _colorRepository.DeleteAsync(id);
            return new ColorResponseDto
            {
                ColorId = color.ColorId,
                ColorName = color.ColorName
            };
        }

        public async Task<IEnumerable<Color>> GetAllColors()
        {
            var colors = await _colorRepository.GetAllAsync();
            return colors;
        }

        public async Task<ColorResponseDto> GetColorById(int id)
        {
            var color = await _colorRepository.GetByIdAsync(id);
            return new ColorResponseDto
            {
                ColorId = color.ColorId,
                ColorName = color.ColorName
            };
        }

        public async Task<ColorResponseDto> UpdateColor(int id, ColorRequestDto updateDto)
        {
            var color = await _colorRepository.GetByIdAsync(id);
            if (color == null)
            {
                throw new KeyNotFoundException("Color not found.");
            }
            color.ColorName = updateDto.ColorName;
            color = await _colorRepository.UpdateAsync(id, color);
            return new ColorResponseDto
            {
                ColorId = color.ColorId,
                ColorName=color.ColorName
            };
            

        }
    }
}