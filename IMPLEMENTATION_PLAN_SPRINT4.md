# Plano de Implementação - Sprint 4: Certificados, Métricas e Refinamento

## 1. Visão Geral

A Sprint 4 é a fase final do MVP da Vulkanos Academy, focando na conclusão das funcionalidades essenciais: geração de certificados de conclusão em PDF, dashboard de métricas para produtores e administradores, e refinamento geral da plataforma.

**Período:** Semanas 7-8  
**Objetivo Principal:** Entregar uma plataforma LMS funcional e completa com todas as funcionalidades do MVP.

---

## 2. Arquitetura de Implementação

### 2.1 Backend (VulkanosAcademy.Api)

#### Novos Serviços Implementados

**CertificateService.cs**
- Interface: `ICertificateService`
- Responsabilidades:
  - Gerar certificados em PDF para matrículas concluídas
  - Armazenar referências de certificados no banco de dados
  - Recuperar certificados por ID ou por usuário
  - Validar se um certificado já existe para uma matrícula

**MetricsService.cs**
- Interface: `IMetricsService`
- Responsabilidades:
  - Calcular métricas globais da plataforma (total de usuários, cursos, matrículas, etc.)
  - Calcular métricas específicas para produtores
  - Fornecer estatísticas detalhadas por curso
  - Calcular taxa de conclusão, progresso médio e receita

#### Novos Controladores

**CertificatesController.cs**
- Endpoints:
  - `POST /api/certificates/generate` - Gerar certificado para uma matrícula
  - `GET /api/certificates/{certificateId}` - Obter certificado específico
  - `GET /api/certificates/user/{userId}` - Obter todos os certificados de um usuário
  - `GET /api/certificates/enrollment/{enrollmentId}/exists` - Verificar existência de certificado
  - `GET /api/certificates/{certificateId}/download` - Fazer download do certificado em PDF

**MetricsController.cs**
- Endpoints:
  - `GET /api/metrics/global` - Obter métricas globais (apenas admin)
  - `GET /api/metrics/producer/{producerId}` - Obter métricas do produtor
  - `GET /api/metrics/producer/{producerId}/courses` - Obter estatísticas dos cursos do produtor

#### Novos DTOs

**CertificateDto.cs**
- `CertificateDto` - Representa um certificado
- `GenerateCertificateDto` - DTO para requisição de geração de certificado
- `CertificateResponseDto` - DTO para resposta de certificado gerado
- `GlobalMetricsDto` - DTO para métricas globais

#### Modificações no Program.cs

```csharp
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<IMetricsService, MetricsService>();
```

### 2.2 Frontend (VulkanosAcademy.Web)

#### Novos Serviços

**CertificateService.cs**
- Interface: `ICertificateService`
- Métodos:
  - `GenerateCertificateAsync(Guid enrollmentId)` - Gerar certificado
  - `GetCertificateAsync(Guid certificateId)` - Obter certificado
  - `GetUserCertificatesAsync(Guid userId)` - Obter certificados do usuário
  - `CertificateExistsAsync(Guid enrollmentId)` - Verificar existência
  - `DownloadCertificateAsync(Guid certificateId, string fileName)` - Fazer download

**MetricsService.cs**
- Interface: `IMetricsService`
- Métodos:
  - `GetGlobalMetricsAsync()` - Obter métricas globais
  - `GetProducerMetricsAsync(Guid producerId)` - Obter métricas do produtor
  - `GetProducerCoursesStatsAsync(Guid producerId)` - Obter estatísticas dos cursos

#### Novas Páginas Razor

**Certificates.razor**
- Rota: `/certificates`
- Funcionalidades:
  - Listar certificados do usuário autenticado
  - Visualizar detalhes de cada certificado
  - Gerar novos certificados para cursos completos
  - Fazer download de certificados em PDF
  - Design responsivo com cards interativos

**AdminDashboard.razor**
- Rota: `/admin/dashboard`
- Funcionalidades:
  - Exibir métricas globais da plataforma
  - Mostrar total de usuários, cursos e matrículas
  - Visualizar taxa de conclusão e receita total
  - Gráficos e indicadores visuais
  - Acesso restrito a administradores

**ProducerMetrics.razor**
- Rota: `/producer/metrics`
- Funcionalidades:
  - Exibir métricas específicas do produtor
  - Mostrar desempenho por curso
  - Visualizar taxa de conclusão e receita
  - Tabela com estatísticas detalhadas dos cursos
  - Acesso restrito a produtores

