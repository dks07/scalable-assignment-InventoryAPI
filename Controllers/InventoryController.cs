using InventoryAPI.Models;
using InventoryAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
  [ApiController]
  [Authorize]
  [Route("api/[controller]")]
  public class InventoryController : ControllerBase
  {
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
      _inventoryService = inventoryService;
    }
    
    [HttpGet("{id}", Name = nameof(GetProductByIdAsync))]
    public async Task<ActionResult<ProductModel>> GetProductByIdAsync(string id)
    {
      var product = await _inventoryService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound();
      }

      return Ok(product);
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductModel>>> GetProductsAsync()
    {
      var products = await _inventoryService.GetProductsAsync();
      return Ok(products);
    }

    [HttpPost]
    public async Task<ActionResult<ProductModel>> CreateProductAsync(ProductModel productModel)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      var product = await _inventoryService.CreateProductAsync(productModel);
      return CreatedAtRoute("GetProductByIdAsync", new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductModel>> UpdateProductAsync(string id, ProductModel productModel)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var product = await _inventoryService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound();
      }

      await _inventoryService.UpdateProductAsync(id, productModel);
      return Ok();
    }
    

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProductAsync(string id)
    {
      var result = await _inventoryService.DeleteProductAsync(id);
      if (!result)
      {
        return NotFound();
      }

      return Ok();
    }
  }
}