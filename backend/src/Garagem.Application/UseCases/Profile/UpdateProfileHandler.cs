using Garagem.Application.DTOs;
using Garagem.Domain.Interfaces;

namespace Garagem.Application.UseCases.Profile;

/// <summary>
/// Handler para atualizar o perfil do usuário autenticado
/// </summary>
public class UpdateProfileHandler
{
	private readonly IUserRepository _userRepository;

	public UpdateProfileHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<UserProfileResponse> Handle(
		Guid userId,
		UpdateProfileRequest request,
		CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByIdAsync(userId);

		if (user == null)
			throw new Exception("Usuário não encontrado");

		if (!user.IsActive)
			throw new Exception("Usuário inativo");

		// Validação básica
		ValidateRequest(request);

		// Atualiza o perfil do usuário
		user.UpdateProfile(
			firstName: request.FirstName,
			lastName: request.LastName,
			bio: request.Bio,
			phoneNumber: request.PhoneNumber,
			birthDate: request.BirthDate
		);

		// Atualiza o endereço
		user.UpdateAddress(
			zipCode: request.ZipCode,
			street: request.Street,
			number: request.Number,
			district: request.District,
			city: request.City,
			state: request.State,
			country: request.Country
		);

		// Persiste as alterações
		await _userRepository.UpdateAsync(user);

		// Retorna o perfil atualizado
		return new UserProfileResponse(
			Id: user.Id,
			FirstName: user.FirstName,
			LastName: user.LastName,
			Email: user.Email,
			PhoneNumber: user.PhoneNumber,
			SecondaryPhone: user.SecondaryPhone,
			Bio: user.Bio,
			ProfileImageUrl: user.ProfileImageUrl,
			BirthDate: user.BirthDate,
			ZipCode: user.ZipCode,
			Street: user.Street,
			Number: user.Number,
			District: user.District,
			City: user.City,
			State: user.State,
			Country: user.Country,
			CreatedAt: user.CreatedAt,
			UpdatedAt: user.UpdatedAt,
			LastLoginAt: user.LastLoginAt
		);
	}

	private static void ValidateRequest(UpdateProfileRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.FirstName))
			throw new ArgumentException("FirstName é obrigatório", nameof(request.FirstName));

		if (string.IsNullOrWhiteSpace(request.LastName))
			throw new ArgumentException("LastName é obrigatório", nameof(request.LastName));

		if (!string.IsNullOrEmpty(request.FirstName) && request.FirstName.Length > 100)
			throw new ArgumentException("FirstName não pode ter mais de 100 caracteres", nameof(request.FirstName));

		if (!string.IsNullOrEmpty(request.LastName) && request.LastName.Length > 100)
			throw new ArgumentException("LastName não pode ter mais de 100 caracteres", nameof(request.LastName));

		if (request.Bio != null && request.Bio.Length > 500)
			throw new ArgumentException("Bio não pode ter mais de 500 caracteres", nameof(request.Bio));

		if (request.BirthDate.HasValue && request.BirthDate.Value > DateTime.UtcNow)
			throw new ArgumentException("Data de nascimento não pode ser no futuro", nameof(request.BirthDate));
	}
}
