using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Services.Auth.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace APICatalogo.Services.Auth;

public class Auth(ITokenService _tokenService, UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager, IConfiguration _configuration, ILogger _logger) : IAuth
{
    public async Task<BaseApiResponse<string, string>> CreateRole(string roleName)
    {
        var roleExist = await _roleManager.RoleExistsAsync(roleName);
        if (roleExist)
        {
            _logger.LogInformation(2, "Error");
            return new BaseApiResponse<string, string> { Error = $"Role:{roleName} já existente" };
        }

        _logger.LogInformation(1, "Roles Added");
        var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

        return new BaseApiResponse<string, string> { Result = $"Role: {roleName} adicionada." };
    }
    public async Task<BaseApiResponse<string, string>> AddUserToRole(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            _logger.LogInformation(1, $"Erro ao adicionar email:{user!.Email} na role {roleName} role");
            return new BaseApiResponse<string, string> { Result = "Dados nulos ou inválidos." };
        }
        var result = await _userManager.AddToRoleAsync(user, roleName);
        _logger.LogInformation(1, $"User {user.Email} adicionado na {roleName} role");

        return new BaseApiResponse<string, string> { Result = "Email adicionado na role." };
    }

    public async Task<BaseApiResponse<OutputLoginModel, string>> Login(LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName!);

        if (user is null && await _userManager.CheckPasswordAsync(user, model.Password!))
            return new BaseApiResponse<OutputLoginModel, string> { Error = "Senha inválida ou dados inváilidos do usuário."};

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("id",user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = _tokenService.GenerateAccessToken(authClaims, _configuration);

            var refreshToken = _tokenService.GenerateRefreshToken();

            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"], out int refreshTokenValidityInMinutes);

            user.RefreshTokenExpireTime = DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);

            user.RefreshToken = refreshToken;

            await _userManager.UpdateAsync(user);

            return new BaseApiResponse<OutputLoginModel, string> { Result = new OutputLoginModel { Token = new JwtSecurityTokenHandler().WriteToken(token), RefreshToken = refreshToken, Expiration = token.ValidTo } };
    }

    public async Task<BaseApiResponse<string, string>> Register(RegisterModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username!);

        if (userExists != null)
        {
            return new BaseApiResponse<string, string> { Error = "Usuário já existente." };
        }
        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };
        var result = await _userManager.CreateAsync(user, model.Password!);

        if (!result.Succeeded)
        {
            return new BaseApiResponse<string, string> { Error = "Criação do usuário falhou." };
        }

        return new BaseApiResponse<string, string> { Result = "Usuário criado com sucesso." };
    }

    public async Task<BaseApiResponse<OutputTokenModel, string>> RefreshToken(InputTokenModel tokenModel)
    {
        if (tokenModel is null)
            return new BaseApiResponse<OutputTokenModel, string> { Error = "Modelo de token inválido" };

        string? accessToken = tokenModel.AccessToken ?? throw new ArgumentNullException(nameof(tokenModel));

        string? refreshToken = tokenModel.RefreshToken ?? throw new ArgumentException(nameof(tokenModel));

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!, _configuration);

        if (principal == null)
            return new BaseApiResponse<OutputTokenModel, string> { Error = "Token de acesso inválido" };

        string username = principal.Identity!.Name!;

        var user = await _userManager.FindByNameAsync(username!);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpireTime <= DateTime.Now)
            return new BaseApiResponse<OutputTokenModel, string> { Error = "Token de acesso inválido" };

        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList(), _configuration);

        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;

        await _userManager.UpdateAsync(user);

        return new BaseApiResponse<OutputTokenModel, string> { Result = new OutputTokenModel { AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken), RefreshToken = newRefreshToken } };
    }

    public async Task<BaseApiResponse<string, string>> Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null) return new BaseApiResponse<string, string> { Error = "Username inválido." };

        user.RefreshToken = null;

        await _userManager.UpdateAsync(user);
        return new BaseApiResponse<string, string> { Result = "Revogado." };
    }
}
