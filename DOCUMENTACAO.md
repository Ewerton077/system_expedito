# Documentação — Sto Expedito

## Bugs Críticos

### 1. Login como "root" resulta em 404
`LoginController` redireciona root para `AdminController`, que **não existe**.  
Login como root → tela branca / 404.  
**Correção:** Criar `AdminController` com action `Index`, ou redirecionar root para `Dashboard/Index` até o admin estar pronto.

### 2. `EnsureDeleted()` destrói banco toda inicialização
Em `Program.cs`:
```csharp
context.Database.EnsureDeleted();
context.Database.Migrate();
```
Qualquer dado real é perdido ao reiniciar a aplicação.  
**Correção:** Remover `EnsureDeleted()`. Usar `Migrate()` apenas.

---

## Problemas de Segurança

### 3. Senhas hardcoded no código-fonte
```csharp
Username = "root",
PasswordHash = BCryptHelper.HashPassword("krakermanfodase")
```
Senhas em texto puro no `Program.cs`. Vazar o repo expõe credenciais.  
**Correção:** Mover para variáveis de ambiente ou `appsettings.json` criptografado.

### 4. Classe `BCryptHelper` usa SHA256, não bcrypt
Nome enganoso. SHA256 é rápido demais para hash de senhas (força bruta).  
**Futuro:** Migrar para bcrypt (NuGet: `BCrypt.Net-Next`) quando possível.

### 5. Sem autorização por role
`[Authorize]` sem `[Authorize(Roles = "Admin")]` → qualquer usuário logado acessa qualquer controller.  
**Futuro:** Adicionar políticas de autorização e decorar controllers sensíveis.

### 6. AdminUser sem coluna de role/tipo
A distinção root vs evin é feita por `username == "root"` — frágil.  
**Futuro:** Adicionar coluna `Tipo` (enum: `Root`, `Admin`) no `AdminUser`.

---

## Problemas de Arquitetura

### 7. ViewBag em vez de ViewModels
Todos os controllers usam `ViewBag` para passar dados. Sem type safety, quebra silenciosa.  
**Futuro:** Migrar para ViewModels tipados.

### 8. Sem áreas (Areas)
Admin e sistema principal misturados no mesmo namespace.  
**Futuro:** Criar área `Admin` com controllers/views separadas.

### 9. Sidebar estática
Avatar "AD" e label "Administrador" fixos no HTML. Não refletem usuário logado.  
**Futuro:** Renderizar sidebar com base no `User.Identity` e claims.

### 10. DashboardController sem distinção root vs evin
Root e evin veem os mesmos dados. Admin tech deveria mostrar mais info.  
**Correção:** Verificar claim `UsuarioTipo` e passar dados extras para root.

---

## Problemas Futuros

| Problema | Risco |
|----------|-------|
| Sem migração separada dev/prod | `EnsureDeleted()` pode ir parar em produção |
| Conexão SQL fixa no `appsettings.json` | String de conexão sensível no repo |
| Sem testes automatizados | Qualquer refactor pode quebrar sem warning |
| Views sem tratamento de erro | Se ViewBag estiver nulo, exceção em runtime |
| JS inline no `Cadastro/Index.cshtml` | Manutenção difícil, sem minificação |
| Chart.js carregado de CDN | Sem fallback se sem internet |
| Nomes de migration inconsistentes | `asdfds`, sem padrão semântico |

---

## Status Atual (26/06/2026)

- [x] CEP no cadastro de clientes
- [x] Mini calendário popup para datas médicas
- [x] Remoção de "Horário dos Banhos" e "Dias da Semana"
- [x] Mock data (300 clientes × 3 pets)
- [x] Monitoramento DB no Dashboard
- [x] Seed "root" e "evin" no banco
- [ ] **AdminController** — não criado (BUG: 404 ao logar como root)
- [ ] **Views/Admin/Index.cshtml** — não criada
- [ ] **wwwroot/css/admin.css** — não criado
- [ ] Sidebar com link Admin para root
- [ ] Sidebar dinâmica por role
