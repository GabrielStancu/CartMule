using TouCart.Models;

namespace TouCart.Services;

public interface ICategoryService
{
    Task<List<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<Category> CreateCategoryAsync(string name);
    Task<Category> UpdateCategoryAsync(int id, string name);
    Task DeleteCategoryAsync(int id);
}
