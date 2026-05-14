using Garagem.Application.DTOs;
using Garagem.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Garagem.Application.UseCases.Profile;

/// <summary>
/// Handler para fazer upload de imagem de perfil do usuário
/// </summary>
public class UploadProfileImageHandler
{
	private readonly IUserRepository _userRepository;
	private readonly IStorageService _storageService;

	private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
	private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

	public UploadProfileImageHandler(
		IUserRepository userRepository,
		IStorageService storageService)
	{
		_userRepository = userRepository;
		_storageService = storageService;
	}

	public async Task<UploadProfileImageResponse> Handle(
		Guid userId,
		IFormFile file,
		CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByIdAsync(userId);

		if (user == null)
			throw new Exception("Usuário não encontrado");

		if (!user.IsActive)
			throw new Exception("Usuário inativo");

		// Validação do arquivo
		ValidateFile(file);

		try
		{
			// Remove imagem anterior se existir
			if (!string.IsNullOrEmpty(user.ProfileImageUrl))
			{
				try
				{
					await _storageService.DeleteAsync(user.ProfileImageUrl, cancellationToken);
				}
				catch (Exception ex)
				{
					// Log erro mas não falha o upload
					Console.WriteLine($"Erro ao deletar imagem anterior: {ex.Message}");
				}
			}

			// Faz upload da nova imagem
			var imageUrl = await _storageService.UploadAsync(
				fileName: file.FileName,
				fileStream: file.OpenReadStream(),
				contentType: file.ContentType,
				cancellationToken: cancellationToken
			);

			// Atualiza o usuário com a nova URL
			user.UpdateProfileImage(imageUrl);
			await _userRepository.UpdateAsync(user);

			return new UploadProfileImageResponse(
				ImageUrl: imageUrl,
				Message: "Imagem de perfil enviada com sucesso"
			);
		}
		catch (Exception ex)
		{
			throw new Exception($"Erro ao fazer upload da imagem: {ex.Message}", ex);
		}
	}

	private static void ValidateFile(IFormFile file)
	{
		if (file == null || file.Length == 0)
			throw new ArgumentException("Arquivo não pode estar vazio", nameof(file));

		if (file.Length > MaxFileSizeInBytes)
			throw new ArgumentException(
				$"Arquivo muito grande. Tamanho máximo: {MaxFileSizeInBytes / (1024 * 1024)}MB",
				nameof(file)
			);

		var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

		if (!AllowedExtensions.Contains(extension))
			throw new ArgumentException(
				$"Extensão de arquivo não permitida. Extensões válidas: {string.Join(", ", AllowedExtensions)}",
				nameof(file)
			);
	}
}
