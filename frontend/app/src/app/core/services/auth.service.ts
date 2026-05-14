import { Injectable, inject, Platform } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models';

/**
 * Serviço de autenticação que gerencia JWT
 * SSR-safe usando isPlatformBrowser
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly platform = inject(Platform);
  private readonly apiUrl = `${environment.apiUrl}/api/auth`;

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  private readonly TOKEN_KEY = 'auth_token';

  /**
   * Fazer login com email e senha
   */
  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials);
  }

  /**
   * Registrar novo usuário
   */
  register(data: RegisterRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/register`, data);
  }

  /**
   * Salvar token no localStorage
   */
  saveToken(token: string): void {
    if (isPlatformBrowser(this.platform)) {
      localStorage.setItem(this.TOKEN_KEY, token);
      this.isAuthenticatedSubject.next(true);
    }
  }

  /**
   * Obter token do localStorage
   */
  getToken(): string | null {
    if (isPlatformBrowser(this.platform)) {
      return localStorage.getItem(this.TOKEN_KEY);
    }
    return null;
  }

  /**
   * Verificar se tem token válido
   */
  hasToken(): boolean {
    if (isPlatformBrowser(this.platform)) {
      return !!localStorage.getItem(this.TOKEN_KEY);
    }
    return false;
  }

  /**
   * Fazer logout removendo token
   */
  logout(): void {
    if (isPlatformBrowser(this.platform)) {
      localStorage.removeItem(this.TOKEN_KEY);
      this.isAuthenticatedSubject.next(false);
    }
  }

  /**
   * Obter estado de autenticação sincronamente
   */
  isAuthenticated(): boolean {
    return this.hasToken();
  }
}
