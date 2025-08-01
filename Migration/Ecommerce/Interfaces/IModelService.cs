using Ecommerce.Models;
using Ecommerce.Models.DTOs;

namespace Ecommerce.Interfaces
{
    public interface IModelService
    {
        Task<ModelResponseDto> CreateModel(ModelRequestDto model);
        Task<IEnumerable<Model>> GetAllModels();
        Task<ModelResponseDto> GetModelById(int id);
        Task<ModelResponseDto> UpdateModel(int id, ModelRequestDto updateDto);
        Task<ModelResponseDto> DeleteModel(int id);

    }
}