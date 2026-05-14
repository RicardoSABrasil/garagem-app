/**
 * Perfil completo do usuário
 */
export interface UserProfile {
  Id: string;
  FirstName: string;
  LastName: string;
  Email: string;
  PhoneNumber?: string;
  SecondaryPhone?: string;
  Bio?: string;
  ProfileImageUrl?: string;
  BirthDate?: string;
  ZipCode?: string;
  Street?: string;
  Number?: string;
  District?: string;
  City?: string;
  State?: string;
  Country?: string;
  CreatedAt: string;
  UpdatedAt: string;
  LastLoginAt?: string;
}

/**
 * Requisição para atualizar perfil
 */
export interface UpdateProfileRequest {
  FirstName: string;
  LastName: string;
  Bio?: string;
  PhoneNumber?: string;
  BirthDate?: string;
  ZipCode?: string;
  Street?: string;
  Number?: string;
  District?: string;
  City?: string;
  State?: string;
  Country?: string;
}

/**
 * Resposta de upload de imagem
 */
export interface UploadImageResponse {
  ImageUrl: string;
  Message: string;
}
