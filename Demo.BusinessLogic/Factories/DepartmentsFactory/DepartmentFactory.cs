using Demo.BusinessLogic.DTOs.DepartmentDtos;
using Demo.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BusinessLogic.Factories.DepartmentsFactory
{
    public static class DepartmentFactory
    {
        public static DepartmentDTO ToDepartmentDto(this Department D)
        {
            return new DepartmentDTO()
            {
                DeptId = D.Id,
                Code = D.Code,
                Description = D.Description ?? " ",
                Name = D.Name,
                DateOfCreation = DateOnly.FromDateTime(D.CreatedOn)
            };
        }

        public static DepartmentDetailsDTO ToDepartmentDetailsDto(this Department D)
        {
            return new DepartmentDetailsDTO(D)
            {
                Id = D.Id,
                Name = D.Name,
                CreatedOn = DateOnly.FromDateTime(D.CreatedOn),
                LastModifiedOn = DateOnly.FromDateTime(D.LastModifiedOn),
                Code = D.Code,
                Description = D.Description,
                LastModifiedBy = D.LastModifiedBy,
                CreatedBy = D.CreatedBy,
                IsDeleted = D.IsDeleted
            };
        }

        public static Department ToEntity(this CreatedDepartmentDto departmentDto)
        {
            return new Department()
            {
                Name = departmentDto.Name,
                Code = departmentDto.Code,
                Description = departmentDto.Description,
                CreatedOn = departmentDto.DateOfCreation.ToDateTime(new TimeOnly())
            };
        }
        
        public static Department ToEntity(this UpdatedDepartmentDto departmentDto)
        {
            return new Department()
            {
                Id = departmentDto.Id,
                Name = departmentDto.Name,
                Code = departmentDto.Code,
                Description = departmentDto.Description,
                CreatedOn = departmentDto.DateOfCreation.ToDateTime(new TimeOnly())
            };
        }
    }
}
