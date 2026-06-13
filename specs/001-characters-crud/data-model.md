# Data Model: Dragon Ball Characters CRUD

**Feature**: 001-characters-crud
**Date**: 2026-06-13

---

## Entidad `Character` (dominio)

Ubicación: `src/Domain/Characters/Character.cs`

```csharp
public sealed class Character
{
    public int Id          { get; init; }   // Auto-incremental, > 0
    public string Name     { get; init; }   // Obligatorio, ≤ 100 chars
    public string LastName { get; init; }   // Obligatorio, ≤ 100 chars  [NUEVO]
    public string? Race    { get; init; }   // Opcional, ≤ 100 chars
    public int PowerLevel  { get; init; }   // Obligatorio, ≥ 0
    public string? Description { get; init; } // Opcional, ≤ 500 chars  [NUEVO]
    public string? Affiliation { get; init; } // Opcional (enriquecimiento dataset)
    public string? ImageUrl    { get; init; } // Opcional (enriquecimiento dataset)
}
```

**Invariantes de negocio** (validadas en Application, no en Domain):
- `Name` no puede ser nulo, vacío, ni solo whitespace.
- `LastName` no puede ser nulo, vacío, ni solo whitespace.
- `PowerLevel` ≥ 0 (un nivel de fuerza de 0 es válido — personaje sin poder conocido).
- `Race`, `Description`, `Affiliation`, `ImageUrl`: opcionales — si se envían, respetan
  longitud máxima.

---

## Interfaz `ICharacterRepository`

Ubicación: `src/Domain/Characters/ICharacterRepository.cs`

```csharp
public interface ICharacterRepository
{
    IReadOnlyList<Character> GetAll();
    Character? GetById(int id);
    Character Add(Character character);
    Character? Update(int id, Action<CharacterPatch> applyPatch); // NUEVO
    bool Delete(int id);                                          // NUEVO
}
```

**Nota**: `CharacterPatch` es una clase mutable que representa los campos opcionales
de actualización, permitiendo el patrón de merge parcial sin mutación directa de `Character`.

Alternativa más simple (también válida sin `CharacterPatch`):

```csharp
Character? Update(int id, string? name, string? lastName, string? race,
                  int? powerLevel, string? description, string? affiliation,
                  string? imageUrl);
```

**Decision**: Usar la firma con parámetros explícitos (más simple, sin clase extra).

---

## Comandos y Queries (Application layer)

### Existentes (sin cambios estructurales)

```
GetCharactersQuery           → GetCharactersHandler → IReadOnlyList<Character>
GetCharacterByIdQuery(int id) → GetCharacterByIdHandler → Character?
CreateCharacterCommand(...)  → CreateCharacterHandler → Character
```

### Nuevos

```csharp
// UpdateCharacterCommand — todos opcionales excepto Id
public record UpdateCharacterCommand(
    int Id,
    string? Name        = null,
    string? LastName    = null,
    string? Race        = null,
    int?    PowerLevel  = null,
    string? Description = null,
    string? Affiliation = null,
    string? ImageUrl    = null);

// UpdateCharacterHandler → Character? (null si ID no existe)

// DeleteCharacterCommand
public record DeleteCharacterCommand(int Id);

// DeleteCharacterHandler → bool (false si ID no existe)
```

---

## Validators (Application layer)

### `CreateCharacterValidator` — CORRECCIONES

```csharp
RuleFor(x => x.Name)
    .NotEmpty()
    .MaximumLength(100);

RuleFor(x => x.LastName)              // NUEVO
    .NotEmpty()
    .MaximumLength(100);

RuleFor(x => x.PowerLevel)
    .GreaterThanOrEqualTo(0);         // CORREGIDO: era GreaterThan(0)

// Race: ya no es obligatoria
When(x => x.Race is not null, () =>
    RuleFor(x => x.Race).MaximumLength(100));

// Description: opcional
When(x => x.Description is not null, () =>
    RuleFor(x => x.Description).MaximumLength(500));

// Affiliation + ImageUrl: se eliminan del validator (opcionales sin regla)
```

### `UpdateCharacterValidator` — NUEVO

```csharp
// Solo valida los campos que se envían (no null)
When(x => x.Name is not null, () =>
    RuleFor(x => x.Name).NotEmpty().MaximumLength(100));

When(x => x.LastName is not null, () =>
    RuleFor(x => x.LastName).NotEmpty().MaximumLength(100));

When(x => x.PowerLevel.HasValue, () =>
    RuleFor(x => x.PowerLevel!.Value).GreaterThanOrEqualTo(0));

When(x => x.Race is not null, () =>
    RuleFor(x => x.Race).MaximumLength(100));

When(x => x.Description is not null, () =>
    RuleFor(x => x.Description).MaximumLength(500));
```

