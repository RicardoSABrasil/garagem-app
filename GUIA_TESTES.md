# 🧪 Guia Rápido de Testes - API de Perfil

## 📋 Pré-requisitos

- Backend rodando: `dotnet run` na pasta `backend/src/Garagem.Api`
- MinIO rodando: `docker-compose up` na raiz do projeto
- SQL Server rodando: `docker-compose up` na raiz do projeto
- Swagger acessível: `http://localhost:5000/swagger`

---

## 🚀 Testes via Swagger (Passo a Passo)

### **1️⃣ Registrar um novo usuário**

**Endpoint:** `POST /api/auth/register`

**Body:**
```json
{
  "firstName": "Maria",
  "lastName": "Santos",
  "email": "maria.santos@email.com",
  "password": "SenhaSegura@123"
}
```

**Response (201):**
```
No response body (HTTP 200)
```

---

### **2️⃣ Fazer Login e Obter Token**

**Endpoint:** `POST /api/auth/login`

**Body:**
```json
{
  "email": "maria.santos@email.com",
  "password": "SenhaSegura@123"
}
```

**Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI1NTBlODQwMC1lMjliLTQxZDQtYTcxNi00NDY2NTU0NDAwMDAiLCJlbWFpbCI6Im1hcmlhLnNhbnRvc0BlbWFpbC5jb20iLCJuYW1lIjoibWFyaWEuc2FudG9zQGVtYWlsLmNvbSIsIm5iZiI6MTcxNjk4NzAwMCwiZXhwIjoxNzE3NTkxODAwLCJpYXQiOjE3MTY5ODcwMDAsImlzcyI6IkdhcmFnZW1BcGkiLCJhdWQiOiJHYXJhZ2VtRnJvbnRlbmQifQ.xyz..."
}
```

**⚠️ COPIE O TOKEN COMPLETO** - Será usado nos próximos testes!

---

### **3️⃣ Obter Perfil do Usuário Autenticado**

**Endpoint:** `GET /api/users/me`

**Authorization:** 
- Clique em 🔐 (Authorize button no Swagger)
- Cole o token no campo: `Bearer {TOKEN_AQUI}`

**Response (200):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "firstName": "Maria",
  "lastName": "Santos",
  "email": "maria.santos@email.com",
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
  "createdAt": "2026-05-14T07:00:00Z",
  "updatedAt": "2026-05-14T07:00:00Z",
  "lastLoginAt": "2026-05-14T07:00:30Z"
}
```

---

### **4️⃣ Atualizar Dados do Perfil**

**Endpoint:** `PUT /api/users/profile`

**Authorization:** Bearer {TOKEN}

**Body:**
```json
{
  "firstName": "Maria",
  "lastName": "Santos",
  "bio": "Apaixonada por desenvolvimento e tecnologia",
  "phoneNumber": "(11) 99999-8888",
  "birthDate": "1995-07-22T00:00:00Z",
  "zipCode": "01310-100",
  "street": "Avenida Paulista",
  "number": "1000",
  "district": "Bela Vista",
  "city": "São Paulo",
  "state": "SP",
  "country": "Brasil"
}
```

**Response (200):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "firstName": "Maria",
  "lastName": "Santos",
  "email": "maria.santos@email.com",
  "phoneNumber": "(11) 99999-8888",
  "secondaryPhone": null,
  "bio": "Apaixonada por desenvolvimento e tecnologia",
  "profileImageUrl": null,
  "birthDate": "1995-07-22T00:00:00Z",
  "zipCode": "01310-100",
  "street": "Avenida Paulista",
  "number": "1000",
  "district": "Bela Vista",
  "city": "São Paulo",
  "state": "SP",
  "country": "Brasil",
  "createdAt": "2026-05-14T07:00:00Z",
  "updatedAt": "2026-05-14T07:02:15Z",
  "lastLoginAt": "2026-05-14T07:00:30Z"
}
```

---

### **5️⃣ Upload de Foto de Perfil**

**Endpoint:** `POST /api/users/profile-image`

**Authorization:** Bearer {TOKEN}

**Body:** (form-data)
- Key: `file`
- Value: [Selecione uma imagem do seu computador]
  - Formatos aceitos: `.jpg`, `.jpeg`, `.png`, `.webp`
  - Tamanho máximo: **5 MB**

**Response (200):**
```json
{
  "imageUrl": "http://localhost:9000/garagem-bucket/profile-images/a1b2c3d4-1716987735000000000.jpg",
  "message": "Imagem de perfil enviada com sucesso"
}
```

---

### **6️⃣ Verificar Imagem no Perfil**

**Endpoint:** `GET /api/users/me`

**Authorization:** Bearer {TOKEN}

**Response (200):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "firstName": "Maria",
  "lastName": "Santos",
  "email": "maria.santos@email.com",
  "phoneNumber": "(11) 99999-8888",
  "secondaryPhone": null,
  "bio": "Apaixonada por desenvolvimento e tecnologia",
  "profileImageUrl": "http://localhost:9000/garagem-bucket/profile-images/a1b2c3d4-1716987735000000000.jpg",
  "birthDate": "1995-07-22T00:00:00Z",
  "zipCode": "01310-100",
  "street": "Avenida Paulista",
  "number": "1000",
  "district": "Bela Vista",
  "city": "São Paulo",
  "state": "SP",
  "country": "Brasil",
  "createdAt": "2026-05-14T07:00:00Z",
  "updatedAt": "2026-05-14T07:02:45Z",
  "lastLoginAt": "2026-05-14T07:00:30Z"
}
```

