using System.ComponentModel.DataAnnotations;

namespace HRManagementSystem.Models
{
    public class Employee
    {
        public int Id { get; set; }
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
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;

                if (DateOfBirth.Date > today.AddYears(-age))
                    age--;

                return age;
            }
        }
        public List<string> Departments { get; set; } = new List<string>();
    }
}

