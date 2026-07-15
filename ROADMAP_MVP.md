# Roadmap do MVP - Vulkanos Academy

## 1. Visão Geral

Este roadmap detalha o plano de ação para a entrega da primeira versão funcional (MVP) da plataforma Vulkanos Academy em um prazo de 8 semanas. O foco será nas funcionalidades essenciais para validar o conceito e proporcionar valor imediato aos usuários (Alunos, Produtores e Administradores).

## 2. Divisão em Sprints (8 Semanas)

O projeto será dividido em 4 Sprints de 2 semanas cada, seguindo uma metodologia ágil. Cada sprint terá objetivos claros e entregáveis específicos.

### Sprint 1: Configuração e Autenticação Básica (Semanas 1-2)

**Objetivos:**
*   Configurar o ambiente de desenvolvimento (.NET 10, Blazor).
*   Implementar a estrutura básica do projeto (Frontend e Backend).
*   Desenvolver o módulo de autenticação de usuários (E-mail/Senha).
*   Criar a gestão de usuários com os três níveis de acesso (Aluno, Produtor, Administrador).
*   Configurar o banco de dados inicial e migrações.

**Funcionalidades Entregues:**
*   Login e Registro de Usuários (E-mail/Senha).
*   Gerenciamento de perfis básicos.
*   Sistema de roles (Aluno, Produtor, Admin).
*   Estrutura de projeto Blazor e .NET API funcionando.

**Critérios de Aceitação:**
*   Usuários podem se registrar e fazer login com e-mail/senha.
*   Usuários podem visualizar e editar seu perfil básico.
*   Usuários são atribuídos a uma role (Aluno, Produtor, Admin) e o sistema reconhece essa role.

### Sprint 2: Gestão de Cursos e Conteúdo (Semanas 3-4)

**Objetivos:**
*   Desenvolver a funcionalidade de criação e gestão de cursos, módulos e aulas para Produtores/Administradores.
*   Implementar o upload de vídeos e materiais complementares para armazenamento de objetos.
*   Integrar o player de vídeo customizado no frontend.

**Funcionalidades Entregues:**
*   **Produtor/Admin:** Criar, editar e excluir cursos, módulos e aulas.
*   **Produtor/Admin:** Upload de vídeos para aulas.
*   **Produtor/Admin:** Upload de materiais complementares (PDF, links).
*   **Aluno:** Visualizar a lista de cursos disponíveis.
*   **Aluno:** Acessar aulas com player de vídeo.

**Critérios de Aceitação:**
*   Produtores podem criar um curso completo com módulos e aulas, incluindo vídeos e materiais.
*   Alunos podem navegar pelos cursos e assistir aos vídeos das aulas.
*   Materiais complementares são acessíveis nas aulas.

### Sprint 3: Interatividade e Progresso do Aluno (Semanas 5-6)

**Objetivos:**
*   Implementar a área de comentários/fórum por aula.
*   Desenvolver o dashboard do aluno com trilhas de aprendizado e barra de progresso.
*   Integrar autenticação social (Google, Apple).

**Funcionalidades Entregues:**
*   **Aluno:** Comentar e responder a comentários em aulas.
*   **Aluno:** Dashboard com lista de cursos inscritos e progresso individual.
*   **Aluno:** Login via Google e Apple.

**Critérios de Aceitação:**
*   Alunos podem postar comentários e ver respostas em qualquer aula.
*   O progresso do aluno é atualizado automaticamente ao assistir aulas.
*   Alunos podem fazer login usando suas contas Google ou Apple.

### Sprint 4: Certificados, Métricas e Refinamento (Semanas 7-8)

**Objetivos:**
*   Implementar a geração automática de certificados de conclusão em PDF.
*   Desenvolver o dashboard de métricas para Produtores/Administradores.
*   Realizar testes de integração e aceitação.
*   Refinamento geral da interface e experiência do usuário.

**Funcionalidades Entregues:**
*   **Aluno:** Geração e download de certificados de conclusão.
*   **Produtor/Admin:** Dashboard com métricas básicas (número de cursos, alunos inscritos, taxa de conclusão).
*   **Geral:** Correção de bugs e melhorias de usabilidade.

**Critérios de Aceitação:**
*   Alunos que completam um curso recebem um certificado em PDF.
*   Produtores/Administradores podem visualizar métricas chave da plataforma.
*   A plataforma é estável e responsiva em diferentes dispositivos.

## 3. Próximos Passos Pós-MVP

Após a entrega do MVP, as próximas iterações podem incluir:
*   Sistema de pagamento e assinaturas mais robusto.
*   Notificações por e-mail.
*   Funcionalidades avançadas de fórum.
*   Gamificação.
*   Relatórios mais detalhados.
*   Aplicativos móveis nativos (se necessário).

## 4. Considerações Finais

Este roadmap é um guia e pode ser ajustado conforme o feedback dos usuários e as prioridades de negócio. A comunicação contínua e a flexibilidade serão cruciais para o sucesso do projeto.
