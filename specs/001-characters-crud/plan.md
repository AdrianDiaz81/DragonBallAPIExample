# Implementation Plan: Dragon Ball Characters CRUD API

**Branch**: `001-characters-crud` | **Date**: 2026-06-13
**Spec**: `specs/001-characters-crud/spec.md`
**Input**: REST API mínima CRUD para personajes de Dragon Ball sin base de datos.

---

## Summary

Completar el CRUD de personajes de Dragon Ball extendiendo la base ya implementada
(GET + POST) con las operaciones faltantes (PUT parcial + DELETE), corrigiendo el
modelo de dominio para incluir `LastName` y `Description`, ajustando las validaciones
para que coincidan exactamente con las reglas de negocio de la spec, y habilitando
Scalar en todos los entornos.

El frontend ya tiene los componentes principales; solo requiere actualizar el tipo
TypeScript con los nuevos campos y renderizarlos en `CharacterCard` / `CharacterPage`.

---

## Technical Context

**Language/Version**: C# 13 / .NET 10 + TypeScript 5 / React 19
**Primary Dependencies**: MinApiLib.Endpoints 10.0.0, FluentValidation 12.1.1,
  Scalar.AspNetCore 2.14.14, Vitest, @testing-library/react
**Storage**: `List<Character>` Singleton en DI (in-memory, sin persistencia)
**Testing**: xUnit v3 + FluentAssertions + NSubstitute (.NET) / Vitest + Testing Library (frontend)
**Target Platform**: macOS dev / cualquier plataforma .NET 10
**Project Type**: Web API + SPA (backend + frontend desacoplados)
**Performance Goals**: < 100 ms para GET /characters (colección en memoria, trivial)
**Constraints**: Sin base de datos, sin auth, sin concurrencia multi-usuario
**Scale/Scope**: Demo — un solo desarrollador, ~40 personajes iniciales

---

## Constitution Check

*Verificado antes de Phase 0. Re-verificado tras Phase 1.*

| Principio | Estado | Notas |
|-----------|--------|-------|
| I. Arquitectura — 3 proyectos, sin capas extra | ✅ | `Api`, `Application`, `Domain` + `tests/` |
| I. Sin interfaces innecesarias | ✅ | `ICharacterRepository` es el único contrato de dominio |
| II. Sin autenticación ni autorización | ✅ | Ningún middleware de auth |
| II. CORS para localhost:5173 | ✅ | Ya configurado en `Program.cs` |
| III. Validaciones en FluentValidation | ✅ | CreateCharacterValidator existe; Update a añadir |
| III. HTTP 422 para errores de validación | ✅ | Ya implementado en CreateCharacterEndpoint |
| IV. Nullable enable | ✅ | Todos los .csproj tienen `<Nullable>enable</Nullable>` |
| IV. Sin código muerto | ⚠️ | `Affiliation` requerido en validator — debe hacerse opcional |
| V. 1 test por regla crítica | ⚠️ | Faltan tests: LastName, PowerLevel=0 válido, Update, Delete |
| V. Sin mocks complejos | ✅ | Tests actuales usan stubs simples |
| VI. Scalar habilitado siempre | ⚠️ | Actualmente solo en `isDevelopment` — corregir |

**Violations (justified)**:
- Ninguna violación arquitectónica. Las 3 marcas ⚠️ son brechas de implementación
  respecto a la spec, no decisiones de complejidad injustificada.

---

## Project Structure

### Documentation (esta feature)

```text
specs/001-characters-crud/
├── plan.md              ← este archivo
├── research.md          ← análisis de brechas y decisiones
├── data-model.md        ← modelo de dominio, DTOs, validators
├── contracts/
│   └── api.md           ← contrato completo de la API
└── spec.md              ← especificación funcional
```

### Source Code — estado actual vs. objetivo

