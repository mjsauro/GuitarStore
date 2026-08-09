using Amazon.DynamoDBv2.DataModel;
using GuitarStore.Web.Models;

namespace GuitarStore.Web.Data;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IDynamoDBContext _context;

    public EmployeeRepository(IDynamoDBContext context) => _context = context;

    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken ct = default)
    {
        var employees = await _context.ScanAsync<Employee>([]).GetRemainingAsync(ct);
        return employees.OrderBy(e => e.EmpId).ToList();
    }

    public Task<Employee?> GetByIdAsync(int empId, CancellationToken ct = default) =>
        _context.LoadAsync<Employee?>(empId, ct);

    public Task SaveAsync(Employee employee, CancellationToken ct = default) =>
        _context.SaveAsync(employee, ct);

    public Task DeleteAsync(int empId, CancellationToken ct = default) =>
        _context.DeleteAsync<Employee>(empId, ct);

    public async Task<int> NextIdAsync(CancellationToken ct = default)
    {
        var employees = await GetAllAsync(ct);
        return employees.Count == 0 ? 1 : employees.Max(e => e.EmpId) + 1;
    }
}
