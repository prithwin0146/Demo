using EmployeeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly TaskDbContext _context;

    public EmployeeRepository(TaskDbContext context)
    {
        _context = context;
    }

    public async Task<List<Employee>> GetAllAsync()
    {
        return await _context.Employees.ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees.FindAsync(id);
    }

    public async Task<Employee> AddAsync(Employee emp)
    {
        _context.Employees.Add(emp);
        await _context.SaveChangesAsync();
        return emp;
    }

    public async Task<Employee> UpdateAsync(Employee emp)
    {
        await _context.SaveChangesAsync();
        return emp;
    }

    public async Task<Employee> DeleteAsync(Employee emp)
    {
        _context.Employees.Remove(emp);
        await _context.SaveChangesAsync();
        return emp;
    }
}
