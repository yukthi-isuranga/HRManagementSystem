using Dapper;
using HRManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

[Authorize(Roles = "Admin,HR")]
[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly string _connection;

    public EmployeesController(IConfiguration config)
    {
        _connection = config.GetConnectionString("DefaultConnection");
    }

    //[HttpGet]
    //public async Task<IActionResult> Get()
    //{
    //    using var db = new SqlConnection(_connection);

    //    var sql = "SELECT * FROM Employees";

    //    var data = await db.QueryAsync<Employee>(sql);

    //    return Ok(data);
    //}
    [HttpGet]
    [Authorize(Roles = "Admin,HR,User")]                //Allow Read for All Roles
    public async Task<IActionResult> Get()
    {
        using var db = new SqlConnection(_connection);

    //    var sql = @"
    //    SELECT e.Id, e.FirstName, e.LastName, e.Email, e.DateOfBirth, e.Salary,
    //           d.DepartmentName
    //    FROM Employees e
    //    LEFT JOIN EmployeeDepartments ed ON e.Id = ed.EmployeeId
    //    LEFT JOIN Departments d ON ed.DepartmentId = d.Id
    //";

    //    var employeeDict = new Dictionary<int, Employee>();

    //    var result = await db.QueryAsync<Employee, string, Employee>(
    //        sql,
    //        (emp, deptName) =>
    //        {
    //            if (!employeeDict.TryGetValue(emp.Id, out var existing))
    //            {
    //                existing = emp;
    //                existing.Departments = new List<string>();
    //                employeeDict.Add(existing.Id, existing);
    //            }

    //            if (!string.IsNullOrEmpty(deptName))
    //            {
    //                existing.Departments.Add(deptName);
    //            }

    //            return existing;
    //        },
    //        splitOn: "DepartmentName"
    //    );

    //    return Ok(employeeDict.Values);

        var sql = @"
    SELECT e.Id, e.FirstName, e.LastName, e.Email, e.DateOfBirth, e.Salary,
           d.Id AS DeptId, d.DepartmentName
    FROM Employees e
    LEFT JOIN EmployeeDepartments ed ON e.Id = ed.EmployeeId
    LEFT JOIN Departments d ON ed.DepartmentId = d.Id
    ORDER BY e.Id
";

var lookup = new Dictionary<int, Employee>();

var rows = await db.QueryAsync(sql);

foreach (var row in rows)
{
    int empId = row.Id;
    if (!lookup.ContainsKey(empId))
    {
        lookup[empId] = new Employee
        {
            Id = empId,
            FirstName = row.FirstName,
            LastName = row.LastName,
            Email = row.Email,
            DateOfBirth = row.DateOfBirth,
            Salary = row.Salary,
            Departments = new List<string>()
        };
    }

    if (row.DepartmentName != null)
        lookup[empId].Departments.Add(row.DepartmentName);
}

return Ok(lookup.Values);
    }

    //[HttpPost]
    //public async Task<IActionResult> Create([FromBody] Employee emp)
    //{
    //    using var db = new SqlConnection(_connection);

    //    var sql = @"INSERT INTO Employees 
    //            (FirstName, LastName, Email, DateOfBirth, Salary)
    //            VALUES 
    //            (@FirstName, @LastName, @Email, @DateOfBirth, @Salary)";

    //    await db.ExecuteAsync(sql, emp);

    //    return Ok(new { message = "Employee created successfully" });
    //}

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmployeeCreateDto empDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        using var db = new SqlConnection(_connection);

        // Step 1: Insert Employee and get ID
        //    var employeeId = await db.ExecuteScalarAsync<int>(@"
        //    INSERT INTO Employees (FirstName, LastName, Email, DateOfBirth, Salary)
        //    VALUES (@FirstName, @LastName, @Email, @DateOfBirth, @Salary);
        //    SELECT CAST(SCOPE_IDENTITY() as int);
        //", empDto);
        var employeeId = await db.ExecuteScalarAsync<int>(
        "sp_CreateEmployee",
        new
        {
            empDto.FirstName,
            empDto.LastName,
            empDto.Email,
            empDto.DateOfBirth,
            empDto.Salary
        },
        commandType: System.Data.CommandType.StoredProcedure
    );

        // Step 2: Insert into EmployeeDepartments
        foreach (var deptId in empDto.DepartmentIds)
        {
            await db.ExecuteAsync(
            "sp_AssignEmployeeDepartments",
            new { EmployeeId = employeeId, DepartmentId = deptId },
            commandType: System.Data.CommandType.StoredProcedure
        );
            Console.WriteLine($"Departments Count: {empDto.DepartmentIds.Count}");
        }

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


    [HttpPost("assign-departments")]
    public async Task<IActionResult> AssignDepartments(EmployeeDepartmentDto dto)
    {
        using var db = new SqlConnection(_connection);

        // Remove existing mappings
        await db.ExecuteAsync(
            "DELETE FROM EmployeeDepartments WHERE EmployeeId = @EmployeeId",
            new { dto.EmployeeId });

        // Insert new mappings
        foreach (var deptId in dto.DepartmentIds)
        {
            await db.ExecuteAsync(
                "INSERT INTO EmployeeDepartments (EmployeeId, DepartmentId) VALUES (@EmployeeId, @DepartmentId)",
                new { dto.EmployeeId, DepartmentId = deptId });
        }

        return Ok(new { message = "Departments assigned successfully" });
    }
}