```text
src/
├── Api/
│   ├── Program.cs                             [MODIFICAR — Scalar siempre on, nuevos handlers]
│   └── Characters/
│       ├── GetCharactersEndpoint.cs           [MODIFICAR — WithName + WithTags + Produces]
│       ├── GetCharacterByIdEndpoint.cs        [MODIFICAR — WithName + WithTags + Produces]
│       ├── CreateCharacterEndpoint.cs         [MODIFICAR — LastName + Description en request]
│       ├── UpdateCharacterEndpoint.cs         [CREAR — PUT parcial]
│       └── DeleteCharacterEndpoint.cs         [CREAR — DELETE]
│
├── Application/
│   └── Characters/
│       ├── InMemoryCharacterRepository.cs     [MODIFICAR — dataset + Update + Delete]
│       ├── CreateCharacter/
│       │   ├── CreateCharacterCommand.cs      [MODIFICAR — añadir LastName, Description]
│       │   ├── CreateCharacterHandler.cs      [MODIFICAR — pasar LastName, Description]
│       │   └── CreateCharacterValidator.cs    [MODIFICAR — LastName req, PowerLevel>=0, Race opcional]
│       ├── GetCharacters/                     [sin cambios]
│       ├── GetCharacterById/                  [sin cambios]
│       ├── UpdateCharacter/                   [CREAR]
│       │   ├── UpdateCharacterCommand.cs
│       │   ├── UpdateCharacterHandler.cs
│       │   └── UpdateCharacterValidator.cs
│       └── DeleteCharacter/                   [CREAR]
│           ├── DeleteCharacterCommand.cs
│           └── DeleteCharacterHandler.cs
│
└── Domain/
    └── Characters/
        ├── Character.cs                       [MODIFICAR — LastName, Description, nullable fixes]
        └── ICharacterRepository.cs            [MODIFICAR — añadir Update y Delete]

src/frontend/src/
├── types/character.ts                         [MODIFICAR — lastName, description?]
├── components/
│   ├── CharacterCard.tsx                      [MODIFICAR — mostrar lastName]
│   └── CharacterCard.test.tsx                 [MODIFICAR — fixtures con lastName]
├── pages/
│   └── CharacterPage.tsx                      [MODIFICAR — lastName + description]
├── hooks/
│   └── useCharacters.ts                       [MODIFICAR si no tiene useCharacter(id)]
└── test/fixtures.ts                           [MODIFICAR — añadir lastName, description]

tests/DragonBall.Tests/
├── Characters/
│   ├── CreateCharacterHandlerTests.cs         [MODIFICAR — añadir LastName]
│   ├── CreateCharacterValidatorTests.cs       [MODIFICAR — PowerLevel=0 válido, LastName tests]
│   ├── GetCharactersHandlerTests.cs           [sin cambios estructurales]
│   ├── InMemoryCharacterRepositoryTests.cs    [MODIFICAR — Update + Delete]
│   ├── UpdateCharacterHandlerTests.cs         [CREAR]
│   └── DeleteCharacterHandlerTests.cs         [CREAR]
└── Integration/
    ├── CreateCharacterEndpointTests.cs        [MODIFICAR — LastName en fixtures]
    ├── GetCharactersEndpointTests.cs          [sin cambios estructurales]
    ├── UpdateCharacterEndpointTests.cs        [CREAR]
    └── DeleteCharacterEndpointTests.cs        [CREAR]
```

---

## Phase 0: Research — Completado

Ver `research.md` para el análisis completo. Resumen de decisiones clave:

1. **Modelo**: Añadir `LastName` (requerido) y `Description` (opcional) a `Character`.
   Mantener `Affiliation` e `ImageUrl` como opcionales.
2. **PUT parcial**: Implementar merge en el handler con campos nullable.
3. **Validator**: `PowerLevel >= 0`, `Race` opcional, `LastName` requerido.
4. **Scalar**: Mover fuera del bloque `isDevelopment`.
5. **IDs**: Mantener auto-incremental creciente sin reutilización.

---

## Phase 1: Implementación — Orden de ejecución

### Bloque A: Dominio (base bloqueante)

Debe completarse antes de cualquier otro bloque.

**A1** — `src/Domain/Characters/Character.cs`
- Añadir `LastName` (string, init, nullable: no)
- Añadir `Description` (string?, init)
- Hacer `Race`, `Affiliation`, `ImageUrl` nullable (`string?`)

**A2** — `src/Domain/Characters/ICharacterRepository.cs`
- Añadir: `Character? Update(int id, string? name, string? lastName, string? race, int? powerLevel, string? description, string? affiliation, string? imageUrl)`
- Añadir: `bool Delete(int id)`

### Bloque B: Application — Correcciones (paralelo con C)

**B1** — `CreateCharacterCommand.cs` + `CreateCharacterHandler.cs`
- Añadir `LastName` y `Description` al command y al handler.

**B2** — `CreateCharacterValidator.cs`
- `LastName`: `NotEmpty().MaximumLength(100)`
- `PowerLevel`: cambiar a `GreaterThanOrEqualTo(0)`
- `Race`: eliminar `NotEmpty`, dejar solo `MaximumLength(100)` condicional
- `Affiliation`: eliminar del validator

**B3** — `InMemoryCharacterRepository.cs`
- Añadir `LastName` a los 40 personajes del dataset inicial.
- Implementar `Update(...)`: buscar por ID, reconstruir `Character` con valores merged,
  reemplazar en la lista.
