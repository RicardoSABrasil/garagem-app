import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  isLoading = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;

  registerForm = this.formBuilder.group({
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  onSubmit(): void {
    if (this.registerForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;
    this.successMessage = null;

    this.authService.register(this.registerForm.value as any).subscribe({
      next: () => {
        this.successMessage = 'Registro realizado com sucesso! Redirecionando...';
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage =
          error.error?.message || 'Erro ao registrar. Tente novamente.';
      }
    });
  }

  getFieldError(fieldName: string): string | null {
    const control = this.registerForm.get(fieldName);
    if (!control?.touched) return null;

    if (control.hasError('required')) return `${fieldName} é obrigatório`;
    if (control.hasError('minlength'))
      return `${fieldName} deve ter no mínimo ${control.getError('minlength').requiredLength} caracteres`;
    if (control.hasError('email')) return 'Email inválido';

    return null;
  }
}
