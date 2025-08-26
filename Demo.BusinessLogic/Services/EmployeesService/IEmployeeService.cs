using Demo.BusinessLogic.DTOs.EmployeeDtos;
using System.Numerics;

namespace Demo.BusinessLogic.Services.EmployeesService
{
    public interface IEmployeeService
    {
        int AddEmployee(CreatedEmployeeDto employeeDto);

        bool DeleteEmployee(int id);

        int UpdateEmployee(UpdatedEmployeeDto employeeDto);

        EmployeeDetailsDto? GetEmployeeById(int id);

        IEnumerable<EmployeeDTO> GetAllEmployees(string? employeeSearchName);
    }
}
