namespace Garagem.Application.DTOs;

/// <summary>
/// DTO para resposta do perfil do usuário autenticado
/// </summary>
public record UserProfileResponse(
	Guid Id,
	string FirstName,
	string LastName,
	string Email,
	string? PhoneNumber,
	string? SecondaryPhone,
	string? Bio,
	string? ProfileImageUrl,
	DateTime? BirthDate,
	string? ZipCode,
	string? Street,
	string? Number,
	string? District,
	string? City,
	string? State,
	string? Country,
	DateTime CreatedAt,
	DateTime UpdatedAt,
	DateTime? LastLoginAt
);

/// <summary>
/// DTO para atualização de perfil do usuário
/// </summary>
public class UpdateProfileRequest
{
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string? Bio { get; set; }
	public string? PhoneNumber { get; set; }
	public DateTime? BirthDate { get; set; }
	
	// Endereço
	public string? ZipCode { get; set; }
	public string? Street { get; set; }
	public string? Number { get; set; }
	public string? District { get; set; }
	public string? City { get; set; }
	public string? State { get; set; }
	public string? Country { get; set; }
}

/// <summary>
/// DTO para resposta de upload de imagem de perfil
/// </summary>
public record UploadProfileImageResponse(
	string ImageUrl,
	string Message
);
