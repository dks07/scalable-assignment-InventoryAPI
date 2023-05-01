using InventoryAPI.Models;

namespace InventoryAPI.Services
{
  public interface IInventoryService
  {
    Task<List<ProductModel>> GetProductsAsync();

    Task<ProductModel?> GetProductByIdAsync(string id);

    Task<ProductModel> CreateProductAsync(ProductModel product);

    Task<ProductModel?> UpdateProductAsync(string id, ProductModel product);

    Task<bool> DeleteProductAsync(string id);
  }
}