---

## 3. Fluxo de Geração de Certificados

### 3.1 Processo Passo a Passo

1. **Verificação de Conclusão**
   - Sistema verifica se o aluno completou o curso (Progress >= 100%)
   - Se não completado, retorna erro

2. **Verificação de Existência**
   - Sistema verifica se já existe um certificado para a matrícula
   - Se existe, retorna o certificado existente

3. **Geração do PDF**
   - Cria um documento PDF com:
     - Nome do aluno
     - Título do curso
     - Data de conclusão
     - Número único do certificado
     - Assinatura digital
   - Salva o arquivo no diretório `wwwroot/certificates/`

4. **Armazenamento no Banco de Dados**
   - Cria registro na tabela `Certificates`
   - Armazena URL do certificado
   - Associa ao `Enrollment` correspondente

5. **Retorno ao Cliente**
   - Retorna `CertificateResponseDto` com informações do certificado
   - Cliente pode fazer download ou visualizar

### 3.2 Estrutura do Certificado PDF

O certificado contém:
- Cabeçalho com logo e nome da instituição
- Mensagem de conclusão personalizada
- Nome do aluno em destaque
- Título do curso
- Data de conclusão
- Número único do certificado
- Rodapé com data de emissão

---

## 4. Cálculo de Métricas

### 4.1 Métricas Globais

| Métrica | Cálculo | Descrição |
|---------|---------|-----------|
| Total de Usuários | COUNT(Users) | Número total de usuários registrados |
| Total de Cursos | COUNT(Courses) | Número total de cursos criados |
| Total de Matrículas | COUNT(Enrollments) | Número total de matrículas |
| Matrículas Concluídas | COUNT(Enrollments WHERE Progress >= 100) | Matrículas com 100% de progresso |
| Taxa de Conclusão Média | (Concluídas / Total) * 100 | Percentual de conclusão |
| Alunos Ativos | COUNT(DISTINCT Users WHERE Progress > 0 AND Progress < 100) | Alunos em progresso |
| Receita Total | SUM(Course.Price WHERE Enrollment EXISTS) | Receita total de matrículas |

### 4.2 Métricas por Produtor

Mesmas métricas acima, mas filtradas apenas para cursos do produtor específico.

### 4.3 Estatísticas por Curso

| Métrica | Cálculo | Descrição |
|---------|---------|-----------|
| Total de Matrículas | COUNT(Enrollments FOR Course) | Matrículas no curso |
| Matrículas Concluídas | COUNT(Enrollments WHERE Progress >= 100) | Conclusões no curso |
| Progresso Médio | AVG(Progress) | Progresso médio dos alunos |
| Taxa de Conclusão | (Concluídas / Total) * 100 | Percentual de conclusão |
| Alunos Ativos | COUNT(DISTINCT Users WHERE Progress > 0 AND Progress < 100) | Alunos em progresso |

---

## 5. Segurança e Autorização

### 5.1 Controle de Acesso

**Certificados:**
- Aluno pode visualizar apenas seus próprios certificados
- Admin pode visualizar qualquer certificado
- Produtor não tem acesso a certificados

**Métricas Globais:**
- Apenas admin pode acessar
- Requer role `Admin` no JWT

**Métricas do Produtor:**
- Produtor pode acessar suas próprias métricas
- Admin pode acessar métricas de qualquer produtor
- Validação de `userId` vs `producerId`

### 5.2 Validações

- Verificação de autenticação em todos os endpoints
- Validação de autorização baseada em roles
- Validação de propriedade de recursos (certificados, cursos)
- Tratamento de erros com mensagens apropriadas

---

## 6. Banco de Dados

### 6.1 Tabelas Utilizadas

**Certificates** (existente, sem modificações)
```
- Id (Guid)
- EnrollmentId (Guid FK)
- IssueDate (DateTime)
- CertificateUrl (string)
```

**Enrollments** (existente, sem modificações)
```
- Id (Guid)
- UserId (Guid FK)
- CourseId (Guid FK)
- Progress (decimal)
- CompletionDate (DateTime?)
- CertificateId (Guid FK)
```

Nenhuma migração é necessária, pois as tabelas já existem.

---

## 7. Testes de Integração

### 7.1 Testes de Certificados