- Implementar `Delete(int id)`: `RemoveAll`, retornar bool.

### Bloque C: Application — Nuevos slices (paralelo con B)

**C1** — `UpdateCharacter/` (directorio nuevo)
```
UpdateCharacterCommand.cs   — record con Id + campos nullable
UpdateCharacterHandler.cs   — llama a repository.Update, lanza ValidationException si falla validación
UpdateCharacterValidator.cs — When(field is not null, () => rule)
```

**C2** — `DeleteCharacter/` (directorio nuevo)
```
DeleteCharacterCommand.cs   — record con Id (int)
DeleteCharacterHandler.cs   — llama a repository.Delete, retorna bool
```

### Bloque D: Api layer (depende de B + C)

**D1** — `UpdateCharacterEndpoint.cs` (nuevo)
```csharp
public record UpdateCharacter(int Id) : Put("/characters/{Id}")
// HandleAsync: [FromBody] UpdateCharacterRequest body
// → 200 Character / 404 / 422
```

**D2** — `DeleteCharacterEndpoint.cs` (nuevo)
```csharp
public record DeleteCharacter(int Id) : Delete("/characters/{Id}")
// HandleAsync: no body
// → 204 / 404
```

**D3** — `CreateCharacterEndpoint.cs`: actualizar `CreateCharacterRequest` con `LastName`, `Description`.

**D4** — `GetCharactersEndpoint.cs` + `GetCharacterByIdEndpoint.cs`:
añadir `WithName()`, `WithTags("Characters")`, `.Produces<>()`.

**D5** — `Program.cs`:
- Registrar `UpdateCharacterHandler` + `DeleteCharacterHandler` como `AddScoped`.
- Mover `MapOpenApi()` + `MapScalarApiReference()` fuera del bloque `isDevelopment`.

### Bloque E: Frontend (independiente de A-D, paralelo)

**E1** — `types/character.ts`: añadir `lastName: string`, `description?: string`.

**E2** — `test/fixtures.ts`: añadir `lastName` y `description` a todos los fixtures.

**E3** — `CharacterCard.tsx` + `CharacterCard.test.tsx`: mostrar `lastName` junto a `name`.

**E4** — `CharacterPage.tsx`: mostrar `lastName` + `description` en la vista de detalle.

**E5** — Verificar `useCharacters.ts`: comprobar que existe hook `useCharacter(id)` para la
página de detalle; añadir si no existe.

### Bloque F: Tests (depende de B3 para stubs en memoria)

**F1** — `CreateCharacterValidatorTests.cs`: añadir tests de `LastName`, corregir `PowerLevel=0`.

**F2** — `InMemoryCharacterRepositoryTests.cs`: añadir tests de `Update` y `Delete`.

**F3** — `UpdateCharacterHandlerTests.cs` (nuevo): 1 test por regla crítica de Update.

**F4** — `DeleteCharacterHandlerTests.cs` (nuevo): 1 test por regla crítica de Delete.

**F5** — `UpdateCharacterEndpointTests.cs` (nuevo): test de integración PUT 200 + 404 + 422.

**F6** — `DeleteCharacterEndpointTests.cs` (nuevo): test de integración DELETE 204 + 404.

---

## Complexity Tracking

> No hay violaciones de la constitución que justificar. Toda la complejidad añadida
> es directamente requerida por la especificación.

| Decisión | Justificación |
|----------|--------------|
| `Update` con 7 parámetros nullable | Requerido por PUT parcial (spec clarificada) |
| Dataset de 40 personajes en código | FR-009 exige >= 5; los 40 ya existen y aportan variedad |
| Mantener `Affiliation` + `ImageUrl` | Evita breaking change en frontend + enriquece la demo |

---

## Definition of Done (verificación final)

- [x] `dotnet build` sin errores ni warnings
- [x] `dotnet test` — todos los tests en verde (existentes + nuevos) — 57 tests
- [x] `GET /characters` devuelve los 40 personajes con `lastName` y `description`
- [x] `POST /characters` con `lastName` vacío → 422
- [x] `POST /characters` con `powerLevel: 0` → 201
- [x] `PUT /characters/1` con `{"powerLevel": 99}` → 200, solo `powerLevel` cambia
- [x] `DELETE /characters/1` → 204; `GET /characters/1` → 404
- [x] `GET /scalar/v1` muestra los 5 endpoints documentados
- [x] Frontend: lista en `/` + detalle en `/characters/1` muestran `lastName`
- [x] `npm test` — todos los tests frontend en verde — 24 tests
