using System.ComponentModel.DataAnnotations;

public class Department
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Department Code is required")]
    public string DepartmentCode { get; set; }
    [Required(ErrorMessage = "Department Name is required")]
    public string DepartmentName { get; set; }
}