1. **Geração de Certificado**
   - Teste: Aluno completa curso → Gera certificado
   - Esperado: Certificado criado com sucesso

2. **Validação de Conclusão**
   - Teste: Aluno tenta gerar certificado sem completar curso
   - Esperado: Erro 400 "Aluno não completou o curso"

3. **Duplicação de Certificado**
   - Teste: Aluno tenta gerar certificado duas vezes
   - Esperado: Retorna certificado existente

4. **Download de Certificado**
   - Teste: Aluno faz download de certificado
   - Esperado: Arquivo PDF retornado com sucesso

### 7.2 Testes de Métricas

1. **Métricas Globais**
   - Teste: Admin acessa métricas globais
   - Esperado: Dados corretos retornados

2. **Autorização**
   - Teste: Produtor tenta acessar métricas globais
   - Esperado: Erro 403 Forbidden

3. **Métricas do Produtor**
   - Teste: Produtor acessa suas métricas
   - Esperado: Dados dos seus cursos retornados

---

## 8. Cronograma Detalhado

### Semana 7

**Dias 1-2: Implementação Backend**
- Criar `CertificateService` e `MetricsService`
- Criar `CertificatesController` e `MetricsController`
- Criar DTOs necessários
- Registrar serviços no `Program.cs`

**Dias 3-4: Testes Backend**
- Testar geração de certificados
- Testar cálculo de métricas
- Validar autorizações
- Corrigir bugs encontrados

**Dias 5: Implementação Frontend**
- Criar `CertificateService` no Blazor
- Criar `MetricsService` no Blazor
- Criar página `Certificates.razor`

### Semana 8

**Dias 1-2: Implementação Frontend (Continuação)**
- Criar página `AdminDashboard.razor`
- Criar página `ProducerMetrics.razor`
- Integrar serviços nas páginas

**Dias 3-4: Testes Frontend**
- Testar fluxo de geração de certificados
- Testar exibição de métricas
- Validar responsividade
- Corrigir bugs de UI

**Dias 5: Refinamento e Deploy**
- Correção final de bugs
- Otimização de performance
- Deploy em ambiente de staging
- Testes de aceitação

---

## 9. Critérios de Aceitação

### 9.1 Certificados

- ✅ Aluno que completa um curso recebe um certificado em PDF
- ✅ Certificado contém informações corretas (nome, curso, data)
- ✅ Certificado pode ser baixado e visualizado
- ✅ Certificado não pode ser gerado duas vezes para mesma matrícula
- ✅ Apenas aluno ou admin pode acessar certificado

### 9.2 Métricas

- ✅ Admin pode visualizar métricas globais da plataforma
- ✅ Produtor pode visualizar métricas de seus cursos
- ✅ Métricas mostram dados corretos e atualizados
- ✅ Acesso a métricas é restrito por role
- ✅ Dashboard é responsivo e intuitivo

### 9.3 Refinamento Geral

- ✅ Interface é consistente em todas as páginas
- ✅ Navegação é intuitiva e clara
- ✅ Tratamento de erros é apropriado
- ✅ Performance é aceitável (< 2s de carregamento)
- ✅ Plataforma funciona em diferentes dispositivos

---

## 10. Próximos Passos Pós-MVP

Após a conclusão da Sprint 4 e entrega do MVP, as seguintes funcionalidades podem ser implementadas:

1. **Notificações por Email**
   - Notificar aluno quando certificado é gerado
   - Notificar produtor sobre novos alunos

2. **Relatórios Avançados**
   - Exportar métricas em Excel/PDF
   - Gráficos mais detalhados
   - Análise de tendências

3. **Gamificação**
   - Badges e achievements
   - Leaderboards
   - Pontos de experiência

4. **Sistema de Pagamento**
   - Integração com Stripe/PayPal
   - Gestão de assinaturas
   - Relatórios financeiros

5. **Aplicativos Móveis**
   - App iOS/Android nativo
   - Sincronização com backend
   - Funcionalidades offline

---

## 11. Conclusão

A Sprint 4 completa a entrega do MVP da Vulkanos Academy, fornecendo todas as funcionalidades essenciais para uma plataforma LMS funcional. Com a implementação de certificados, métricas e refinamento geral, a plataforma estará pronta para validação de conceito e feedback de usuários reais.

A arquitetura implementada é escalável e permite fácil adição de novas funcionalidades no futuro.
