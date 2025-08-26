using Demo.BusinessLogic.DTOs.DepartmentDtos;
using Demo.BusinessLogic.Factories.DepartmentsFactory;
using Demo.DataAccess.Models;
using Demo.DataAccess.Repositories.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Demo.BusinessLogic.Services.DepartmentsService
{
    public class DepartmentService(IUnitOfWork unitOfWork) : IDepartmentService
    {

        // Get All Departments
        public IEnumerable<DepartmentDTO> GetAllDepartments(string? departmentSearchName)
        {
            // must mapping
            // types of mapping:
            //1) manual mapping
            //2) auto mapper
            //3) constructor mapping
            //4) Extension methods
            IEnumerable<Department> departments;
            if(string.IsNullOrWhiteSpace(departmentSearchName))
            {
                departments = unitOfWork.DepartmentRepository.GetAll();
            }
            else
            {
                departments = unitOfWork.DepartmentRepository.GetAll(D => D.Name.ToLower().Contains(departmentSearchName.ToLower()));
            }

            #region Manual Mapping

            //var departmentsToReturn = departments.Select(D => new DepartmentDTO()
            //{
            //    DeptId = D.Id,
            //    Code = D.Code,
            //    Description = D.Description ?? " ",
            //    Name = D.Name,
            //    DateOfCreation = DateOnly.FromDateTime(D.CreatedOn)
            //});

            //return departmentsToReturn;
            #endregion

            return departments.Select(D => D.ToDepartmentDto());
        }
        public IEnumerable<DepartmentDTO> GetAllDepartments(bool withTracking = false)
        {
            return GetAllDepartments(null);
        }


        // Get Department By Id
        public DepartmentDetailsDTO GetDepartmentById(int id)
        {
            var department = unitOfWork.DepartmentRepository.GetById(id);

            #region Manual Mapping
            // manual mapping
            //if (department is null) return null!;
            //else
            //{
            //    var departmentToReturn = new DepartmentDetailsDTO(department)
            //    {
            //        Id = department.Id,
            //        Name = department.Name,
            //        CreatedOn = DateOnly.FromDateTime(department.CreatedOn),
            //        LastModifiedOn = DateOnly.FromDateTime(department.LastModifiedOn),
            //        Code = department.Code,
            //        Description = department.Description,
            //        LastModifiedBy = department.LastModifiedBy,
            //        CreatedBy = department.CreatedBy,
            //        IsDeleted = department.IsDeleted
            //    };
            //    return departmentToReturn;
            //} 
            #endregion

            return department.ToDepartmentDetailsDto();
        }

        // Create New Department
        public int AddDepartment(CreatedDepartmentDto departmentDto)
        {
            var department = departmentDto.ToEntity();
            unitOfWork.DepartmentRepository.Add(department);
            return unitOfWork.SaveChanges();
        }

        // Update Department
        public int UpdateDepartment(UpdatedDepartmentDto departmentDto)
        {
            unitOfWork.DepartmentRepository.Update(departmentDto.ToEntity());
            return unitOfWork.SaveChanges();
        }

        // Delete Department
        public bool DeleteDepartment(int id)
        {
            var department = unitOfWork.DepartmentRepository.GetById(id);
            if (department is null) return false;
            else
            {
                department.IsDeleted = true;
                unitOfWork.DepartmentRepository.Update(department);
            }
            return unitOfWork.SaveChanges() > 0 ? true : false;
        }
    }
}
