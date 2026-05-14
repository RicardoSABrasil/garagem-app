import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services';
import { RegisterRequest } from '../../../shared/models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  isLoading = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;

  readonly registerForm = this.formBuilder.group({
    FirstName: ['', [Validators.required, Validators.minLength(2)]],
    LastName: ['', [Validators.required, Validators.minLength(2)]],
    Email: ['', [Validators.required, Validators.email]],
    Password: ['', [Validators.required, Validators.minLength(6)]]
  });

  onSubmit(): void {
    if (this.registerForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;
    this.successMessage = null;

    const newUser: RegisterRequest = this.registerForm.value as RegisterRequest;

    this.authService.register(newUser).subscribe({
      next: () => {
        this.successMessage = 'Registro realizado com sucesso! Redirecionando...';
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 1600);
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage =
          error.error?.message || 'Erro ao registrar. Tente novamente.';
      }
    });
  }

  getFieldError(fieldName: keyof RegisterRequest): string | null {
    const control = this.registerForm.get(fieldName);
    if (!control?.touched) return null;

    if (control.hasError('required')) return `${fieldName} é obrigatório`;
    if (control.hasError('minlength'))
      return `${fieldName} deve ter no mínimo ${control.getError('minlength').requiredLength} caracteres`;
    if (control.hasError('email')) return 'Email inválido';

    return null;
  }
}
