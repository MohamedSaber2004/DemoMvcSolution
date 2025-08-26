
using Demo.BusinessLogic.DTOs.EmployeeDtos;
using Demo.BusinessLogic.Services.EmployeesService;
using Demo.DataAccess.Models.Employee_Model;
using Demo.DataAccess.Models.Shared.Enums;

namespace Demo.BusinessLogic.Factories.EmployeesFactory
{
    public static class EmployeeFactory
    {
        public static Employee ToEntity(this CreatedEmployeeDto employeeDto)
        {
            return new Employee()
            {
                Name = employeeDto.Name,
                Age = employeeDto.Age,
                IsActive = employeeDto.IsActive,
                Address = employeeDto.Address,
                Email = employeeDto.Email,
                Gender = employeeDto.Gender,
                HiringDate = employeeDto.HiringDate.ToDateTime(TimeOnly.MinValue),
                EmployeeType = employeeDto.EmployeeType,
                PhoneNumber = employeeDto.PhoneNumber,
                Salary = employeeDto.Salary
            };
        }

        public static Employee ToEntity(this UpdatedEmployeeDto employeeDto)
        {
            return new Employee()
            {
                Id = employeeDto.Id,
                Name = employeeDto.Name,
                Age = employeeDto.Age,
                IsActive = employeeDto.IsActive,
                Address = employeeDto.Address,
                Email = employeeDto.Email,
                Gender = employeeDto.Gender,
                HiringDate = employeeDto.HiringDate.ToDateTime(TimeOnly.MinValue),
                EmployeeType = employeeDto.EmployeeType,
                PhoneNumber = employeeDto.PhoneNumber,
                Salary = employeeDto.Salary
            };
        }

        public static EmployeeDetailsDto ToEmployeeDetailsDto(this Employee employee)
        {
            return new EmployeeDetailsDto()
            {
                Id = employee.Id,
                Name = employee.Name,
                Age = employee.Age,
                IsActive = employee.IsActive,
                Address = employee.Address,
                Email = employee.Email,
                Gender = employee.Gender.ToString(),
                HiringDate = DateOnly.FromDateTime(employee.HiringDate),
                EmployeeType = employee.EmployeeType.ToString(),
                PhoneNumber = employee.PhoneNumber,
                Salary = employee.Salary
            };
        }

        public static EmployeeDTO ToEmployeeDto(this Employee employee)
        {
            return new EmployeeDTO()
            {
                Id = employee.Id,
                Name = employee.Name,
                Age = employee.Age,
                Email = employee.Email,
                EmpType = employee.EmployeeType.ToString(),
                EmpGender = employee.Gender.ToString(),
                IsActive = employee.IsActive,
                Salary = employee.Salary
            };
        }
    }
}
