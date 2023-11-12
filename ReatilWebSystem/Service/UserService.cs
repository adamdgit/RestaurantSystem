using Microsoft.AspNetCore.Identity;
using BitByByte.Models;
using System.Security.Claims;
using BitByByte.Data;

namespace BitByByte.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public UserService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        public async Task<(string Message, bool Success)> EditUserDetails(EditUser userDetails)
        {

            var user = await userManager.FindByIdAsync(userDetails.Id);
            if (user == null)
            {
                return ("No user found", false);
            }

            user.Email = userDetails.Email;
            user.FirstName = userDetails.FirstName;
            user.LastName = userDetails.LastName;
            user.PhoneNumber = userDetails.PhoneNumber;

            // validate profile url is being updated
            if (userDetails.ProfileUrl != null)
            {
                user.ProfileUrl = userDetails.ProfileUrl;
            }

            // if an admin edits the user, role will be updated
            if (userDetails.Role != null)
            {
                user.Role = userDetails.Role;
            }

            var update = await userManager.UpdateAsync(user);

            if (update.Succeeded)
            {
                return ("User details updated successfully", true);
            }

            return ("Updating details failed.", false);
        } 

        public async Task<(string Message, bool Success)> LoginAsync(Login model)
        {
            try
            {
                var user = await userManager.FindByNameAsync(model.UserName);

                // check user exists
                if (user == null)
                {
                    return ("No user found", false);
                }

                // check password matches
                if (!await userManager.CheckPasswordAsync(user, model.Password))
                {
                    return ("Invalid Password", false);
                }

                // check email has been verified
                var checkVerifiedEmail = await userManager.IsEmailConfirmedAsync(user);

                if (checkVerifiedEmail == false)
                {
                    return ("Email has not been verified, please check your emails, and click the verification link.", false);
                }

                var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, true);
                
                if (signInResult.Succeeded)
                {
                    var userRoles = await userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim>
                    { new Claim(ClaimTypes.Name, user.UserName), };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }
                    return ("Logged in successfully", true);
                }
                else if (signInResult.IsLockedOut)
                {
                    return ("User is locked out", false);
                }
                else
                {
                    return ("Error on logging in", false);
                }

            } 
            catch (Exception e){
                return (e.Message, false);
            }

        }

        public async Task<(string Message, bool Success)> RegisterAsync(Register model)
        {
            var userExists = await userManager.FindByEmailAsync(model.Email);

            if (userExists != null)
            {
                return ("User already exist", false);
            }

            // new users are alwways users, can be modified by admins
            model.Role = "user";

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                //EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                ProfileUrl = "none",
                Role = model.Role
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return ("User creation failed", false);
            }

            if (!await roleManager.RoleExistsAsync(model.Role))
            {
                await roleManager.CreateAsync(new IdentityRole(model.Role));
            }

            if (await roleManager.RoleExistsAsync(model.Role))
            {
                await userManager.AddToRoleAsync(user, model.Role);
            }

            return ("You have registered successfully", true);
        }

        public async Task<(string Message, bool Success)> LogoutAsync()
        {
            await signInManager.SignOutAsync();
            return ("You have Logged out successfully", true);
        }

        public async Task<(string Message, bool Success)> ChangePasswordAsync(ChangePassword model, string username)
        {
            try
            {
                var user = await userManager.FindByNameAsync(username);

                if (user == null)
                {
                    return ("No user found", false);
                }

                var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    return ("Password updated", true);
                }
                else
                {
                    return ("An error occurred", false);
                }
            } 
            catch (Exception e)
            {
                return (e.Message, false);
            }


        }
    }
}
