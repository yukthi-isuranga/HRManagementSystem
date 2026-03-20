using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;

[Authorize(Roles = "Admin,HR,User")]
[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly string _connection;

    private readonly IMemoryCache _cache;

    public DepartmentsController(IConfiguration config, IMemoryCache cache)
    {
        _connection = config.GetConnectionString("DefaultConnection");
        _cache = cache;
    }

    //[HttpGet]
    //public async Task<IActionResult> Get()
    //{
    //    using var db = new SqlConnection(_connection);

    //    var sql = "SELECT * FROM Departments";

    //    var data = await db.QueryAsync<Department>(sql);

    //    return Ok(data);
    //}
    [HttpGet]
    [Authorize(Roles = "Admin,HR,User")]
    public async Task<IActionResult> Get()
    {
        // Try to get cached departments
        if (!_cache.TryGetValue("departments", out List<Department> depts))
        {
            using var db = new SqlConnection(_connection);
            var sql = "SELECT * FROM Departments";
            depts = (await db.QueryAsync<Department>(sql)).ToList();

            // Set cache options (expires in 5 minutes)
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            // Store in cache
            _cache.Set("departments", depts, cacheOptions);
        }

        return Ok(depts);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Create([FromBody] Department dept)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        using var db = new SqlConnection(_connection);

        var sql = @"INSERT INTO Departments (DepartmentCode, DepartmentName)
                VALUES (@DepartmentCode, @DepartmentName)";

        await db.ExecuteAsync(sql, dept);

        _cache.Remove("departments");

        return Ok(new { message = "Department created successfully" });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HR")]
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

        _cache.Remove("departments");

        return Ok(new { message = "Department updated successfully" });
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        using var db = new SqlConnection(_connection);

        var sql = "DELETE FROM Departments WHERE Id = @Id";

        await db.ExecuteAsync(sql, new { Id = id });

        _cache.Remove("departments");

        return Ok(new { message = "Department deleted successfully" });
    }
}