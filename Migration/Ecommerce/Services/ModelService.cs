using Ecommerce.Interfaces;
using Ecommerce.Models;
using Ecommerce.Models.DTOs;

namespace Ecommerce.Services
{
    public class ModelService : IModelService
    {
        private readonly IRepository<int, Model> _modelRepository;
        public ModelService(IRepository<int, Model> modelRepository)
        {
            _modelRepository = modelRepository;
        }
        public async Task<ModelResponseDto> CreateModel(ModelRequestDto model)
        {
            var models = await _modelRepository.GetAllAsync();
            var existing = models.SingleOrDefault(m => string.Equals(m.ModelName, model.ModelName, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
                throw new Exception("Model already exists.");
            Model newModel = new Model
            {
                ModelName = model.ModelName
            };
            newModel = await _modelRepository.AddAsync(newModel);
            return new ModelResponseDto
            {
                ModelId = newModel.ModelId,
                ModelName = newModel.ModelName
            };
        }

        public async Task<IEnumerable<Model>> GetAllModels()
        {
            var models = await _modelRepository.GetAllAsync();
            return models;
        }

        public async Task<ModelResponseDto> GetModelById(int id)
        {
            var model = await _modelRepository.GetByIdAsync(id);
            return new ModelResponseDto
            {
                ModelId = model.ModelId,
                ModelName = model.ModelName
            };
        }

        public async Task<ModelResponseDto> DeleteModel(int id)
        {
            var model = await _modelRepository.DeleteAsync(id);
            return new ModelResponseDto
            {
                ModelId = model.ModelId,
                ModelName = model.ModelName
            };
        }

        public async Task<ModelResponseDto> UpdateModel(int id, ModelRequestDto updateDto)
        {
            var model = await _modelRepository.GetByIdAsync(id);
            model.ModelName = updateDto.ModelName;
            var updatedModel = await _modelRepository.UpdateAsync(id, model);
            return new ModelResponseDto
            {
                ModelId = updatedModel.ModelId,
                ModelName = updatedModel.ModelName
            };
        }
    }
}