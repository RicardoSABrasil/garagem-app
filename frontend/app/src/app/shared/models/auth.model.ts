/**
 * Resposta de autenticação com token JWT
 */
export interface AuthResponse {
  token: string;
}

/**
 * Requisição de login
 */
export interface LoginRequest {
  email: string;
  password: string;
}

/**
 * Requisição de registro
 */
export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}
