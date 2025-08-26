using Demo.BusinessLogic.DTOs.DepartmentDtos;
using Demo.BusinessLogic.Services.DepartmentsService;
using Demo.Presentation.ViewModels.DepartmentsViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Presentation.Controllers
{
    [Authorize(Roles ="Admin")]
    public class DepartmentsController(IDepartmentService _departmentService,
        ILogger<DepartmentsController> _logger,
        IWebHostEnvironment _environment) : Controller
    {
        // BaseURL/Departments/Index
        public IActionResult Index(string? departmentSearchName)
        {
            var departments = _departmentService.GetAllDepartments(departmentSearchName);
            // contains 4 overloads
            return View(departments);
            //return View(view_name);
            //return View(view_name, view_model)
            //return View(view_model)

        }

        #region Create Department

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DepartmentViewModel departmentViewModel)
        {
            if (ModelState.IsValid) // server side validation checker
            {
                try
                {
                    var departmentDto = new CreatedDepartmentDto()
                    {
                        Code = departmentViewModel.Code,
                        Name = departmentViewModel.Name,
                        Description = departmentViewModel.Description,
                        DateOfCreation = departmentViewModel.DateOfCreation
                    };
                    int result = _departmentService.AddDepartment(departmentDto);
                    String Message;
                    if (result > 0)
                        Message = $"Department With Name ({departmentDto.Name}) Is Created Successfully";
                    else
                        Message = $"Department With Name ({departmentDto.Name}) Can't Be Created";

                    TempData["Message"] = Message;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log Exception

                    // 1. Development=> log error in console and return same view with error message
                    if (_environment.IsDevelopment())
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }

                    // 2. Deployment=> log error in file | table in database and return error view
                    else // logging base-on OS [windows: event viewer]
                    {
                        _logger.LogError(ex.Message);
                    }
                }
            }

            return View(departmentViewModel);
        }

        #endregion

        #region Details Of Department

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var department = _departmentService.GetDepartmentById(id.Value);
            if (department is null) return NotFound();

            return View(department);
        }

        #endregion

        #region Edit Department

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var department = _departmentService.GetDepartmentById(id.Value);
            if (department is null) return NotFound();

            var departmentViewModel = new DepartmentViewModel()
            {
                Code = department.Code,
                Name = department.Name,
                Description = department.Description,
                DateOfCreation = department.CreatedOn
            };
            return View(departmentViewModel);
        }

        [HttpPost]
        public IActionResult Edit([FromRoute] int id, DepartmentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var UpdatedDepartment = new UpdatedDepartmentDto()
                    {
                        Id = id,
                        Code = viewModel.Code,
                        Name = viewModel.Name,
                        Description = viewModel.Description,
                        DateOfCreation = viewModel.DateOfCreation
                    };
                    int result = _departmentService.UpdateDepartment(UpdatedDepartment);
                    if (result > 0)
                        RedirectToAction(nameof(Index));
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Department is not Updated");
                    }
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
                        return View("ErrorView", ex);
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Delete Department

        //[HttpGet]
        //public IActionResult Delete(int? id)
        //{
        //    if (!id.HasValue) return BadRequest();

        //    var department = _departmentService.GetDepartmentById(id.Value);
        //    if (department is null) return NotFound();
        //    return View(department);
        //}

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (id == 0) return BadRequest();
            try
            {
                bool deleted = _departmentService.DeleteDepartment(id);
                if (deleted)
                    return RedirectToAction(nameof(Index));
                else
                {
                    ModelState.AddModelError(string.Empty, "Department is not Deleted");
                    return RedirectToAction(nameof(Index)); // redirect to http get only
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
