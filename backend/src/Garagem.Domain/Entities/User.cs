using Garagem.Domain.Enums;

namespace Garagem.Domain.Entities;

public class User
{
	private string hash;

	public Guid Id { get; private set; }

	// identidade
	public string FirstName { get; private set; } = string.Empty;
	public string LastName { get; private set; } = string.Empty;

	public string FullName =>
		$"{FirstName} {LastName}";

	// login
	public string Email { get; private set; } = string.Empty;
	public string PasswordHash { get; private set; } = string.Empty;

	// contato
	public string? PhoneNumber { get; private set; }
	public string? SecondaryPhone { get; private set; }

	// perfil
	public string? Bio { get; private set; }
	public string? ProfileImageUrl { get; private set; }

	public DateTime? BirthDate { get; private set; }

	// endereço
	public string? ZipCode { get; private set; }
	public string? Street { get; private set; }
	public string? Number { get; private set; }
	public string? District { get; private set; }
	public string? City { get; private set; }
	public string? State { get; private set; }
	public string? Country { get; private set; }

	// controle
	public bool IsActive { get; private set; }
	public bool EmailConfirmed { get; private set; }

	public DateTime CreatedAt { get; private set; }
	public DateTime UpdatedAt { get; private set; }

	public DateTime? LastLoginAt { get; private set; }

	// permissão
	public UserRole Role { get; private set; }

	private User() { }

	public User(
		string firstName,
		string lastName,
		string email,
		string passwordHash)
	{
		Id = Guid.NewGuid();

		FirstName = firstName;
		LastName = lastName;

		Email = email;
		PasswordHash = passwordHash;

		IsActive = true;
		EmailConfirmed = false;

		Role = UserRole.User;

		CreatedAt = DateTime.UtcNow;
		UpdatedAt = DateTime.UtcNow;
	}

	public User(string email, string hash)
	{
		Email = email;
		this.hash = hash;
	}

	public void UpdateProfile(
		string firstName,
		string lastName,
		string? bio,
		string? phoneNumber,
		DateTime? birthDate)
	{
		FirstName = firstName;
		LastName = lastName;

		Bio = bio;
		PhoneNumber = phoneNumber;

		BirthDate = birthDate;

		UpdatedAt = DateTime.UtcNow;
	}

	public void UpdateAddress(
		string? zipCode,
		string? street,
		string? number,
		string? district,
		string? city,
		string? state,
		string? country)
	{
		ZipCode = zipCode;
		Street = street;
		Number = number;
		District = district;
		City = city;
		State = state;
		Country = country;

		UpdatedAt = DateTime.UtcNow;
	}

	public void UpdateProfileImage(string imageUrl)
	{
		ProfileImageUrl = imageUrl;

		UpdatedAt = DateTime.UtcNow;
	}

	public void RegisterLogin()
	{
		LastLoginAt = DateTime.UtcNow;
	}

	public void ChangeRole(UserRole role)
	{
		Role = role;

		UpdatedAt = DateTime.UtcNow;
	}

	public void Disable()
	{
		IsActive = false;

		UpdatedAt = DateTime.UtcNow;
	}
}