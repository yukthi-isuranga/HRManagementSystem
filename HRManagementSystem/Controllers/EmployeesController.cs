using Dapper;
using HRManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly string _connection;

    public EmployeesController(IConfiguration config)
    {
        _connection = config.GetConnectionString("DefaultConnection");
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        using var db = new SqlConnection(_connection);

        var sql = "SELECT * FROM Employees";

        var data = await db.QueryAsync<Employee>(sql);

        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Employee emp)
    {
        using var db = new SqlConnection(_connection);

        var sql = @"INSERT INTO Employees 
                (FirstName, LastName, Email, DateOfBirth, Salary)
                VALUES 
                (@FirstName, @LastName, @Email, @DateOfBirth, @Salary)";

        await db.ExecuteAsync(sql, emp);

        return Ok(new { message = "Employee created successfully" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Employee emp)
    {
        using var db = new SqlConnection(_connection);

        var sql = @"UPDATE Employees 
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email,
                    DateOfBirth = @DateOfBirth,
                    Salary = @Salary
                WHERE Id = @Id";

        await db.ExecuteAsync(sql, new
        {
            emp.FirstName,
            emp.LastName,
            emp.Email,
            emp.DateOfBirth,
            emp.Salary,
            Id = id
        });

        return Ok(new { message = "Employee updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        using var db = new SqlConnection(_connection);

        var sql = "DELETE FROM Employees WHERE Id = @Id";

        await db.ExecuteAsync(sql, new { Id = id });

        return Ok(new { message = "Employee deleted successfully" });
    }
}