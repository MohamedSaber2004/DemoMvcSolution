using Demo.BusinessLogic.DTOs.DepartmentDtos;

namespace Demo.BusinessLogic.Services.DepartmentsService
{
    public interface IDepartmentService
    {
        int AddDepartment(CreatedDepartmentDto departmentDto);
        bool DeleteDepartment(int id);
        IEnumerable<DepartmentDTO> GetAllDepartments(string? departmentSearchName);
        IEnumerable<DepartmentDTO> GetAllDepartments(bool withTracking = false);
        DepartmentDetailsDTO GetDepartmentById(int id);
        int UpdateDepartment(UpdatedDepartmentDto departmentDto);
    }
}