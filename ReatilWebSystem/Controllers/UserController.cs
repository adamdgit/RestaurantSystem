using System.Data;
using BitByByte.Data;
using BitByByte.Models;
using BitByByte.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace BitByByte.Controllers
{

    public class UserController : Controller
    {
        private readonly IUserService _authService;
        private readonly UserManager<ApplicationUser> _user;
        private IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IUserService authService, UserManager<ApplicationUser> user, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _user = user;
            _env = env;
            _userManager = userManager;
        }

        // verify email from verification link
        public async Task<IActionResult> Verify()
        {
            var query = HttpContext.Request.Query;
            string code = query["code"];
            string userID = query["id"];

            // no code or user id found
            if (userID == null || code == null)
            {
                TempData["msg"] = "Something went wrong.";
                return View();
            }

            var user = await _user.FindByIdAsync(userID);

            if (user != null)
            {
                var codeIsValid = await _user.ConfirmEmailAsync(user, code);

                if (codeIsValid != null)
                {
                    // update email confirmed if code is validated
                    user.EmailConfirmed = true;
                    await _user.UpdateAsync(user);

                    TempData["msg"] = "Your account has been verifed.";
                    return View();
                }
                else
                {
                    TempData["msg"] = "Account couldn't be verified";
                    return View();
                }

            }

            return View();
        }

        // --- REGISTER ----
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid) {

                var result = await _authService.RegisterAsync(model);

                if (result.Success)
                {
                    var user = await _user.FindByEmailAsync(model.Email);
                    var code = await _user.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    string callbackURL = $"User/Verify/?code={code}&id={user.Id}";

                    string url = "Please click the link to confirm your email: \n" + $"<a href='https://localhost:7149/{HtmlEncoder.Default.Encode(callbackURL)}'>Confirm Email</a>";

                    EmailOuterService Outer = new EmailOuterService();
                    var confirmation = Outer.Inner.SendEmailConfirmation(model.Email, model.FirstName, model.LastName, url);

                    if (!confirmation)
                    {
                        TempData["msg"] = result.Message + " - " + "Confirmation email failed to send.";
                    }
                    else
                    {
                        TempData["msg"] = result.Message + " - " + "You have been sent a confirmation email.";
                    }

                    return RedirectToAction(nameof(Login));
                } 
                else
                {
                    ModelState.AddModelError("Email", "Email already in use.");
                    return View(model);
                }
            } 

            return View(model);

        }

        // --- LOGIN ----
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.LoginAsync(model);

                if (result.Success)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["msg"] = result.Message;
                    return RedirectToAction(nameof(Login));
                }
            }

            return View(model);
        }

        // --- Logout ----
        public async Task<IActionResult> Logout()
        {
            var result = await this._authService.LogoutAsync();

            TempData["msg"] = result.Message;

            return RedirectToAction(nameof(Login));
        }

        // Show list of users, with option to edit
        [Authorize(Roles = "admin")]
        public IActionResult UserList()
        {
            var users = _user.Users
                .Select(a => new EditUser { Id = a.Id, FirstName = a.FirstName, LastName = a.LastName, Email = a.Email, PhoneNumber = a.PhoneNumber, Role = a.Role })
                .ToList();

            return View(users);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditUser(string id)
        {
            // get selected user by id param
            var user = await _user.FindByIdAsync(id);

            if (user == null)
            {
                return View();
            } 
            else
            {
                // get users roles
                var userRoles = await _userManager.GetRolesAsync(user);

                // send to the view edituser type
                var editUser = new EditUser
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = userRoles[0]
                };
                return View(editUser);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUser userDetails)
        {

            if (userDetails != null)
            {
                var result = await _authService.EditUserDetails(userDetails);

                if (result.Success)
                {
                    TempData["msg"] = result.Message;
                    return View(userDetails);
                }
            }

            return View(userDetails);
        }

        // Delete a user by ID
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var user = await _user.FindByIdAsync(Id);
            if (user != null)
            {
                var result = await _user.DeleteAsync(user);
                if (result != null)
                {
                    var users = _user.Users
                        .Select(a => new EditUser { Id = a.Id, FirstName = a.FirstName, LastName = a.LastName, Email = a.Email, PhoneNumber = a.PhoneNumber, Role = a.Role })
                        .ToList();

                    return View(users);
                }
            }

            return NotFound("User not found");
        }

        public async Task<IActionResult> Edit()
        {
            var user = await _user.GetUserAsync(User);
            if (user != null)
            {
                ViewData["UserDetails"] = user;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUser userDetails, IFormFile file)
        {
            // get user ID
            var user = await _user.GetUserAsync(User);
            if (user != null)
            {
                ViewData["UserDetails"] = user;

                // if File is uploaded, save to uploads folder on server
                if (file != null)
                {
                    // only allow specific file types
                    var ext = Path.GetExtension(file.FileName);
                    var allowedExtensions = new string[] { ".jpg", ".png", ".webp", ".jpeg" };

                    // if file type is not supported, return model error
                    if (!allowedExtensions.Contains(ext))
                    {
                        ModelState.AddModelError("ProfileUrl", "File must be of type: jpeg, jpg, png or webp");
                        return View(userDetails);
                    }

                    // image is greater than 1MB, return error
                    if (file.Length > 1048576)
                    {
                        ModelState.AddModelError("ProfileUrl", "File size is too large, images should be less than 1MB");
                        return View(userDetails);
                    }

                    // generate a unique filename, to prevent duplicates
                    string uniqueFileName = Guid.NewGuid() + "_" + file.FileName;
                    string filePath = Path.Combine(_env.WebRootPath, "uploads", uniqueFileName);

                    // copy data steam to uploads folder
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // set profile url if a picture has been uploaded
                    userDetails.ProfileUrl = $"/uploads/{uniqueFileName}";

                    // default profileurl for new accounts is "none" so we don't have an old image to delete for new accounts
                    if (user.ProfileUrl != "none")
                    {
                        // Check if old profile picture exists, and delete it
                        string oldProfilePath = Path.Combine(_env.WebRootPath, "uploads", userDetails.ProfileUrl);
                        if (System.IO.File.Exists(oldProfilePath))
                        {
                            System.IO.File.Delete(oldProfilePath);
                        }
                        else
                        {
                            Console.WriteLine("File does not exist.");
                        }
                    }
                }
                // update user details
                var result = await _authService.EditUserDetails(userDetails);

                TempData["msg"] = result.Message;
                return View(userDetails);
            }

            return View(userDetails);
        }

        //Change Password
        [HttpGet, ActionName("Update")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // [Authorize]
        [HttpPost, ActionName("Update")]
        public async Task<IActionResult> ChangePassword(ChangePassword model)
        {
            if (ModelState.IsValid && User != null)
            {
                var result = await _authService.ChangePasswordAsync(model, User.Identity.Name);

                TempData["msg"] = result.Message;

                if (result.Success)
                {
                    return RedirectToAction(nameof(Login));
                }

                return View(model);
            }

            return View(model);

        }
    }
}
