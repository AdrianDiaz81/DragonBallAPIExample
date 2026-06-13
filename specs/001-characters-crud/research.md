# Research: Dragon Ball Characters CRUD API

**Feature**: 001-characters-crud
**Date**: 2026-06-13
**Status**: Complete — no NEEDS CLARIFICATION pendientes

---

## GAP Analysis: Código Existente vs. Especificación

El proyecto tiene implementadas las operaciones de lectura (GET) y creación (POST).
Las decisiones de diseño existentes son mayoritariamente correctas; se identifican
**brechas concretas** que requieren trabajo.

---

### Decision 1: Modelo de dominio `Character` — campos a añadir/corregir

**Existing state**:
```csharp
// src/Domain/Characters/Character.cs
public int Id        // ✓
public string Name   // ✓ — requerido
public string Race   // ✓ — pero el validator lo exige, spec lo hace opcional
public int PowerLevel// ✓ — pero el validator exige > 0, spec permite 0
public string Affiliation // no en spec — mantener como campo opcional de enriquecimiento
public string ImageUrl    // no en spec — mantener como campo opcional de enriquecimiento
// MISSING: LastName (string, requerido)
// MISSING: Description (string?, opcional)
```

**Decision**: Extender `Character` con `LastName` y `Description`. Mantener `Affiliation`
e `ImageUrl` como campos opcionales (aportan valor visual en el frontend y el dataset
inicial ya los tiene; eliminarlos rompería el frontend existente).

**Rationale**: La spec define el modelo mínimo obligatorio. Campos adicionales opcionales
son compatibles sin violar ningún principio de la constitución.

**Alternatives considered**:
- Eliminar `Affiliation` e `ImageUrl`: rechazado — breaking change innecesario con pérdida
  de valor visual en el frontend.
- Hacer `LastName` parte de `Name` (nombre completo): rechazado — la spec los separa
  explícitamente como campos distintos con validación independiente.

---

### Decision 2: Comportamiento de PUT — actualización parcial

**Decision**: `PUT /characters/{id}` acepta un body con todos los campos opcionales.
Solo se aplican los campos presentes en el JSON. Implementado como merge en el handler,
no como reemplazo del objeto.

**Rationale**: Clarificado en la sesión de clarificación (2026-06-13). Reduce el payload
necesario del cliente y es más ergonómico para demos desde Scalar.

**Implementation pattern**:
```csharp
// UpdateCharacterCommand con todos los campos como nullable
public record UpdateCharacterCommand(
    int Id,
    string? Name,
    string? LastName,
    string? Race,
    int? PowerLevel,
    string? Description,
    string? Affiliation,
    string? ImageUrl);
```

**Alternatives considered**:
- PUT como reemplazo total: rechazado por el usuario en la sesión de clarificación.
- PATCH separado: no necesario dado que la spec solo requiere un endpoint de actualización.

---

### Decision 3: `ICharacterRepository` — métodos faltantes

**Existing interface**: `GetAll()`, `GetById(int id)`, `Add(Character character)`

**Missing**: `Update(int id, UpdateCharacterCommand command)` → `Character?`
           `Delete(int id)` → `bool`

**Decision**: Añadir los dos métodos a la interfaz. El repositorio en memoria modifica
la lista directamente (no es necesario `lock` dado que la concurrencia no es un requisito).

**Rationale**: La interfaz es el contrato del dominio; todos los casos de uso deben estar
representados en ella.

---

### Decision 4: Validaciones — correcciones al validator existente

**Current `CreateCharacterValidator` issues**:
- `PowerLevel`: usa `GreaterThan(0)` — spec requiere `GreaterThanOrEqualTo(0)` (0 permitido).
- `Race`: marcada como requerida (`NotEmpty`) — spec la hace opcional.
- `Affiliation`: marcada como requerida — spec no la incluye como requerida.
- `LastName`: no existe — **requerido por spec**.

**Decision**: Reescribir el validator para que refleje exactamente las reglas de negocio
de la spec. El nuevo `CreateCharacterValidator` valida: `Name` (requerido, ≤100), `LastName`
(requerido, ≤100), `PowerLevel` (requerido, ≥0), `Race` (opcional, ≤100 si present),
`Description` (opcional, ≤500 si present).

---

### Decision 5: Scalar — disponibilidad en todos los entornos

**Current state**: Scalar solo disponible en `isDevelopment` (guarded by `if`).
**Spec requirement FR-010**: Scalar DEBE estar habilitado siempre.

**Decision**: Mover `MapOpenApi()` y `MapScalarApiReference()` fuera del bloque
`if (isDevelopment)`.

**Rationale**: El proyecto es de demostración; no hay producción real. La constitución
(Principio VI) lo exige explícitamente.

---

### Decision 6: Generación de ID tras eliminaciones

**Decision**: ID auto-incremental siempre creciente (`Max(c => c.Id) + 1`). No se reutilizan
IDs de personajes eliminados.

**Rationale**: Comportamiento más predecible para tests de integración; evita confusión al
llamar a `GET /characters/{id}` con un ID previamente eliminado.

**Existing code**: Ya implementado así en `InMemoryCharacterRepository.Add()`. Sin cambios.

---

### Decision 7: Frontend — componentes existentes vs. nuevos campos

**Existing `character.ts` type**:
```ts
export interface Character {
  id: number; name: string; race: string;
  powerLevel: number; affiliation: string; imageUrl: string;
}
```

**Required changes**: Añadir `lastName: string` y `description?: string` al tipo.
Actualizar `CharacterCard` y `CharacterPage` para mostrar los nuevos campos.

**Decision**: Actualización mínima — solo añadir campos al tipo y renderizarlos donde
semánticamente corresponda (lastName junto a name, description en CharacterPage).

---

### Decision 8: Tests — cobertura de las nuevas reglas

**Tests necesarios por la constitución (un test por regla crítica)**:
1. `LastName` vacío → ValidationException (nuevo)
2. `PowerLevel = 0` → válido, no lanza excepción (corrección del test existente)
3. `UpdateCharacterHandler` — solo actualiza campos enviados (nuevo)
4. `DeleteCharacterHandler` — elimina personaje existente (nuevo)
5. `DeleteCharacterHandler` — ID inexistente → resultado vacío (nuevo)

**Existing tests to fix**:
- `CreateCharacterValidatorTests`: ajustar el test de `PowerLevel` (actualmente espera
  error con 0, con la corrección debe ser válido).
