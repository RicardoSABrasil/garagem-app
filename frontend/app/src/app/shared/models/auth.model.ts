/**
 * Resposta de autenticação com token JWT
 */
export interface AuthResponse {
  Token: string;
}

/**
 * Requisição de login
 */
export interface LoginRequest {
  Email: string;
  Password: string;
}

/**
 * Requisição de registro
 */
export interface RegisterRequest {
  FirstName: string;
  LastName: string;
  Email: string;
  Password: string;
}
