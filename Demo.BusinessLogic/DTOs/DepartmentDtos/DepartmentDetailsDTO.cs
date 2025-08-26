using Demo.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BusinessLogic.DTOs.DepartmentDtos
{
    public class DepartmentDetailsDTO
    {
        // constructor mapping
        public DepartmentDetailsDTO(Department department)
        {
            Id = department.Id;
            Name = department.Name;
            CreatedOn = DateOnly.FromDateTime(department.CreatedOn);
            CreatedBy = department.CreatedBy;
            LastModifiedBy = department.LastModifiedBy;
            LastModifiedOn = DateOnly.FromDateTime(department.LastModifiedOn);
            IsDeleted = department.IsDeleted;
            Code = department.Code;
            Description = department.Description;
        }
        // PK
        public int Id { get; set; }
        // User Id
        public int CreatedBy { get; set; }
        // date of creation
        public DateOnly CreatedOn { get; set; }
        // User Id
        public int LastModifiedBy { get; set; }
        // date of modification [auto calculated]
        public DateOnly LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
    }
}
