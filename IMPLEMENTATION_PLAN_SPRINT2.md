# Plano de Implementação - Sprint 2: Gestão de Cursos e Conteúdo

## 1. Status Atual do Projeto

O projeto Vulkanos Academy foi clonado e analisado. A Sprint 1, focada em Configuração e Autenticação Básica, foi concluída com sucesso. A estrutura do projeto em .NET 10 e Blazor está estabelecida, e as funcionalidades de autenticação (login/registro com e-mail/senha) e gerenciamento de usuários com roles (Aluno, Produtor, Administrador) estão implementadas. O banco de dados inicial com as entidades principais também está configurado.

**Componentes Implementados na Sprint 1:**

*   **Backend (.NET 10 API):**
    *   `AuthController.cs`: Login, Registro, Geração de JWT.
    *   `CoursesController.cs`: CRUD básico para Cursos (Get, GetById, Create, Update, Delete).
    *   `LessonMaterialsController.cs`, `LessonsController.cs`, `ModulesController.cs`: Controladores existentes, mas com funcionalidades ainda a serem expandidas.
    *   `ApplicationDbContext.cs`: Configuração das entidades e relacionamentos.
*   **Frontend (Blazor WebAssembly):**
    *   Páginas: `Login.razor`, `Register.razor`, `Dashboard.razor`, `ProducerDashboard.razor`.
    *   Serviços: `AuthService.cs`, `CourseService.cs` (para buscar cursos).

## 2. Objetivos da Sprint 2

Esta sprint se concentrará na implementação das funcionalidades de gestão de cursos e conteúdo, conforme definido no `ROADMAP_MVP.md`.

**Objetivos Principais:**

*   **Produtor/Admin:** Implementar CRUD completo para Cursos, Módulos e Aulas.
*   **Produtor/Admin:** Habilitar upload de vídeos para aulas.
*   **Produtor/Admin:** Habilitar upload de materiais complementares (PDF, links) para aulas.
*   **Aluno:** Integrar um player de vídeo customizado e responsivo.
*   **Aluno:** Permitir acesso às aulas com o player de vídeo e materiais complementares.

## 3. Plano de Ação Detalhado para a Sprint 2

### 3.1. Backend (VulkanosAcademy.Api)

#### 3.1.1. Expansão dos Controladores Existentes

*   **`ModulesController.cs`:**
    *   Implementar endpoints para CRUD completo de Módulos (Create, Read, Update, Delete), associados a um `CourseId`.
    *   Garantir que apenas Produtores/Administradores possam gerenciar módulos.
*   **`LessonsController.cs`:**
    *   Implementar endpoints para CRUD completo de Aulas (Create, Read, Update, Delete), associadas a um `ModuleId`.
    *   Adicionar lógica para upload de vídeo (URL) e materiais complementares (URL/links).
    *   Garantir que apenas Produtores/Administradores possam gerenciar aulas.
*   **`LessonMaterialsController.cs`:**
    *   Implementar endpoints para CRUD completo de Materiais de Aula (Create, Read, Update, Delete), associados a um `LessonId`.
    *   Adicionar suporte para diferentes tipos de materiais (PDF, Link).
    *   Garantir que apenas Produtores/Administradores possam gerenciar materiais.

#### 3.1.2. Implementação de Upload de Arquivos (Simulado)

*   Para o MVP, o upload de arquivos (vídeos e PDFs) será simulado armazenando apenas as URLs dos recursos. Em futuras sprints, pode-se integrar com serviços de armazenamento de objetos como Azure Blob Storage ou AWS S3.
*   Modificar DTOs (`CreateLessonDto`, `UpdateLessonDto`, `CreateLessonMaterialDto`, `UpdateLessonMaterialDto`) para incluir campos para URLs de vídeo e material.

### 3.2. Frontend (VulkanosAcademy.Web)

#### 3.2.1. Páginas de Gestão para Produtor/Admin

*   **`ProducerDashboard.razor`:**
    *   Exibir lista de cursos criados pelo produtor.
    *   Adicionar botões para 
criar, editar e excluir cursos.
    *   Navegação para páginas de gestão de módulos e aulas de um curso específico.
*   **Nova Página: `CourseManagement.razor`:**
    *   Detalhes do curso selecionado.
    *   Listagem de módulos do curso com opções de CRUD.
    *   Navegação para páginas de gestão de aulas de um módulo específico.
*   **Nova Página: `ModuleManagement.razor`:**
    *   Detalhes do módulo selecionado.
    *   Listagem de aulas do módulo com opções de CRUD.
    *   Formulário para adicionar/editar aulas, incluindo campos para URL do vídeo e upload de materiais.

#### 3.2.2. Páginas de Visualização para Aluno

*   **Nova Página: `CourseDetails.razor`:**
    *   Exibir detalhes do curso, lista de módulos e aulas.
    *   Botão para iniciar/continuar o curso.
*   **Nova Página: `LessonPlayer.razor`:**
    *   Integrar um player de vídeo (ex: `<video>` HTML5 ou componente Blazor de terceiros).
    *   Exibir o vídeo da aula.
    *   Listar e permitir download/visualização de materiais complementares.
    *   Navegação entre aulas.

#### 3.2.3. Serviços Frontend

*   **`CourseService.cs`:**
    *   Adicionar métodos para `CreateCourse`, `UpdateCourse`, `DeleteCourse`.
    *   Adicionar métodos para buscar cursos por instrutor.
*   **Novo Serviço: `ModuleService.cs`:**
    *   Métodos para CRUD de Módulos.
*   **Novo Serviço: `LessonService.cs`:**
    *   Métodos para CRUD de Aulas.
*   **Novo Serviço: `MaterialService.cs`:**
    *   Métodos para CRUD de Materiais de Aula.

## 4. Modelagem de Banco de Dados (Revisão)

As entidades existentes (`Course`, `Module`, `Lesson`, `LessonMaterial`) já suportam as funcionalidades da Sprint 2. Nenhuma alteração significativa na estrutura do banco de dados é esperada, mas a configuração do Entity Framework Core será revisada para garantir que todos os relacionamentos e propriedades estejam corretamente mapeados.

## 5. Testes e Validação

*   Testes unitários para os novos endpoints da API.
*   Testes de integração para a comunicação entre Frontend e Backend.
*   Testes manuais das funcionalidades de gestão de cursos e visualização de aulas.

## 6. Próximos Passos

Após a conclusão da Sprint 2, o foco será na Sprint 3, que abordará a interatividade (comentários/fórum), progresso do aluno e autenticação social (Google, Apple).
