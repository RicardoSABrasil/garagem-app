/**
 * Perfil completo do usuário
 */
export interface UserProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  secondaryPhone?: string;
  bio?: string;
  profileImageUrl?: string;
  birthDate?: string;
  zipCode?: string;
  street?: string;
  number?: string;
  district?: string;
  city?: string;
  state?: string;
  country?: string;
  createdAt: string;
  updatedAt: string;
  lastLoginAt?: string;
}

/**
 * Requisição para atualizar perfil
 */
export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  bio?: string;
  phoneNumber?: string;
  birthDate?: string;
  zipCode?: string;
  street?: string;
  number?: string;
  district?: string;
  city?: string;
  state?: string;
  country?: string;
}

/**
 * Resposta de upload de imagem
 */
export interface UploadImageResponse {
  imageUrl: string;
  message: string;
}
