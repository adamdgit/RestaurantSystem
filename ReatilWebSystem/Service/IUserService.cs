using BitByByte.Models;
using Microsoft.AspNetCore.Identity;

namespace BitByByte.Service
{
    public interface IUserService
    {
        Task<(string Message, bool Success)> RegisterAsync(Register model);
        Task<(string Message, bool Success)> LoginAsync(Login model);
        Task<(string Message, bool Success)> LogoutAsync();
        Task<(string Message, bool Success)> ChangePasswordAsync(ChangePassword model, string username);
        Task<(string Message, bool Success)> EditUserDetails(EditUser userDetails);
    }
}
