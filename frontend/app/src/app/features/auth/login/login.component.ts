import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  isLoading = false;
  errorMessage: string | null = null;

  loginForm = this.formBuilder.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    this.authService.login(this.loginForm.value as any).subscribe({
      next: (response) => {
        this.authService.saveToken(response.token);
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
    const control = this.loginForm.get('email');
    if (control?.hasError('required')) return 'Email é obrigatório';
    if (control?.hasError('email')) return 'Email inválido';
    return null;
  }

  get passwordError(): string | null {
    const control = this.loginForm.get('password');
    if (control?.hasError('required')) return 'Senha é obrigatória';
    if (control?.hasError('minlength'))
      return 'Senha deve ter no mínimo 6 caracteres';
    return null;
  }
}
