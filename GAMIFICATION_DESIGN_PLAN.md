# Plano de Gamificação e Design - Sprint 5

## 1. Visão Geral da Gamificação

A gamificação visa aumentar o engajamento, a retenção e a motivação dos alunos através da aplicação de mecânicas de jogos no ambiente de aprendizado.

### 1.1 Mecânicas Principais

1. **Pontos de Experiência (XP)**
   - Concluir uma aula: +10 XP
   - Concluir um módulo: +50 XP
   - Concluir um curso: +200 XP
   - Fazer um comentário/pergunta: +5 XP
   - Receber um "curtir" em um comentário: +2 XP

2. **Níveis (Levels)**
   - Nível 1 (Iniciante): 0 - 100 XP
   - Nível 2 (Aprendiz): 101 - 300 XP
   - Nível 3 (Estudioso): 301 - 600 XP
   - Nível 4 (Especialista): 601 - 1000 XP
   - Nível 5 (Mestre): 1001+ XP

3. **Medalhas (Badges)**
   - *Primeiros Passos*: Concluir a primeira aula
   - *Maratonista*: Concluir 5 aulas no mesmo dia
   - *Comunicativo*: Fazer 10 comentários
   - *Graduado*: Concluir o primeiro curso
   - *Colecionador*: Obter 3 certificados

4. **Ranking (Leaderboard)**
   - Ranking global de alunos por XP
   - Ranking mensal para incentivar engajamento contínuo

## 2. Arquitetura de Banco de Dados (Novas Entidades)

### 2.1 UserProfile (Extensão de User)
- `UserId` (Guid, PK, FK)
- `TotalXP` (int)
- `CurrentLevel` (int)
- `Title` (string) - ex: "Iniciante", "Mestre"

### 2.2 GamificationEvent
- `Id` (Guid, PK)
- `UserId` (Guid, FK)
- `EventType` (Enum: LessonCompleted, CourseCompleted, CommentPosted, etc.)
- `XPGranted` (int)
- `CreatedAt` (DateTime)
- `ReferenceId` (Guid) - ID da aula, curso ou comentário

### 2.3 Badge
- `Id` (Guid, PK)
- `Name` (string)
- `Description` (string)
- `IconUrl` (string)
- `RequiredXP` (int?)
- `BadgeType` (Enum)

### 2.4 UserBadge
- `Id` (Guid, PK)
- `UserId` (Guid, FK)
- `BadgeId` (Guid, FK)
- `EarnedAt` (DateTime)

## 3. Guia de Estilo Visual (UI/UX)

### 3.1 Paleta de Cores Atualizada
- **Primária**: `#FF6600` (Laranja Vulkanos - Energia, Ação)
- **Secundária**: `#4A90E2` (Azul Tecnológico - Confiança, Foco)
- **Acento/Gamificação**: `#FFD700` (Dourado - Conquistas, Medalhas)
- **Sucesso**: `#28A745` (Verde - Conclusão, Progresso)
- **Fundo**: `#F8F9FA` (Cinza muito claro - Leitura confortável)
- **Superfícies**: `#FFFFFF` (Branco - Cards, Modais)
- **Texto Principal**: `#212529` (Cinza escuro - Contraste ideal)

### 3.2 Tipografia
- **Títulos (Headings)**: 'Poppins', sans-serif (Moderno, geométrico)
- **Corpo (Body)**: 'Inter', sans-serif (Legibilidade, clareza)

### 3.3 Elementos de Interface
- **Cards**: Bordas arredondadas (12px), sombras suaves (`box-shadow: 0 4px 12px rgba(0,0,0,0.05)`), transições suaves no hover.
- **Botões**: Arredondados, gradientes sutis para ações primárias, feedback tátil (scale down no clique).
- **Barras de Progresso**: Mais espessas (8-10px), cantos arredondados, gradiente animado.
- **Badges**: Ícones vetoriais com brilho sutil, animação de "pop" ao serem conquistados.

### 3.4 Animações
- Transições de página suaves (Fade in/Slide up)
- Feedback visual imediato ao ganhar XP (Pequeno popup "+10 XP" flutuando)
- Confete ou animação de celebração ao concluir um curso ou ganhar uma medalha.

## 4. Plano de Implementação

### Fase 1: Backend (Gamificação)
1. Criar as novas entidades no `VulkanosAcademy.Domain`.
2. Atualizar o `ApplicationDbContext` com as novas tabelas.
3. Criar o `GamificationService` para processar eventos e conceder XP/Badges.
4. Criar o `GamificationController` para expor os dados (Leaderboard, Perfil do Usuário).
5. Integrar o `GamificationService` nos serviços existentes (ex: ao concluir aula, chamar o serviço de gamificação).

### Fase 2: Frontend (Design e UI)
1. Atualizar o `app.css` com as novas variáveis de cor e tipografia.
2. Criar componentes visuais reutilizáveis (XP Badge, Level Progress Bar).
3. Refatorar o layout principal (`MainLayout.razor`) para incluir o nível e XP do usuário no header.
4. Criar a página de Perfil do Aluno (`UserProfile.razor`) com suas medalhas e histórico.
5. Criar a página de Ranking (`Leaderboard.razor`).
6. Adicionar animações CSS para feedback de gamificação.
