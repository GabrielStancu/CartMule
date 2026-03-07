using CartMule.Data;
using CartMule.Models;

namespace CartMule.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepo;

    public CategoryService(ICategoryRepository categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }

    public Task<List<Category>> GetAllCategoriesAsync() =>
        _categoryRepo.GetAllAsync();

    public Task<Category?> GetCategoryByIdAsync(int id) =>
        _categoryRepo.GetByIdAsync(id);
}
