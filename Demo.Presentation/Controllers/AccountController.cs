using Demo.DataAccess.Models.IdentityModel;
using Demo.DataAccess.Models.Shared.Enums;
using Demo.Presentation.Helpers;
using Demo.Presentation.Models;
using Demo.Presentation.Utitlities;
using Demo.Presentation.ViewModels.AuthViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Demo.Presentation.Controllers
{
    public class AccountController(UserManager<ApplicationUser> userManager,
                                   SignInManager<ApplicationUser> signInManager,
                                   RoleManager<ApplicationRole> roleManager,
                                   IMailService mailService,
                                   ISmsService smsService) : Controller
    {
        #region Register
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var user = new ApplicationUser
            {
                UserName = viewModel.UserName,
                Email = viewModel.Email,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                RegisteredAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, viewModel.Password);
            if (result.Succeeded)
            {
                foreach (var role in Enum.GetValues(typeof(Role)).Cast<Role>())
                {
                    if (!await roleManager.RoleExistsAsync(role.ToString()))
                    {
                        await roleManager.CreateAsync(new ApplicationRole { Name = role.ToString() });
                    }
                }

                await userManager.AddToRoleAsync(user, "Pending");

                TempData["Message"] = "Registration successful. Please wait for admin approval.";
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(viewModel);
            }
        }
        #endregion

        #region Login
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var user = userManager.FindByEmailAsync(viewModel.Email).Result;

            if (user is not null)
            {
                var isPending = userManager.IsInRoleAsync(user, "Pending").Result;
                if (isPending)
                {
                    ModelState.AddModelError(string.Empty, "Your account is pending approval. Please wait for admin confirmation.");
                    return View(viewModel);
                }

                var Result = signInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RememberMe, false).Result;

                if (Result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid Login");
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            var claims = result.Principal?.Identities?.FirstOrDefault()?.Claims;
            if (claims != null)
            {
                var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (emailClaim != null)
                {
                    var user = await userManager.FindByEmailAsync(emailClaim.Value);

                    if (user == null)
                    {
                        user = new ApplicationUser { UserName = emailClaim.Value, Email = emailClaim.Value };
                        var createResult = await userManager.CreateAsync(user);

                        if (!createResult.Succeeded)
                        {
                            TempData["Message"] = "An error occurred while creating your account.";
                            return RedirectToAction(nameof(Login));
                        }

                        await userManager.AddToRoleAsync(user, "Pending");
                    }

                    var isPending = await userManager.IsInRoleAsync(user, "Pending");
                    if (isPending)
                    {
                        TempData["Message"] = "Your account is pending approval. Please wait for admin confirmation.";
                        return RedirectToAction(nameof(Login));
                    }

                    await signInManager.SignOutAsync();
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["Message"] = "Google authentication failed.";
            return RedirectToAction(nameof(Login));
        }
        #endregion

        #region SignOut
        [HttpPost]
        public IActionResult Logout()
        {
            signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Forget Password
        [HttpGet]
        public IActionResult ForgetPassword() => View();

        [HttpPost]
        public IActionResult SendResetPasswordLink(ForgetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var User = userManager.FindByEmailAsync(viewModel.Email).Result;
                if (User is not null)
                {
                    var token = userManager.GeneratePasswordResetTokenAsync(User).Result;
                    var resetPasswordLink = Url.Action("ResetPassword", "Account", new { email = viewModel.Email, token = token }, Request.Scheme);
                    var email = new Email()
                    {
                        To = viewModel.Email,
                        Subject = "Reset Password",
                        Body = resetPasswordLink // TODO
                    };
                    //EmailSettings.SendEmail(email);
                    mailService.Send(email);
                    return RedirectToAction("CheckYourInbox", "Account");
                }
            }
            ModelState.AddModelError(string.Empty, "Invalid Operation");
            return View(nameof(ForgetPassword), viewModel);
        }

        // for SMS Verification
        //[HttpPost]
        //public IActionResult SendResetPasswordLinkSms(ForgetPasswordViewModel viewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var User = userManager.FindByEmailAsync(viewModel.Email).Result;
        //        if (User is not null)
        //        {
        //            var token = userManager.GeneratePasswordResetTokenAsync(User).Result;
        //            var resetPasswordLink = Url.Action("ResetPassword", "Account", new { email = viewModel.Email, token = token }, Request.Scheme);
        //            //var email = new Email()
        //            //{
        //            //    To = viewModel.Email,
        //            //    Subject = "Reset Password",
        //            //    Body = resetPasswordLink // TODO
        //            //};
        //            //EmailSettings.SendEmail(email);

        //            var sms = new SmsMessage()
        //            {
        //                Body = resetPasswordLink,
        //                PhoneNumber = User.PhoneNumber
        //            };
                    
        //            smsService.SendSms(sms);
        //            return Ok("Check Your SMS");
        //        }
        //    }
        //    ModelState.AddModelError(string.Empty, "Invalid Operation");
        //    return View(nameof(ForgetPassword), viewModel);
        //}

        [HttpGet]
        public IActionResult CheckYourInbox() => View();

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            TempData["email"] = email;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            string email = TempData["email"] as string ?? string.Empty;
            string token = TempData["token"] as string ?? string.Empty;

            var user = userManager.FindByEmailAsync(email).Result;
            if (user is not null)
            {
                var Result = userManager.ResetPasswordAsync(user, token, viewModel.Password).Result;
                if (Result.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach (var error in Result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(nameof(ResetPassword), viewModel);
        }

        #endregion
    }
}
