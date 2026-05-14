# 📱 Sistema de Perfil de Usuário - Documentação Completa

## 🏗️ Arquitetura Implementada

A implementação segue os princípios de **Domain-Driven Design (DDD)** com camadas bem separadas:

```
┌─────────────────────────────────────────────────────────┐
│ Garagem.Api (Controllers - Presentation Layer)          │
├─────────────────────────────────────────────────────────┤
│ ProfileController                                       │
│  - GET /api/users/me          (GetCurrentUserHandler)   │
│  - PUT /api/users/profile     (UpdateProfileHandler)    │
│  - POST /api/users/profile-image (UploadProfileImageHandler) │
└─────────────────────────────────────────────────────────┘
          ↓
┌─────────────────────────────────────────────────────────┐
│ Garagem.Application (Use Cases - Application Layer)     │
├─────────────────────────────────────────────────────────┤
│ GetCurrentUserHandler     → Busca perfil do usuário     │
│ UpdateProfileHandler      → Atualiza dados do perfil    │
│ UploadProfileImageHandler → Faz upload de imagem        │
└─────────────────────────────────────────────────────────┘
          ↓
┌─────────────────────────────────────────────────────────┐
│ Garagem.Domain (Business Logic - Domain Layer)          │
├─────────────────────────────────────────────────────────┤
│ User Entity              → Entidade de domínio          │
│ IUserRepository          → Interface de persistência    │
│ IStorageService          → Interface de armazenamento   │
└─────────────────────────────────────────────────────────┘
          ↓
┌─────────────────────────────────────────────────────────┐
│ Garagem.Infrastructure (Implementation - Infrastructure) │
├─────────────────────────────────────────────────────────┤
│ UserRepository           → Implementação do repositório │
│ MinioStorageService      → Implementação MinIO/S3       │
│ AppDbContext             → EF Core DbContext            │
└─────────────────────────────────────────────────────────┘
```

---

## 📁 Estrutura de Arquivos Criados

### 1. **DTOs (Application/DTOs/ProfileDtos.cs)**
```
UserProfileResponse     → Response com dados completos do perfil
UpdateProfileRequest    → Request para atualizar perfil
UploadProfileImageResponse → Response do upload de imagem
```

### 2. **Interfaces (Domain/Interfaces/IStorageService.cs)**
Abstrai o serviço de armazenamento (MinIO/S3):
- `UploadAsync()` - Upload de arquivo
- `DeleteAsync()` - Deleção de arquivo
- `GeneratePresignedUrl()` - URL assinada/segura

### 3. **Implementação MinIO (Infrastructure/Storage/MinioStorageService.cs)**
- Usa AWS SDK S3 (compatível com MinIO)
- Gera nomes únicos para arquivos
- Suporta deleção de imagens anteriores
- Gera URLs públicas automáticas

### 4. **Handlers/Use Cases (Application/UseCases/Profile/)**
- `GetCurrentUserHandler` - Obter perfil do usuário autenticado
- `UpdateProfileHandler` - Atualizar dados do perfil com validações
- `UploadProfileImageHandler` - Upload com validação de arquivo

### 5. **Controller (Api/Controllers/ProfileController.cs)**
- `GET /api/users/me` - Retorna perfil completo
- `PUT /api/users/profile` - Atualiza informações do usuário
- `POST /api/users/profile-image` - Upload de foto de perfil

### 6. **Configuração (appsettings.json)**
Adicionada seção MinIO com:
```json
"Minio": {
  "Endpoint": "http://localhost:9000",
  "AccessKey": "minioadmin",
  "SecretKey": "minioadmin",
  "BucketName": "garagem-bucket",
  "UseSSL": false,
  "Region": "us-east-1"
}
```

---

## 🔐 Segurança e Validações

### JWT (JSON Web Tokens)
- ✅ Autenticação com `[Authorize]`
- ✅ Extração do UserId do JWT (`sub` claim)
- ✅ Validação de token em todas as requisições

