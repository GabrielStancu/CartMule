using TouCart.Data;
using TouCart.Models;

namespace TouCart.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepo;

    public CategoryService(ICategoryRepository categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }

    public Task<List<Category>> GetAllCategoriesAsync() =>
        _categoryRepo.GetAllAsync();

    public Task<List<Category>> GetCategoriesForListAsync(int listId) =>
        _categoryRepo.GetForListAsync(listId);

    public Task<Category?> GetCategoryByIdAsync(int id) =>
        _categoryRepo.GetByIdAsync(id);

    public Task<Category> CreateCategoryAsync(string name) =>
        _categoryRepo.CreateAsync(new Category { Name = name.Trim() });

    public Task<Category> CreateCategoryForListAsync(string name, int listId) =>
        _categoryRepo.CreateAsync(new Category { Name = name.Trim(), ListId = listId });

    public async Task<Category> UpdateCategoryAsync(int id, string name)
    {
        var cat = await _categoryRepo.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"Category {id} not found.");
        cat.Name = name.Trim();
        return await _categoryRepo.UpdateAsync(cat);
    }

    public Task DeleteCategoryAsync(int id) =>
        _categoryRepo.DeleteAsync(id);
}