---

## Request / Response DTOs (Api layer)

### `CreateCharacterRequest`
```csharp
public sealed record CreateCharacterRequest(
    string Name,
    string LastName,
    string? Race,
    int PowerLevel,
    string? Description,
    string? Affiliation,
    string? ImageUrl);
```

### `UpdateCharacterRequest`
```csharp
// Todos nullable — solo se envían los campos a modificar
public sealed record UpdateCharacterRequest(
    string? Name        = null,
    string? LastName    = null,
    string? Race        = null,
    int?    PowerLevel  = null,
    string? Description = null,
    string? Affiliation = null,
    string? ImageUrl    = null);
```

---

## TypeScript Interface (Frontend)

Ubicación: `src/frontend/src/types/character.ts`

```ts
export interface Character {
  id: number;
  name: string;
  lastName: string;       // NUEVO
  race: string;
  powerLevel: number;
  description?: string;   // NUEVO
  affiliation: string;
  imageUrl: string;
}
```

---

## Dataset Inicial (carga en memoria)

Ubicación: `src/Application/Characters/InMemoryCharacterRepository.cs`

El dataset existente tiene 40 personajes con `Name`, `Race`, `PowerLevel`, `Affiliation`,
`ImageUrl`. Tras el cambio del modelo, se debe añadir `LastName` a cada entrada.

**Regla para personajes de un solo nombre** (ej. Beerus, Whis, Zeno, Cell, Broly):
Usar su raza o título como `LastName` simbólico. Si no aplica, repetir el nombre
(p.ej. `Name = "Beerus"`, `LastName = "Hakaishin"`). El campo DEBE ser no vacío.

**Regla general**: Si el nombre original tiene dos palabras, la primera va a `Name`
y la segunda a `LastName`. Si tiene una sola palabra, ver tabla abajo.

**Tabla completa del dataset inicial (40 personajes)**:

| ID | Name actual | → Name | LastName |
|----|-------------|--------|----------|
| 1 | "Goku" | "Goku" | "Son" |
| 2 | "Vegeta" | "Vegeta" | "Ouji" |
| 3 | "Piccolo" | "Piccolo" | "Daimao" |
| 4 | "Bulma" | "Bulma" | "Brief" |
| 5 | "Frieza" | "Frieza" | "Icejin" |
| 6 | "Zarbon" | "Zarbon" | "Zarbon" |
| 7 | "Dodoria" | "Dodoria" | "Dodoria" |
| 8 | "Ginyu" | "Ginyu" | "Captain" |
| 9 | "Cell" | "Cell" | "Bio-Android" |
| 10 | "Gohan" | "Gohan" | "Son" |
| 11 | "Krillin" | "Krillin" | "Krillin" |
| 12 | "Tenshinhan" | "Tenshinhan" | "Tenshinhan" |
| 13 | "Yamcha" | "Yamcha" | "Yamcha" |
| 14 | "Chi-Chi" | "Chi" | "Chi" |
| 15 | "Gotenks" | "Gotenks" | "Gotenks" |
| 16 | "Trunks" | "Trunks" | "Briefs" |
| 17 | "Master Roshi" | "Muten" | "Roshi" |
| 18 | "Bardock" | "Bardock" | "Bardock" |
| 19 | "Launch" | "Launch" | "Launch" |
| 20 | "Mr. Satan" | "Hercule" | "Satan" |
| 21 | "Dende" | "Dende" | "Guardian" |
| 22 | "Android 17" | "Android" | "17" |
| 23 | "Android 16" | "Android" | "16" |
| 24 | "Android 19" | "Android" | "19" |
| 25 | "Android 13" | "Android" | "13" |
| 26 | "Android 14" | "Android" | "14" |
| 27 | "Android 15" | "Android" | "15" |
| 28 | "Nail" | "Nail" | "Namekian" |
| 29 | "Raditz" | "Raditz" | "Son" |
| 30 | "Babidi" | "Babidi" | "Wizard" |
| 31 | "Majin Buu" | "Majin" | "Buu" |
| 32 | "Beerus" | "Beerus" | "Hakaishin" |
| 33 | "Whis" | "Whis" | "Angel" |
| 34 | "Zeno" | "Zeno" | "Omni-King" |
| 35 | "Kibito-Shin" | "Kibito" | "Shin" |
| 36 | "Jiren" | "Jiren" | "Pride Trooper" |
| 37 | "Toppo" | "Toppo" | "Pride Trooper" |
| 38 | "Dyspo" | "Dyspo" | "Pride Trooper" |
| 39 | "Broly" | "Broly" | "Legendary" |
| 40 | "Gogeta" | "Gogeta" | "Fusion" |