### Validação de Imagem
- ✅ Tamanho máximo: **5 MB**
- ✅ Extensões permitidas: `.jpg`, `.jpeg`, `.png`, `.webp`
- ✅ Valida antes de fazer upload

### Validação de Perfil
- ✅ FirstName e LastName obrigatórios
- ✅ Bio máximo 500 caracteres
- ✅ Data de nascimento não pode ser no futuro
- ✅ Usuário deve estar ativo

---

## 🚀 Como Testar via Swagger

### 1. **Iniciar o Backend**
```bash
cd backend/src/Garagem.Api
dotnet run
```
Acesse: `http://localhost:5000/swagger`

### 2. **Fluxo Completo de Teste**

#### **Passo 1: Registrar Usuário**
```http
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "João",
  "lastName": "Silva",
  "email": "joao@exemplo.com",
  "password": "Senha@123"
}
```

#### **Passo 2: Fazer Login**
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "joao@exemplo.com",
  "password": "Senha@123"
}
```
**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### **Passo 3: Obter Perfil Atual**
```http
GET /api/users/me
Authorization: Bearer <TOKEN_RECEBIDO>
```

**Resposta:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "firstName": "João",
  "lastName": "Silva",
  "email": "joao@exemplo.com",
  "phoneNumber": null,
  "secondaryPhone": null,
  "bio": null,
  "profileImageUrl": null,
  "birthDate": null,
  "zipCode": null,
  "street": null,
  "number": null,
  "district": null,
  "city": null,
  "state": null,
  "country": null,
  "createdAt": "2026-05-14T06:42:00Z",
  "updatedAt": "2026-05-14T06:42:00Z",
  "lastLoginAt": "2026-05-14T06:43:00Z"
}
```

#### **Passo 4: Atualizar Perfil**
```http
PUT /api/users/profile
Authorization: Bearer <TOKEN_RECEBIDO>
Content-Type: application/json

{
  "firstName": "João",
  "lastName": "Silva",
  "bio": "Desenvolvedor Full Stack apaixonado por tecnologia",
  "phoneNumber": "(11) 98765-4321",
  "birthDate": "1990-03-15T00:00:00Z",
  "zipCode": "01310-100",
  "street": "Avenida Paulista",
  "number": "1000",
  "district": "Cerqueira César",
  "city": "São Paulo",
  "state": "SP",
  "country": "Brasil"
}
```

#### **Passo 5: Upload de Foto de Perfil**
```http
POST /api/users/profile-image
Authorization: Bearer <TOKEN_RECEBIDO>
Content-Type: multipart/form-data

[Selecione um arquivo de imagem (JPG, PNG, WEBP - máx 5MB)]
```

**Resposta:**
```json
{
  "imageUrl": "http://localhost:9000/garagem-bucket/profile-images/a1b2c3d4-1234567890.jpg",
  "message": "Imagem de perfil enviada com sucesso"
}
```

#### **Passo 6: Verificar Imagem no Perfil**
```http
GET /api/users/me
Authorization: Bearer <TOKEN_RECEBIDO>
```

A resposta agora incluirá `profileImageUrl` preenchida!

---

## 🔧 Dependency Injection no Program.cs

```csharp
// Registra handlers do perfil
builder.Services.AddScoped<GetCurrentUserHandler>();
builder.Services.AddScoped<UpdateProfileHandler>();
builder.Services.AddScoped<UploadProfileImageHandler>();

// Configura cliente S3 para MinIO
var config = new AmazonS3Config
{
    ServiceURL = minioEndpoint,
    ForcePathStyle = true,
    SignatureVersion = "4"
};

builder.Services.AddScoped<IAmazonS3>(sp =>
    new AmazonS3Client(minioAccessKey, minioSecretKey, config));

// Registra serviço de armazenamento
builder.Services.AddScoped<IStorageService, MinioStorageService>();
```

---

## 💾 Padrões de Armazenamento de Imagem

