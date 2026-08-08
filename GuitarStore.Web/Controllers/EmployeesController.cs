using GuitarStore.Web.Data;
using GuitarStore.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuitarStore.Web.Controllers;

/// <summary>
/// Staff records — names, dates of birth, and wages. In the MVC 5 version this controller
/// had no authorization at all and was fully reachable by anonymous visitors.
/// </summary>
[Authorize(Roles = Roles.Admin)]
public class EmployeesController : Controller
{
    private readonly IEmployeeRepository _employees;

    public EmployeesController(IEmployeeRepository employees) => _employees = employees;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        ViewData["Title"] = "Employees";
        return View(await _employees.GetAllAsync(ct));
    }

    public async Task<IActionResult> Details(int? id, CancellationToken ct)
    {
        if (id is null)
        {
            return BadRequest();
        }

        var employee = await _employees.GetByIdAsync(id.Value, ct);
        return employee is null ? NotFound() : View(employee);
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "New Employee";
        return View(new Employee { DOB = new DateTime(1990, 1, 1) });
    }

    [HttpPost]
    public async Task<IActionResult> Create(Employee employee, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(employee);
        }

        employee.EmpId = await _employees.NextIdAsync(ct);
        await _employees.SaveAsync(employee, ct);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id, CancellationToken ct)
    {
        if (id is null)
        {
            return BadRequest();
        }

        var employee = await _employees.GetByIdAsync(id.Value, ct);
        return employee is null ? NotFound() : View(employee);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Employee employee, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(employee);
        }

        if (await _employees.GetByIdAsync(employee.EmpId, ct) is null)
        {
            return NotFound();
        }

        await _employees.SaveAsync(employee, ct);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id, CancellationToken ct)
    {
        if (id is null)
        {
            return BadRequest();
        }

        var employee = await _employees.GetByIdAsync(id.Value, ct);
        return employee is null ? NotFound() : View(employee);
    }

    [HttpPost]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        await _employees.DeleteAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}
