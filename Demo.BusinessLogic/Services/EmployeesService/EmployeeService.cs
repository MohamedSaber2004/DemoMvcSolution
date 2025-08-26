using AutoMapper;
using Demo.BusinessLogic.DTOs.EmployeeDtos;
using Demo.BusinessLogic.Services.AttachmentService;
using Demo.DataAccess.Models.Employee_Model;
using Demo.DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BusinessLogic.Services.EmployeesService
{
    public class EmployeeService(IUnitOfWork unitOfWork, IMapper _mapper,
                                 IAttachmentService attachmentService) : IEmployeeService
    {
        public IEnumerable<EmployeeDTO> GetAllEmployees(string? employeeSearchName)
        {
            IEnumerable<Employee> employees;
            if (string.IsNullOrWhiteSpace(employeeSearchName))
            {
                employees = unitOfWork.EmployeeRepository.GetAll();
            }
            else
            {
                employees = unitOfWork.EmployeeRepository.GetAll(E => E.Name.ToLower().Contains(employeeSearchName.ToLower()));
            }

            #region AutoMapper Mapping
            var employeesDto = _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeDTO>>(employees);

            return employeesDto;
            #endregion

            #region Manual Mapping
            //var employeesDto = Employees.Select(emp => new EmployeeDTO()
            //{
            //    Id = emp.Id,
            //    Name = emp.Name,
            //    Age = emp.Age,
            //    Email = emp.Email,
            //    IsActive = emp.IsActive,
            //    Salary = emp.Salary,
            //    EmployeeType = emp.EmployeeType.ToString(),
            //    Gender = emp.Gender.ToString()
            //});
            //return employeesDto;
            #endregion

            // applying encapsulation (database interaction allowed in data
            // access layer only otherwise will implemented in application)
            //var employeeDto = _unitOfWork.EmployeeRepository.GetAll(E => new EmployeeDTO()
            //{
            //    Id = E.Id,
            //    Name = E.Name,
            //    Salary = E.Salary
            //});

            //return employeeDto;
        }

        public EmployeeDetailsDto? GetEmployeeById(int id)
        {
            var employee = unitOfWork.EmployeeRepository.GetById(id);
            return employee is null ? null : _mapper.Map<Employee, EmployeeDetailsDto>(employee);
        }

        public int AddEmployee(CreatedEmployeeDto employeeDto)
        {
            var employee = _mapper.Map<CreatedEmployeeDto, Employee>(employeeDto);
            if(employeeDto.Image is not null)
            {
                employee.ImageName =  attachmentService.Upload(employeeDto.Image,"Images");
            }
            unitOfWork.EmployeeRepository.Add(employee);
            return unitOfWork.SaveChanges();
        }
        public int UpdateEmployee(UpdatedEmployeeDto employeeDto)
        {
            // Step 3: Map the updated data (excluding the image) to the existing employee entity
            _mapper.Map<UpdatedEmployeeDto,Employee>(employeeDto);
            var existingEmployee = unitOfWork.EmployeeRepository.GetById(employeeDto.Id);

            // Step 4: Handle Image Deletion (if applicable)
            if (employeeDto.Image is not null)
            {
                // If an old image exists, delete it
                if (!string.IsNullOrEmpty(existingEmployee.ImageName))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "Files", "Images", existingEmployee.ImageName);

                    if (File.Exists(oldImagePath))
                    {
                        try
                        {
                            bool isDeleted = attachmentService.Delete(oldImagePath); // Delete the old image from the server
                            if (isDeleted)
                            {
                                Console.WriteLine("Old image deleted successfully.");
                            }
                            else
                            {
                                Console.WriteLine("Failed to delete old image.");
                            }
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            Console.WriteLine($"Permission issue while deleting old image: {ex.Message}");
                        }
                        catch (IOException ex)
                        {
                            Console.WriteLine($"Error while deleting old image: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Old image file does not exist.");
                    }

                    // Now, remove the old image record from the database
                    existingEmployee.ImageName = null; // Clear the image reference in the database
                }

                // Step 5: Upload the new image and update the ImageName in the employee entity
                existingEmployee.ImageName = attachmentService.Upload(employeeDto.Image, "Images");
                Console.WriteLine($"New image uploaded: {existingEmployee.ImageName}");
            }

            // Step 6: Save the changes to the database (Update the employee record)
            unitOfWork.EmployeeRepository.Update(existingEmployee);
            return unitOfWork.SaveChanges();
        }




        public bool DeleteEmployee(int id)
        {
            // making soft delete not hard delete (update entity only)
            var employee = unitOfWork.EmployeeRepository.GetById(id);
            if (employee is null) return false;
            else
            {
                employee.IsDeleted = true;
                unitOfWork.EmployeeRepository.Update(employee);
            }
            return unitOfWork.SaveChanges() > 0 ? true : false;
        }
    }
}
