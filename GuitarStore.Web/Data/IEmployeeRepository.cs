using GuitarStore.Web.Models;

namespace GuitarStore.Web.Data;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken ct = default);

    Task<Employee?> GetByIdAsync(int empId, CancellationToken ct = default);

    Task SaveAsync(Employee employee, CancellationToken ct = default);

    Task DeleteAsync(int empId, CancellationToken ct = default);

    /// <summary>Next available id, since DynamoDB has no identity column.</summary>
    Task<int> NextIdAsync(CancellationToken ct = default);
}