✅ **Veja! A `profileImageUrl` foi atualizada!**

---

## ❌ Testes de Validação (Casos de Erro)

### **Teste 1: Fazer upload sem autenticação**

**Endpoint:** `POST /api/users/profile-image`

**Authorization:** (deixe vazio)

**Response (401):**
```
Unauthorized - Bearer token not provided or invalid
```

---

### **Teste 2: Upload de arquivo muito grande**

**Endpoint:** `POST /api/users/profile-image`

**Authorization:** Bearer {TOKEN}

**Body:** Arquivo > 5 MB

**Response (400):**
```json
{
  "message": "Arquivo muito grande. Tamanho máximo: 5MB"
}
```

---

### **Teste 3: Upload de formato inválido**

**Endpoint:** `POST /api/users/profile-image`

**Authorization:** Bearer {TOKEN}

**Body:** Arquivo `.pdf` ou `.gif`

**Response (400):**
```json
{
  "message": "Extensão de arquivo não permitida. Extensões válidas: .jpg, .jpeg, .png, .webp"
}
```

---

### **Teste 4: Atualizar perfil sem FirstName**

**Endpoint:** `PUT /api/users/profile`

**Authorization:** Bearer {TOKEN}

**Body:**
```json
{
  "firstName": "",
  "lastName": "Santos",
  "bio": "Test"
}
```

**Response (400):**
```json
{
  "message": "FirstName é obrigatório"
}
```

---

### **Teste 5: Data de nascimento no futuro**

**Endpoint:** `PUT /api/users/profile`

**Authorization:** Bearer {TOKEN}

**Body:**
```json
{
  "firstName": "Maria",
  "lastName": "Santos",
  "birthDate": "2030-01-01T00:00:00Z"
}
```

**Response (400):**
```json
{
  "message": "Data de nascimento não pode ser no futuro"
}
```

---

## 🔍 Verificar Armazenamento no MinIO

### Via CLI do MinIO:

```bash
# Entrar no container do MinIO
docker exec -it minio bash

# Listar objetos no bucket
mc ls minio/garagem-bucket/profile-images/

# Visualizar arquivo
mc cat minio/garagem-bucket/profile-images/a1b2c3d4-1716987735000000000.jpg
```

### Via Navegador:

1. Acesse: `http://localhost:9001`
2. Login: 
   - Usuário: `minioadmin`
   - Senha: `minioadmin`
3. Navegue até: `garagem-bucket` → `profile-images`
4. Veja as imagens uploadeadas

---

## 📊 Resumo dos Endpoints

| Método | Endpoint | Autenticação | Descrição |
|--------|----------|--------------|-----------|
| POST | `/api/auth/register` | ❌ | Registra novo usuário |
| POST | `/api/auth/login` | ❌ | Faz login e retorna token |
| GET | `/api/users/me` | ✅ | Obtem perfil do usuário |
| PUT | `/api/users/profile` | ✅ | Atualiza dados do perfil |
| POST | `/api/users/profile-image` | ✅ | Faz upload de foto |

---

## 🐛 Troubleshooting

### "Unauthorized" em GET /api/users/me

- ✅ Verificar se o token foi copiado corretamente
- ✅ Clicar em "Authorize" e colar novamente
- ✅ Fazer login novamente se o token expirou (7 dias)

### "MinIO connection refused"

- ✅ Verificar se `docker-compose up` foi executado
- ✅ Verificar URL em `appsettings.json`: `http://localhost:9000`
- ✅ Verificar credenciais MinIO

### "Database error"

- ✅ Verificar se SQL Server está rodando
- ✅ Verificar connection string em `appsettings.json`
- ✅ Executar migrations se necessário

---

## 🎓 Conceitos Aprendidos

- ✅ **DDD** - Domain-Driven Design com camadas bem separadas
- ✅ **JWT** - Autenticação com tokens
- ✅ **S3/MinIO** - Armazenamento de objetos
- ✅ **Clean Architecture** - Separação de responsabilidades
- ✅ **Async/Await** - Programação assíncrona
- ✅ **Validação** - Validações em múltiplas camadas
- ✅ **Dependency Injection** - Injeção de dependências

---

**Happy Testing! 🎉**
