import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services';
import { AuthResponse, LoginRequest } from '../../../shared/models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  isLoading = false;
  errorMessage: string | null = null;

  readonly loginForm = this.formBuilder.group({
    Email: ['', [Validators.required, Validators.email]],
    Password: ['', [Validators.required, Validators.minLength(6)]]
  });

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    const credentials: LoginRequest = this.loginForm.value as LoginRequest;

    this.authService.login(credentials).subscribe({
      next: (response: AuthResponse) => {
        this.authService.saveToken(response.Token);
        this.router.navigate(['/profile']);
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage =
          error.error?.message || 'Erro ao fazer login. Tente novamente.';
      }
    });
  }

  get emailError(): string | null {
    const control = this.loginForm.get('Email');
    if (control?.hasError('required')) return 'Email é obrigatório';
    if (control?.hasError('email')) return 'Email inválido';
    return null;
  }

  get passwordError(): string | null {
    const control = this.loginForm.get('Password');
    if (control?.hasError('required')) return 'Senha é obrigatória';
    if (control?.hasError('minlength'))
      return 'Senha deve ter no mínimo 6 caracteres';
    return null;
  }
}
