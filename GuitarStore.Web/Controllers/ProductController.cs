using GuitarStore.Web.Data;
using Microsoft.AspNetCore.Mvc;

namespace GuitarStore.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductRepository _products;

    public ProductController(IProductRepository products) => _products = products;

    /// <summary>Catalog listing. With no id, shows everything; with one, filters by product type.</summary>
    public async Task<IActionResult> List(string? id, CancellationToken ct)
    {
        var products = string.IsNullOrEmpty(id)
            ? await _products.GetAllAsync(ct)
            : await _products.GetByTypeAsync(id, ct);

        ViewData["Title"] = string.IsNullOrEmpty(id) ? "All Products" : id;
        return View(products);
    }

    /// <summary>Product detail page.</summary>
    public async Task<IActionResult> Index(int? id, CancellationToken ct)
    {
        if (id is null)
        {
            return BadRequest();
        }

        var product = await _products.GetByIdAsync(id.Value, ct);
        if (product is null)
        {
            return NotFound();
        }

        ViewData["Title"] = product.DisplayName;
        return View(product);
    }
}
