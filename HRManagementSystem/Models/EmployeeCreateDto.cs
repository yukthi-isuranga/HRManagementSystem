using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class EmployeeCreateDto
{
    [Required(ErrorMessage = "First Name is required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Date of Birth is required")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Salary is required")]
    public decimal Salary { get; set; }

    // List of department names
    public List<string> Departments { get; set; } = new List<string>();
}