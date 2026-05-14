using Garagem.Application.DTOs;
using Garagem.Application.UseCases.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Garagem.Api.Controllers;

/// <summary>
/// Controller para gerenciar o perfil do usuário autenticado
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize]
public class ProfileController : ControllerBase
{
	private readonly GetCurrentUserHandler _getCurrentUserHandler;
	private readonly UpdateProfileHandler _updateProfileHandler;
	private readonly UploadProfileImageHandler _uploadProfileImageHandler;

	public ProfileController(
		GetCurrentUserHandler getCurrentUserHandler,
		UpdateProfileHandler updateProfileHandler,
		UploadProfileImageHandler uploadProfileImageHandler)
	{
		_getCurrentUserHandler = getCurrentUserHandler;
		_updateProfileHandler = updateProfileHandler;
		_uploadProfileImageHandler = uploadProfileImageHandler;
	}

	/// <summary>
	/// Obtém o perfil do usuário autenticado
	/// </summary>
	/// <returns>Perfil completo do usuário</returns>
	[HttpGet("me")]
	public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken = default)
	{
		try
		{
			var userId = GetUserIdFromClaims();

			var profile = await _getCurrentUserHandler.Handle(userId, cancellationToken);

			return Ok(profile);
		}
		catch (Exception ex)
		{
			return NotFound(new { message = ex.Message });
		}
	}

	/// <summary>
	/// Atualiza o perfil do usuário autenticado
	/// </summary>
	/// <param name="request">Dados atualizados do perfil</param>
	/// <returns>Perfil atualizado</returns>
	[HttpPut("profile")]
	public async Task<IActionResult> UpdateProfile(
		[FromBody] UpdateProfileRequest request,
		CancellationToken cancellationToken = default)
	{
		try
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var userId = GetUserIdFromClaims();

			var updatedProfile = await _updateProfileHandler.Handle(userId, request, cancellationToken);

			return Ok(updatedProfile);
		}
		catch (ArgumentException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
		catch (Exception ex)
		{
			return NotFound(new { message = ex.Message });
		}
	}

	/// <summary>
	/// Faz upload de imagem de perfil do usuário autenticado
	/// </summary>
	/// <param name="file">Arquivo de imagem (JPG, PNG, WEBP até 5MB)</param>
	/// <returns>URL da imagem enviada</returns>
	[HttpPost("profile-image")]
	[Consumes("multipart/form-data")]
	public async Task<IActionResult> UploadProfileImage(
		[FromForm] IFormFile file,
		CancellationToken cancellationToken = default)
	{
		try
		{
			if (file == null || file.Length == 0)
				return BadRequest(new { message = "Nenhum arquivo foi enviado" });

			var userId = GetUserIdFromClaims();

			var result = await _uploadProfileImageHandler.Handle(userId, file, cancellationToken);

			return Ok(result);
		}
		catch (ArgumentException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
		catch (Exception ex)
		{
			return NotFound(new { message = ex.Message });
		}
	}

	/// <summary>
	/// Extrai o ID do usuário do JWT
	/// </summary>
	private Guid GetUserIdFromClaims()
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
			?? User.FindFirst("sub");

		if (userIdClaim == null)
			throw new Exception("UserId não encontrado no JWT");

		if (!Guid.TryParse(userIdClaim.Value, out var userId))
			throw new Exception("UserId inválido no JWT");

		return userId;
	}
}
