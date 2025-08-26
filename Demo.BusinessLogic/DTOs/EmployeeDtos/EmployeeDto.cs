﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Demo.BusinessLogic.DTOs.EmployeeDtos
{
    // application validation
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? Age { get; set; }
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string EmpGender { get; set; }
        [Display(Name = "Employee Type")]
        public string EmpType { get; set; }
        public string? Department { get; set; }
        [Display(Name = "Image")]
        public string? ImageName { get; set; }
    }
}
