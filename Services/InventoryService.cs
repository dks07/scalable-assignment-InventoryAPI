using InventoryAPI.Models;
using InventoryAPI.Settings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InventoryAPI.Services
{
  public class InventoryService : IInventoryService
  {
    private readonly IMongoCollection<Product> _products;

    public InventoryService(IMongoClient client, IInventotyDatabaseSettings settings)
    {
      var database = client.GetDatabase(settings.DatabaseName);

      _products = database.GetCollection<Product>(settings.ProductCollectionName);
    }

    public async Task<List<ProductModel>> GetProductsAsync()
    {
      var cursor = await _products.FindAsync(product => true);
      var products = await cursor.ToListAsync();
      return products.Select(product => new ProductModel
      {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        Quantity = product.Quantity
      })
        .ToList();
    }

    public async Task<ProductModel?> GetProductByIdAsync(string id)
    {
      var cursor = await _products.FindAsync(product => product.Id == id);
      var product = await cursor.FirstOrDefaultAsync();
      if (product == null) return null;
      return new ProductModel
      {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        Quantity = product.Quantity
      };
    }

    public async Task<ProductModel> CreateProductAsync(ProductModel product)
    {
      if (string.IsNullOrEmpty(product.Id))
      {
        product.Id = ObjectId.GenerateNewId().ToString();
      }
      await _products.InsertOneAsync(new Product
      {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        Quantity = product.Quantity
      });
      return product;
    }

    public async Task<ProductModel?> UpdateProductAsync(string id, ProductModel product)
    {
      await _products.ReplaceOneAsync(p => p.Id == id, new Product
      {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        Quantity = product.Quantity
      });
      return product;
    }

    public async Task<bool> DeleteProductAsync(string id)
    {
      var deleteResult = await _products.DeleteOneAsync(p => p.Id == id);
      return deleteResult.DeletedCount == 1;
    }
  }
}