### Estrutura de Nomes
```
garagem-bucket/
├── profile-images/
│   ├── a1b2c3d4-1234567890.jpg
│   ├── e5f6g7h8-1234567891.png
│   └── i9j0k1l2-1234567892.webp
```

**Por que assim?**
- `profile-images/` - Organiza imagens de perfil
- `{GUID}-{TIMESTAMP}` - Garante unicidade absoluta
- Reutiliza nomes sem conflito

### URL Pública Gerada
```
http://localhost:9000/garagem-bucket/profile-images/a1b2c3d4-1234567890.jpg
```

---

## 🌐 Fluxo Completo de Imagem

```
1. Usuário faz upload de arquivo
   ↓
2. UploadProfileImageHandler recebe IFormFile
   ↓
3. Valida: tamanho, extensão, usuário ativo
   ↓
4. Deleta imagem anterior (se existir) via IStorageService
   ↓
5. MinioStorageService faz upload no MinIO
   ↓
6. Gera nome único: profile-images/{GUID}-{TIMESTAMP}.ext
   ↓
7. Retorna URL pública: http://localhost:9000/garagem-bucket/...
   ↓
8. User.UpdateProfileImage(url) atualiza entidade
   ↓
9. UserRepository.UpdateAsync() persiste no banco
   ↓
10. Retorna UploadProfileImageResponse com URL
```

---

## 📊 Campos do Perfil Atualizáveis

| Campo | Tipo | Validação |
|-------|------|-----------|
| FirstName | string | Obrigatório, máx 100 chars |
| LastName | string | Obrigatório, máx 100 chars |
| Bio | string | Opcional, máx 500 chars |
| PhoneNumber | string | Opcional |
| SecondaryPhone | string | Opcional |
| BirthDate | DateTime | Opcional, não pode ser futuro |
| ZipCode | string | Opcional |
| Street | string | Opcional |
| Number | string | Opcional |
| District | string | Opcional |
| City | string | Opcional |
| State | string | Opcional |
| Country | string | Opcional |
| ProfileImageUrl | string | Gerada automaticamente |

---

## 🔄 Tratamento de Erros

Todos os handlers retornam mensagens claras:

```json
{
  "message": "Descrição do erro"
}
```

### Códigos HTTP
- `200 OK` - Sucesso
- `400 Bad Request` - Validação falhou
- `401 Unauthorized` - Sem autenticação
- `404 Not Found` - Usuário não encontrado
- `500 Internal Server Error` - Erro no servidor

---

## 🎯 Próximos Passos (Sugestões)

1. **Logging**
   - Integrar Serilog para logs estruturados
   - Log de uploads com sucesso/erro

2. **Notificações**
   - Email quando foto é alterada
   - Auditoria de mudanças de perfil

3. **Cache**
   - Redis para cachear perfil de usuário
   - Invalidação ao atualizar

4. **Validação Avançada**
   - Detector de faces em uploads
   - Varredura de vírus em arquivos

5. **Performance**
   - Compressão de imagens
   - Geração de thumbnails
   - CDN para servir imagens

---

## 📦 Dependências Adicionadas

```xml
<PackageReference Include="AWSSDK.S3" Version="3.7.410.6" />
<PackageReference Include="Microsoft.AspNetCore.Http.Abstractions" Version="2.3.10" />
```

---

## ✅ Checklist de Implementação

- ✅ Criada interface IStorageService (Domain)
- ✅ Implementado MinioStorageService (Infrastructure)
- ✅ Criados DTOs de perfil (Application)
- ✅ Implementados 3 handlers (Application)
- ✅ Criado ProfileController (Api)
- ✅ Configurado MinIO em appsettings.json
- ✅ Registrado Dependency Injection
- ✅ Validações de arquivo e perfil
- ✅ JWT integration
- ✅ Projeto compilando sem erros

---

**Desenvolvido com ❤️ usando DDD, Clean Architecture e .NET 10**
