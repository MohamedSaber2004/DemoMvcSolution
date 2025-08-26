using Demo.DataAccess.Models.IdentityModel;
using Demo.Presentation.ViewModels.ManagerViewModel.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.Presentation.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController(UserManager<ApplicationUser> userManager,
                                 RoleManager<ApplicationRole> roleManager,
                                 IWebHostEnvironment environment,
                                 ILogger<UsersController> logger) : Controller
    {
        public async Task<IActionResult> Index(string SearchUserName)
        {
            IQueryable<ApplicationUser> QuerableUsers;

            if (string.IsNullOrWhiteSpace(SearchUserName))
            {
                QuerableUsers = userManager.Users;
            }
            else
            {
                QuerableUsers = userManager.Users.Where(u =>
                    u.FirstName.ToLower().Contains(SearchUserName.ToLower()) ||
                    u.LastName.ToLower().Contains(SearchUserName.ToLower())
                );
            }

            var users = await QuerableUsers.ToListAsync();

            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);

                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Fname = user.FirstName,
                    Lname = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles.ToList()
                });
            }

            return View(userViewModels);
        }

        #region Pending Users
        [HttpGet]
        public async Task<IActionResult> PendingUsers()
        {
            var pendingUsers = await userManager.GetUsersInRoleAsync("Pending");
            var model = pendingUsers.Select(u => new UserManagerViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FullName = $"{u.FirstName} {u.LastName}",
                RegisteredAt = u.RegisteredAt
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            await userManager.RemoveFromRoleAsync(user, "Pending");
            await userManager.AddToRoleAsync(user, "User");

            TempData["Message"] = "User approved successfully.";
            return RedirectToAction("PendingUsers");
        }

        [HttpPost]
        public async Task<IActionResult> RejectUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            await userManager.DeleteAsync(user);

            TempData["Message"] = "User rejected and removed.";
            return RedirectToAction("PendingUsers");
        }
        #endregion

        #region Details Of User
        [HttpGet]
        public async Task<IActionResult> Details([FromRoute]string id)
        {
            if (id is null) 
                return BadRequest();

            var user = await userManager.FindByIdAsync(id);
            if (user is null) 
                return NotFound();

            var roles = await userManager.GetRolesAsync(user);

            var userViewModel = new UserViewModel()
            {
                Id = user.Id,
                Fname = user.FirstName,
                Lname = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToList()
            };

            return View(userViewModel);
        }
        #endregion

        #region Edit User
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var user = await userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            var userRoles = await userManager.GetRolesAsync(user);
            var allRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();

            var viewModel = new UserEditViewModel
            {
                Fname = user.FirstName,
                Lname = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Roles = allRoles.Select(role => new RoleSelectionViewModel
                {
                    RoleName = role,
                    IsSelected = userRoles.Contains(role)
                }).ToList()
            };

            return View("Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id, UserEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var user = await userManager.FindByIdAsync(id);
                if (user is null) return NotFound();

                user.FirstName = viewModel.Fname;
                user.LastName = viewModel.Lname;
                user.PhoneNumber = viewModel.PhoneNumber;

                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(viewModel);
                }

                var currentRoles = await userManager.GetRolesAsync(user);
                var selectedRoles = viewModel.Roles.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();

                var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to remove old roles.");
                    return View(viewModel);
                }

                var addResult = await userManager.AddToRolesAsync(user, selectedRoles);
                if (!addResult.Succeeded)
                {
                    foreach (var error in addResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(viewModel);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (environment.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                else
                {
                    logger.LogError(ex, "Error while updating user.");
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the user.");
                }

                return View(viewModel);
            }
        }
        #endregion

        #region Delete User
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if(id is null) return BadRequest();

            var user = await userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            var userViewModel = new UserViewModel()
            {
                Id= user.Id,
                Fname = user.FirstName,
                Lname = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            try
            {
                var user = await userManager.FindByIdAsync(id);
                if (user is null)
                    return NotFound();

                var result = await userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    TempData["Error"] = "Something went wrong while deleting the user.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                TempData["Message"] = "User deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (environment.IsDevelopment())
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                else
                {
                    logger.LogError(ex, $"Error occurred while deleting user with ID: {id}");
                    TempData["Error"] = "An unexpected error occurred while deleting the user.";
                }

                return RedirectToAction(nameof(Delete), new { id });
            }
        }
        #endregion
    }
}
