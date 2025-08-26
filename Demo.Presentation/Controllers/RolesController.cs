using Demo.DataAccess.Models.IdentityModel;
using Demo.DataAccess.Models.Shared.Enums;
using Demo.Presentation.ViewModels.ManagerViewModel.RoleManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Presentation.Controllers
{
    [Authorize(Roles ="Admin")]
    public class RolesController(RoleManager<ApplicationRole> roleManager,
                                 IWebHostEnvironment environment,
                                 ILogger<RolesController> logger) : Controller
    {
        public IActionResult Index(string RoleSearchName)
        {
            var roles = string.IsNullOrEmpty(RoleSearchName)
                            ? roleManager.Roles
                            : roleManager.Roles.Where(R => R.Name.Contains(RoleSearchName));

            var rolesViewModel = roles.Select(role => new RoleViewModel()
            {
                Id = role.Id,
                RoleName = role.Name ?? string.Empty,
            }).ToList();
            return View(rolesViewModel);
        }

        #region Create Role
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(CreateEditRoleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var identityRole = new ApplicationRole()
                    {
                        Name = viewModel.RoleName.ToString()
                    };

                    var result = roleManager.CreateAsync(identityRole).Result;

                    if (result.Succeeded)
                    {
                        TempData["Message"] = $"{identityRole.Name} Role Is Created Successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (environment.IsDevelopment())
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                    else
                    {
                        logger.LogError(ex.Message);
                    }
                }
            }

            return View(viewModel);
        }
        #endregion

        #region Edit Role
        [HttpGet]
        public IActionResult Edit([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var role = roleManager.FindByIdAsync(id).Result;

            if (role is null) return NotFound();

            if (!Enum.TryParse<Role>(role.Name, out var parsedRole))
            {
                return BadRequest("Invalid role name in database.");
            }

            var roleViewModel = new CreateEditRoleViewModel()
            {
                RoleName = parsedRole
            };

            return View(roleViewModel);
        }

        [HttpPost]
        public IActionResult Edit([FromRoute] string id, CreateEditRoleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var role = roleManager.FindByIdAsync(id).Result;
                    if (role is null) return NotFound();

                    role.Name = viewModel.RoleName.ToString();

                    var result = roleManager.UpdateAsync(role).Result;

                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (environment.IsDevelopment())
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                    else
                    {
                        logger.LogError(ex.Message);
                    }
                }
            }

            return View(viewModel);
        }
        #endregion

        #region Details Of Role
        [HttpGet]
        public IActionResult Details(string id)
        {
            if (id is null) return BadRequest();

            var role = roleManager.FindByIdAsync(id).Result;
            if (role is null) return NotFound();

            var roleViewModel = new RoleViewModel()
            {
                Id = role.Id,
                RoleName = role.Name ?? string.Empty
            };
            return View(roleViewModel);
        }
        #endregion

        #region Delete Role
        [HttpPost]
        public IActionResult Delete([FromRoute] string id)
        {
            if (id is not null)
            {
                try
                {
                    var role = roleManager.FindByIdAsync(id).Result;

                    if (role is null) return NotFound();

                    var result = roleManager.DeleteAsync(role).Result;
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Role Can not be Deleted");
                        return RedirectToActionPermanent(nameof(Index));
                    }

                }
                catch (Exception ex)
                {
                    if (environment.IsDevelopment())
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                    else
                    {
                        logger.LogError(ex.Message);
                        return View("ErrorView", ex);
                    }
                }
            }

            return BadRequest();
        }
        #endregion
    }
}
