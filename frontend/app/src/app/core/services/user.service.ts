import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserProfile, UpdateProfileRequest, UploadImageResponse } from '../../shared/models';

/**
 * Serviço para gerenciar dados do perfil do usuário
 */
@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/users`;

  /**
   * Obter perfil do usuário autenticado
   */
  getCurrentUser(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.apiUrl}/me`);
  }

  /**
   * Atualizar perfil do usuário
   */
  updateProfile(data: UpdateProfileRequest): Observable<UserProfile> {
    return this.http.put<UserProfile>(`${this.apiUrl}/profile`, data);
  }

  /**
   * Fazer upload de foto de perfil
   */
  uploadProfileImage(file: File): Observable<UploadImageResponse> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<UploadImageResponse>(
      `${this.apiUrl}/profile-image`,
      formData
    );
  }
}
