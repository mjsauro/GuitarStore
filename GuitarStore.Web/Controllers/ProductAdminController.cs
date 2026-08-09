using GuitarStore.Web.Data;
using GuitarStore.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuitarStore.Web.Controllers;

/// <summary>
/// Catalog management. Like Employees, the MVC 5 version of this controller was reachable
/// by anyone — create, edit, and delete included.
/// </summary>
[Authorize(Roles = Roles.Admin)]
public class ProductAdminController : Controller
{
    private static readonly string[] ProductTypes = ["Guitar", "Amplifier", "Effect"];

    private readonly IProductRepository _products;

    public ProductAdminController(IProductRepository products) => _products = products;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        ViewData["Title"] = "Manage Products";
        return View(await _products.GetAllAsync(ct));
    }

    public async Task<IActionResult> Details(int? id, CancellationToken ct)
    {
        if (id is null)
        {
            return BadRequest();
        }

        var product = await _products.GetByIdAsync(id.Value, ct);
        return product is null ? NotFound() : View(product);
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "New Product";
        ViewBag.ProductTypes = ProductTypes;
        return View(new Product());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product, CancellationToken ct)
    {
        ViewBag.ProductTypes = ProductTypes;
        if (!ModelState.IsValid)
        {
            return View(product);
        }

        var all = await _products.GetAllAsync(ct);
        product.Id = all.Count == 0 ? 1 : all.Max(p => p.Id) + 1;
        product.DateCreated = product.DateModified = DateTime.UtcNow;

        await _products.SaveAsync(product, ct);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id, CancellationToken ct)
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

        ViewBag.ProductTypes = ProductTypes;
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Product product, CancellationToken ct)
    {
        ViewBag.ProductTypes = ProductTypes;
        if (!ModelState.IsValid)
        {
            return View(product);
        }

        var existing = await _products.GetByIdAsync(product.Id, ct);
        if (existing is null)
        {
            return NotFound();
        }

        // Preserve fields the form doesn't post.
        product.Properties = existing.Properties;
        product.DateCreated = existing.DateCreated;
        product.DateModified = DateTime.UtcNow;

        await _products.SaveAsync(product, ct);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id, CancellationToken ct)
    {
        if (id is null)
        {
            return BadRequest();
        }

        var product = await _products.GetByIdAsync(id.Value, ct);
        return product is null ? NotFound() : View(product);
    }

    [HttpPost]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        await _products.DeleteAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}
