import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest } from '../../shared/models';

/**
 * Serviço de autenticação que gerencia JWT
 * SSR-safe usando isPlatformBrowser
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly platformId = inject(PLATFORM_ID);
  private readonly apiUrl = `${environment.apiUrl}/api/auth`;

  private readonly TOKEN_KEY = 'auth_token';
  private readonly isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
  public readonly isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials);
  }

  register(data: RegisterRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/register`, data);
  }

  saveToken(token: string): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(this.TOKEN_KEY, token);
      this.isAuthenticatedSubject.next(true);
    }
  }

  getToken(): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem(this.TOKEN_KEY);
    }
    return null;
  }

  hasToken(): boolean {
    if (isPlatformBrowser(this.platformId)) {
      return !!localStorage.getItem(this.TOKEN_KEY);
    }
    return false;
  }

  logout(): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(this.TOKEN_KEY);
      this.isAuthenticatedSubject.next(false);
    }
  }

  isAuthenticated(): boolean {
    return this.hasToken();
  }
}
