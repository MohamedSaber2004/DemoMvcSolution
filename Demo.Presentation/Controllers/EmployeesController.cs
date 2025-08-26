using Demo.BusinessLogic.DTOs.EmployeeDtos;
using Demo.BusinessLogic.Services.EmployeesService;
using Demo.DataAccess.Models.Employee_Model;
using Demo.DataAccess.Models.Shared.Enums;
using Demo.Presentation.ViewModels.EmployeesViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Drawing;

namespace Demo.Presentation.Controllers
{
    [Authorize(Roles ="Admin")]
    public class EmployeesController(IEmployeeService _employeeService,
        IWebHostEnvironment _environment,
        ILogger<EmployeesController> _logger) : Controller
    {
        // BaseURL/Employees/Index
        public IActionResult Index(string? employeeSearchName)
        {
            var employees = _employeeService.GetAllEmployees(employeeSearchName);
            return View(employees);
        }

        #region Create Employee
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(EmployeeViewModel employeeViewModel)
        {
            if (ModelState.IsValid) // server side validation checker
            {
                try
                {
                    var employeeDto = new CreatedEmployeeDto()
                    {
                        Name = employeeViewModel.Name,
                        Address = employeeViewModel.Address,
                        Age = employeeViewModel.Age,
                        Email = employeeViewModel.Email,
                        IsActive = employeeViewModel.IsActive,
                        EmployeeType = employeeViewModel.EmployeeType,
                        Gender = employeeViewModel.Gender,
                        HiringDate = employeeViewModel.HiringDate,
                        PhoneNumber = employeeViewModel.PhoneNumber,
                        Salary = employeeViewModel.Salary,
                        DepartmentId = employeeViewModel.DepartmentId,
                        Image = employeeViewModel.Image
                    };
                    int result = _employeeService.AddEmployee(employeeDto);
                    string Message;
                    if (result > 0)
                        Message = $"Employee With Name ({employeeDto.Name}) Is Created Successfully";
                    else
                        Message = $"Employee With Name ({employeeDto.Name}) Can't Be Created";

                    TempData["Message"] = Message;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (_environment.IsDevelopment())
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                    else
                    {
                        _logger.LogError(ex.Message);
                    }
                }
            }

            return View(employeeViewModel);
        }
        #endregion

        #region Details Of Employee
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var employee = _employeeService.GetEmployeeById(id.Value);
            return (employee is null) ? NotFound() : View(employee);
        }
        #endregion

        #region Edit Employee
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var employee = _employeeService.GetEmployeeById(id.Value);
            if (employee is null) return NotFound();

            var employeeViewModel = new EmployeeViewModel()
            {
                Name = employee.Name,
                Address = employee.Address,
                Salary = employee.Salary,
                Age = employee.Age,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                IsActive = employee.IsActive,
                HiringDate = employee.HiringDate,
                EmployeeType = Enum.Parse<EmployeeType>(employee.EmployeeType),
                Gender = Enum.Parse<Gender>(employee.Gender),
                DepartmentId = employee.DepartmentId,
                ImageName = employee.ImageName
            };
            return View(employeeViewModel);
        }

        [HttpPost]
        public IActionResult Edit([FromRoute] int? id, EmployeeViewModel employeeViewModel)
        {
            if (!id.HasValue) return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    var employeeDto = new UpdatedEmployeeDto()
                    {
                        Id = id.Value,
                        Name = employeeViewModel.Name,
                        Address = employeeViewModel.Address,
                        Salary = employeeViewModel.Salary,
                        Age = employeeViewModel.Age,
                        Email = employeeViewModel.Email,
                        PhoneNumber = employeeViewModel.PhoneNumber,
                        IsActive = employeeViewModel.IsActive,
                        HiringDate = employeeViewModel.HiringDate,
                        EmployeeType = employeeViewModel.EmployeeType,
                        Gender = employeeViewModel.Gender,
                        DepartmentId = employeeViewModel.DepartmentId,
                        Image = employeeViewModel.Image,
                        ImageName= employeeViewModel.ImageName
                    };
                    var result = _employeeService.UpdateEmployee(employeeDto);
                    if (result > 0) return RedirectToAction(nameof(Index));
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Employee is not Updated");
                    }
                }
                catch (Exception ex)
                {
                    if (_environment.IsDevelopment())
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                        return View(employeeViewModel);
                    }
                    else
                    {
                        _logger.LogError(ex.Message);
                        return View("ErrorView", ex);
                    }
                }
            }

            return View(employeeViewModel);
        }
        #endregion

        #region Delete Employee
        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (id == 0) return BadRequest();

            try
            {
                var Deleted = _employeeService.DeleteEmployee(id);
                if (Deleted) return RedirectToAction(nameof(Index));
                else
                {
                    ModelState.AddModelError(string.Empty, "Employee is not Deleted");
                    return RedirectToActionPermanent(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogError(ex.Message);
                    return View("ErrorView", ex);
                }
            }

        }
        #endregion
    }
}
