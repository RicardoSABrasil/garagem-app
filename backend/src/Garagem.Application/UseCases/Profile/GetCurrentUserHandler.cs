using Garagem.Application.DTOs;
using Garagem.Domain.Interfaces;

namespace Garagem.Application.UseCases.Profile;

/// <summary>
/// Handler para obter o perfil do usuário autenticado
/// </summary>
public class GetCurrentUserHandler
{
	private readonly IUserRepository _userRepository;

	public GetCurrentUserHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<UserProfileResponse> Handle(Guid userId, CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByIdAsync(userId);

		if (user == null)
			throw new Exception("Usuário não encontrado");

		if (!user.IsActive)
			throw new Exception("Usuário inativo");

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
}
