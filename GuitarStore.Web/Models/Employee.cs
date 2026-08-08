using System.ComponentModel.DataAnnotations;
using Amazon.DynamoDBv2.DataModel;

namespace GuitarStore.Web.Models;

[DynamoDBTable("GuitarStore-Employees")]
public class Employee
{
    [DynamoDBHashKey]
    [Display(Name = "Employee ID")]
    public int EmpId { get; set; }

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = "";

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = "";

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime DOB { get; set; }

    [Range(0, 1_000_000)]
    public decimal Wage { get; set; }

    [DynamoDBIgnore]
    public string FullName => $"{FirstName} {LastName}".Trim();
}
