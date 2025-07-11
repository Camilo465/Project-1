using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Services.Auth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuth _auth) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "SuperAdminOnly")]
    [Route("CreateRole")]
    public async Task<ActionResult<BaseApiResponse<string, string>>> CreateRole(string roleName)
    {
        try
        {
            var result = await _auth.CreateRole(roleName);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }
    [HttpPost]
    [Authorize(Policy = "SuperAdminOnly")]
    [Route("AddUserToRole")]
    public async Task<ActionResult<BaseApiResponse<string, string>>> AddUserToRole(string email, string roleName)
    {
        try
        {
            var result = await _auth.AddUserToRole(email, roleName);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<BaseApiResponse<OutputLoginModel, string>>> Login([FromBody] LoginModel model)
    {
        try
        {
            var result = await _auth.Login(model);

            return result.Result == null ? Ok(result) : Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<BaseApiResponse<string, string>>> Register([FromBody] RegisterModel model)
    {
        try
        {
            var result = await _auth.Register(model);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }
    [HttpPost]
    [Route("refresh-token")]
    public async Task<ActionResult<BaseApiResponse<OutputTokenModel, string>>> RefreshToken(InputTokenModel tokenModel)
    {
        try
        {
            var result = await _auth.RefreshToken(tokenModel);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }
    [Authorize(Policy = "ExclusiveOnly")]
    [HttpPost]
    [Route("revoke/{username}")]
    public async Task<ActionResult<BaseApiResponse<string, string>>> Revoke(string username)
    {
        try
        {
            var result = await _auth.Revoke(username);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}