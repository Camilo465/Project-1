using APICatalogo.DTOs;
using APICatalogo.Models;

namespace APICatalogo.Services.Auth.Interfaces;

public interface IAuth
{
    Task<BaseApiResponse<string, string>> CreateRole(string roleName);
    Task<BaseApiResponse<string, string>> AddUserToRole(string email, string roleName);
    Task<BaseApiResponse<OutputLoginModel, string>> Login(LoginModel model);
    Task<BaseApiResponse<string, string>> Register(RegisterModel model);
    Task<BaseApiResponse<OutputTokenModel, string>> RefreshToken(InputTokenModel model);
    Task<BaseApiResponse<string, string>> Revoke(string username);
}
