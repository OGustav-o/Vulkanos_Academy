# Vulkanos Academy - Implementação do Projeto

## 📋 Status Atual

A estrutura base da plataforma Vulkanos Academy foi implementada com sucesso, seguindo a arquitetura e o roadmap definidos. O projeto está compilando sem erros e pronto para o desenvolvimento das funcionalidades.

## 🏗️ Estrutura do Projeto

### Projetos da Solução

1. **VulkanosAcademy.Domain** - Camada de Domínio
   - Entidades de negócio (User, Course, Module, Lesson, etc.)
   - DTOs (Data Transfer Objects) para comunicação entre camadas
   - Enums para tipos (UserRole, CourseStatus, MaterialType)

2. **VulkanosAcademy.Data** - Camada de Dados
   - ApplicationDbContext (Entity Framework Core)
   - Configurações de relacionamentos entre entidades
   - Suporte a SQL Server

3. **VulkanosAcademy.Api** - Backend (.NET 10)
   - Controllers: AuthController, CoursesController
   - Serviços: AuthService (autenticação com JWT)
   - Autenticação JWT integrada
   - CORS configurado para comunicação com o Frontend

4. **VulkanosAcademy.Web** - Frontend Blazor
   - Páginas: Login, Register, Dashboard
   - Serviços: AuthService, CourseService
   - Integração com Bootstrap para UI

## 🚀 Como Executar o Projeto

### Pré-requisitos

- .NET 10 SDK instalado
- SQL Server ou LocalDB
- Visual Studio 2022 ou Visual Studio Code

### Passos para Execução

#### 1. Clonar o Repositório

```bash
git clone https://github.com/OGustav-o/Vulkanos_Academy.git
cd Vulkanos_Academy
```

#### 2. Restaurar Dependências

```bash
dotnet restore
```

#### 3. Configurar o Banco de Dados

Edite o arquivo `VulkanosAcademy.Api/appsettings.json` e configure a string de conexão:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=VulkanosAcademy;Trusted_Connection=true;"
}
```

#### 4. Executar as Migrações (se necessário)

```bash
cd VulkanosAcademy.Api
dotnet ef database update
```

#### 5. Executar o Backend (API)

```bash
cd VulkanosAcademy.Api
dotnet run
```

A API estará disponível em `https://localhost:7001`

#### 6. Em outro terminal, executar o Frontend (Blazor)

```bash
cd VulkanosAcademy.Web
dotnet run
```

O Frontend estará disponível em `https://localhost:7000`

## 📚 Funcionalidades Implementadas (Sprint 1)

### Autenticação
- ✅ Login com E-mail/Senha
- ✅ Registro de novos usuários
- ✅ Geração de tokens JWT
- ✅ Três níveis de acesso (Aluno, Produtor, Admin)

### API Backend
- ✅ Controlador de Autenticação (Login, Registro)
- ✅ Controlador de Cursos (CRUD básico)
- ✅ Serviço de Autenticação com hash de senha
- ✅ Autenticação JWT integrada

### Frontend Blazor
- ✅ Página de Login
- ✅ Página de Registro
- ✅ Dashboard com lista de cursos
- ✅ Integração com API via HttpClient

## 🔄 Próximas Etapas (Sprints 2-4)

### Sprint 2: Gestão de Cursos e Conteúdo
- [ ] Implementar upload de vídeos
- [ ] Criar módulos e aulas
- [ ] Upload de materiais complementares (PDF, links)
- [ ] Player de vídeo customizado

### Sprint 3: Interatividade e Progresso
- [ ] Sistema de comentários/fórum por aula
- [ ] Rastreamento de progresso do aluno
- [ ] Autenticação social (Google, Apple)
- [ ] Dashboard do aluno com trilhas

### Sprint 4: Certificados e Métricas
- [ ] Geração automática de certificados em PDF
- [ ] Dashboard de métricas para Produtor/Admin
- [ ] Testes de integração
- [ ] Refinamento de UI/UX

## 🔐 Configurações de Segurança

### JWT Settings

Edite `VulkanosAcademy.Api/appsettings.json`:

```json
"JwtSettings": {
  "SecretKey": "your-super-secret-key-change-this-in-production-12345",
  "Issuer": "VulkanosAcademy",
  "Audience": "VulkanosAcademyUsers",
  "ExpirationHours": 24
}
```

⚠️ **IMPORTANTE**: Altere a `SecretKey` em produção!

## 📁 Estrutura de Diretórios

```
VulkanosAcademy/
├── VulkanosAcademy.Domain/
│   ├── Entities/
│   ├── DTOs/
│   └── VulkanosAcademy.Domain.csproj
├── VulkanosAcademy.Data/
│   ├── ApplicationDbContext.cs
│   └── VulkanosAcademy.Data.csproj
├── VulkanosAcademy.Api/
│   ├── Controllers/
│   ├── Services/
│   ├── Program.cs
│   ├── appsettings.json
│   └── VulkanosAcademy.Api.csproj
├── VulkanosAcademy.Web/
│   ├── Components/
│   │   ├── Pages/
│   │   └── Layout/
│   ├── Services/
│   ├── Program.cs
│   ├── appsettings.json
│   └── VulkanosAcademy.Web.csproj
├── VulkanosAcademy.sln
└── README.md
```

## 🛠️ Tecnologias Utilizadas

- **Backend**: C# .NET 10, ASP.NET Core
- **Frontend**: Blazor (Server-side)
- **Banco de Dados**: SQL Server / LocalDB
- **ORM**: Entity Framework Core 10.0
- **Autenticação**: JWT (JSON Web Tokens)
- **UI Framework**: Bootstrap 5
- **API**: RESTful com OpenAPI/Swagger

## 📝 Notas Importantes

1. O banco de dados é criado automaticamente na primeira execução (via `EnsureCreated()`)
2. A autenticação JWT está configurada com expiração de 24 horas
3. O CORS está configurado para aceitar requisições de qualquer origem (ajuste em produção)
4. Senhas são armazenadas com hash SHA256

## 🤝 Contribuindo

Para contribuir com o projeto, siga o fluxo de branches:

1. Crie uma branch para sua feature: `git checkout -b feature/sua-feature`
2. Commit suas mudanças: `git commit -am 'Add sua-feature'`
3. Push para a branch: `git push origin feature/sua-feature`
4. Abra um Pull Request

## 📞 Suporte

Para dúvidas ou problemas, abra uma issue no repositório GitHub.

---

**Desenvolvido por**: Manus AI - Engenheiro de Software e Arquiteto de Soluções Sênior  
**Data de Início**: Julho de 2026  
**Status**: Em Desenvolvimento (Sprint 1 Completo)
