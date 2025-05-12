using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Octavus.App.Api.Authentication.Request;
using Octavus.Authentication.Request;
using Octavus.Infra.Core.Services;
using System.Collections.Generic;
using Octavus.Core.Application.DTO;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly KeycloakService _keycloakService;

    public AuthenticationController(KeycloakService keycloakService)
    {
        _keycloakService = keycloakService;
    }

    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser([FromBody] KeycloakUser request)
    {
        var result = await _keycloakService.CreateUserAndAssignRolesAsync(request);
        if (result)
            return Ok("Usuário criado com sucesso.");
        return BadRequest("Erro ao criar o usuário.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _keycloakService.AuthenticateAsync(request.Username, request.Password);
        if (token == null)
            return Unauthorized("Credenciais inválidas.");

        return Ok(new { token });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader))
            return BadRequest("Token não fornecido.");

        var accessToken = authHeader.Replace("Bearer ", "");

        var success = await _keycloakService.LogoutAsync(accessToken);
        if (!success)
            return BadRequest("Erro ao efetuar logout.");

        return Ok("Logout realizado com sucesso.");
    }
}
