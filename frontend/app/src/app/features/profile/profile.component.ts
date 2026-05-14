import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { UserService, AuthService } from '../../core/services';
import { UserProfile, UpdateProfileRequest, UploadImageResponse } from '../../shared/models';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly userService = inject(UserService);
  private readonly authService = inject(AuthService);

  isLoading = true;
  isSaving = false;
  isUploading = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;

  userProfile: UserProfile | null = null;
  previewImageUrl: string | null = null;

  profileForm = this.formBuilder.group({
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName: ['', [Validators.required, Validators.minLength(2)]],
    bio: [''],
    phoneNumber: [''],
    birthDate: [''],
    zipCode: [''],
    street: [''],
    number: [''],
    district: [''],
    city: [''],
    state: [''],
    country: ['']
  });

  ngOnInit(): void {
    this.loadUserProfile();
  }

  loadUserProfile(): void {
    this.isLoading = true;
    this.userService.getCurrentUser().subscribe({
      next: (profile: UserProfile) => {
        this.userProfile = profile;
        this.previewImageUrl = profile.ProfileImageUrl || null;
        this.populateForm(profile);
        this.isLoading = false;
      },
      error: (error: any) => {
        this.isLoading = false;
        this.errorMessage = 'Erro ao carregar perfil. Tente novamente.';
        console.error(error);
      }
    });
  }

  private populateForm(profile: UserProfile): void {
    this.profileForm.patchValue({
      firstName: profile.FirstName,
      lastName: profile.LastName,
      bio: profile.Bio,
      phoneNumber: profile.PhoneNumber,
      birthDate: this.formatDateForInput(profile.BirthDate),
      zipCode: profile.ZipCode,
      street: profile.Street,
      number: profile.Number,
      district: profile.District,
      city: profile.City,
      state: profile.State,
      country: profile.Country
    });
  }

  private formatDateForInput(dateString?: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toISOString().split('T')[0];
  }

  onProfileSubmit(): void {
    if (this.profileForm.invalid) {
      return;
    }

    this.isSaving = true;
    this.errorMessage = null;
    this.successMessage = null;

    const updateRequest: UpdateProfileRequest = this.profileForm.value as any;

    this.userService.updateProfile(updateRequest).subscribe({
      next: (profile: UserProfile) => {
        this.userProfile = profile;
        this.isSaving = false;
        this.successMessage = 'Perfil atualizado com sucesso!';
        setTimeout(() => {
          this.successMessage = null;
        }, 3000);
      },
      error: (error: any) => {
        this.isSaving = false;
        this.errorMessage = error.error?.message || 'Erro ao atualizar perfil.';
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = input.files;

    if (!files || files.length === 0) {
      return;
    }

    const file = files[0];

    // Validação básica
    if (!file.type.startsWith('image/')) {
      this.errorMessage = 'Por favor, selecione uma imagem válida.';
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      this.errorMessage = 'A imagem deve ter no máximo 5MB.';
      return;
    }

    // Preview
    const reader = new FileReader();
    reader.onload = (e) => {
      this.previewImageUrl = (e.target?.result as string) || null;
    };
    reader.readAsDataURL(file);

    // Upload
    this.isUploading = true;
    this.errorMessage = null;

    this.userService.uploadProfileImage(file).subscribe({
      next: (response: UploadImageResponse) => {
        this.isUploading = false;
        this.successMessage = response.Message;
        if (this.userProfile) {
          this.userProfile.ProfileImageUrl = response.ImageUrl;
        }
        setTimeout(() => {
          this.successMessage = null;
        }, 3000);
      },
      error: (error: any) => {
        this.isUploading = false;
        this.errorMessage = error.error?.message || 'Erro ao fazer upload da imagem.';
      }
    });
  }

  onLogout(): void {
    this.authService.logout();
    // Router vai ser injetado se precisar fazer programatic redirect
  }

  getFieldError(fieldName: string): string | null {
    const control = this.profileForm.get(fieldName);
    if (!control?.touched) return null;

    if (control.hasError('required')) return `${fieldName} é obrigatório`;
    if (control.hasError('minlength'))
      return `${fieldName} deve ter no mínimo ${control.getError('minlength').requiredLength} caracteres`;

    return null;
  }
}
