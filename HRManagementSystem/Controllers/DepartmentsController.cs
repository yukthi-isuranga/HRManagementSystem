using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly string _connection;

    public DepartmentsController(IConfiguration config)
    {
        _connection = config.GetConnectionString("DefaultConnection");
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        using var db = new SqlConnection(_connection);

        var sql = "SELECT * FROM Departments";

        var data = await db.QueryAsync<Department>(sql);

        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Department dept)
    {
        using var db = new SqlConnection(_connection);

        var sql = @"INSERT INTO Departments (DepartmentCode, DepartmentName)
                VALUES (@DepartmentCode, @DepartmentName)";

        await db.ExecuteAsync(sql, dept);

        return Ok(new { message = "Department created successfully" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Department dept)
    {
        using var db = new SqlConnection(_connection);

        var sql = @"UPDATE Departments 
                SET DepartmentCode = @DepartmentCode,
                    DepartmentName = @DepartmentName
                WHERE Id = @Id";

        await db.ExecuteAsync(sql, new
        {
            dept.DepartmentCode,
            dept.DepartmentName,
            Id = id
        });

        return Ok(new { message = "Department updated successfully" });
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        using var db = new SqlConnection(_connection);

        var sql = "DELETE FROM Departments WHERE Id = @Id";

        await db.ExecuteAsync(sql, new { Id = id });

        return Ok(new { message = "Department deleted successfully" });
    }
}