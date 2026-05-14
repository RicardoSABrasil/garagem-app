using Garagem.Application.DTOs;
using Garagem.Application.UseCases.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace Garagem.Api.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : Controller
	{
		private readonly RegisterUserHandler _register;
		private readonly LoginUserHandler _login;

		public AuthController(RegisterUserHandler register, LoginUserHandler login)
		{
			_register = register;
			_login = login;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterRequest request)
		{
			await _register.Handle(request);
			return Ok();
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequest request)
		{
			var result = await _login.Handle(request);
			return Ok(result);
		}
	}
}
