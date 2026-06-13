---
description: "Backlog de tareas secuencial — Dragon Ball Characters CRUD API"
---

# Tasks: Dragon Ball Characters — CRUD API

**Feature**: 001-characters-crud
**Input**: `specs/001-characters-crud/plan.md`, `spec.md`, `data-model.md`, `contracts/api.md`
**Generado**: 2026-06-13
**Instrucción adicional**: Tareas secuenciales y completas; incluir documentación y preparación técnica.

**Tests**: Incluidos — una prueba unitaria por regla de negocio crítica (mandato constitucional).

**Organización**: Las tareas siguen el flujo Domain → Application → Api → Frontend → Tests,
garantizando que cada capa tenga su base antes de ser construida.

---

## Formato: `[ID] [P?] [Story?] Descripción`

- **[P]**: Puede ejecutarse en paralelo con otras tareas [P] del mismo bloque (distintos ficheros)
- **[Story]**: Historia de usuario a la que pertenece (US1–US4)
- Las rutas de fichero son absolutas desde la raíz del repositorio

---

## Phase 1: Setup — Verificación del estado inicial

**Propósito**: Confirmar que la base de código compila y los tests pasan antes de hacer cambios.
Establecer una línea base de referencia.

- [ ] T001 Ejecutar `dotnet build` desde la raíz y confirmar cero errores (`src/` + `tests/`)
- [ ] T002 Ejecutar `dotnet test` y anotar el número de tests en verde (línea base: 31 tests)
- [ ] T003 Ejecutar `npm test` en `src/frontend/` y anotar el número de tests en verde (línea base: 24 tests)
- [ ] T004 Verificar que `dotnet run --project src/Api` arranca y `GET /characters` devuelve 200

---

## Phase 2: Foundational — Capa de Dominio (bloqueante)

**Propósito**: Actualizar el modelo de dominio y el contrato del repositorio. Todas las capas
superiores (Application, Api, Frontend) dependen de estos cambios. NINGUNA tarea de historia
de usuario puede comenzar hasta que este phase esté completo.

**⚠️ CRÍTICO**: Completar T005–T008 antes de avanzar al Phase 3.

- [ ] T005 Modificar `src/Domain/Characters/Character.cs`:
  añadir `string LastName { get; init; }` y `string? Description { get; init; }`;
  cambiar `Race`, `Affiliation`, `ImageUrl` a `string?` (nullable)

- [ ] T006 Modificar `src/Domain/Characters/ICharacterRepository.cs`:
  añadir método `Character? Update(int id, string? name, string? lastName, string? race, int? powerLevel, string? description, string? affiliation, string? imageUrl)`;
  añadir método `bool Delete(int id)`

- [ ] T007 Modificar `src/Application/Characters/InMemoryCharacterRepository.cs` — parte 1:
  implementar el método `Update(...)` con lógica de merge parcial (solo aplica los campos no null);
  implementar el método `Delete(int id)` (RemoveAll + return bool)

- [ ] T008 Modificar `src/Application/Characters/InMemoryCharacterRepository.cs` — parte 2:
  añadir el campo `LastName` a los 40 personajes del dataset inicial siguiendo la tabla
  del `data-model.md` (p.ej. Goku→"Son", Vegeta→"Ouji", Piccolo→"Daimao", etc.)

**Checkpoint**: `dotnet build` DEBE compilar sin errores antes de continuar.

---

## Phase 3: User Story 1 — Consultar el catálogo de personajes (Priority: P1) 🎯 MVP

**Goal**: Exponer `GET /characters` y `GET /characters/{id}` correctamente documentados
con Scalar, incluyendo los nuevos campos del modelo.

**Independent Test**: `GET /characters` devuelve 200 con array de personajes que incluyen
`lastName` y `description`. `GET /scalar/v1` muestra ambos endpoints bajo el tag `Characters`.

### Implementation — User Story 1

- [ ] T009 [US1] Modificar `src/Api/Program.cs`:
  mover `app.MapOpenApi()` fuera del bloque `if (app.Environment.IsDevelopment())`;
  reemplazar `app.MapScalarApiReference()` por
  `app.MapScalarApiReference(options => options.WithEndpointPrefix("/scalar/v1"))`
  para garantizar que la ruta es exactamente `/scalar/v1` (FR-010, SC-004)

- [ ] T010 [US1] Modificar `src/Api/Characters/GetCharactersEndpoint.cs`:
  añadir `.WithName("GetCharacters")`, `.WithTags("Characters")`,
  `.Produces<IReadOnlyList<Character>>(200)` a la definición del endpoint

- [ ] T011 [US1] Modificar `src/Api/Characters/GetCharacterByIdEndpoint.cs`:
  añadir `.WithName("GetCharacterById")`, `.WithTags("Characters")`,
  `.Produces<Character>(200)`, `.ProducesProblem(404)` a la definición del endpoint

- [ ] T012 [US1] Verificar `src/Application/Characters/GetCharacters/GetCharactersHandler.cs`
  y `GetCharactersQuery.cs`: confirmar que devuelven el objeto `Character` completo
  (con `LastName` y `Description` incluidos) sin cambios adicionales

- [ ] T013 [US1] Verificar `src/Application/Characters/GetCharacterById/GetCharacterByIdHandler.cs`
  y `GetCharacterByIdQuery.cs`: confirmar que el handler devuelve `null` cuando el ID
  no existe y el endpoint responde correctamente con 404

### Tests — User Story 1

- [ ] T014 [US1] Modificar `tests/DragonBall.Tests/Integration/GetCharactersEndpointTests.cs`:
  actualizar fixtures para incluir `LastName`; verificar que la respuesta JSON
  contiene el campo `lastName` en al menos un personaje del array

- [ ] T015 [US1] Verificar `tests/DragonBall.Tests/Characters/GetCharactersHandlerTests.cs`:
  confirmar que el test pasa con el nuevo modelo de `Character` (ajustar fixtures si necesario)

**Checkpoint**: `GET /characters` devuelve 200 + `GET /scalar/v1` muestra los endpoints.

---

## Phase 4: User Story 2 — Añadir un nuevo personaje (Priority: P2)

**Goal**: `POST /characters` funciona con las nuevas reglas de negocio: `lastName` obligatorio,
`powerLevel >= 0` (0 es válido), `race` opcional.

**Independent Test**: `POST` con `lastName` vacío → 422. `POST` con `powerLevel: 0` → 201.
`POST` con datos completos → 201 con ID generado y `lastName` en la respuesta.

### Implementation — User Story 2

- [ ] T016 [US2] Modificar `src/Application/Characters/CreateCharacter/CreateCharacterCommand.cs`:
  añadir parámetro `string LastName` y `string? Description` al record;
  mantener el orden: `Name, LastName, Race, PowerLevel, Description, Affiliation, ImageUrl`

- [ ] T017 [US2] Modificar `src/Application/Characters/CreateCharacter/CreateCharacterHandler.cs`:
  actualizar la llamada a `repository.Add(...)` para pasar `LastName` y `Description`
  al construir el objeto `Character`

- [ ] T018 [US2] Reescribir `src/Application/Characters/CreateCharacter/CreateCharacterValidator.cs`
  con las nuevas reglas:
  - `Name`: `NotEmpty().MaximumLength(100)`
  - `LastName`: `NotEmpty().MaximumLength(100)` ← NUEVO, requerido
  - `PowerLevel`: `GreaterThanOrEqualTo(0)` ← CORREGIDO (era GreaterThan(0))
  - `Race`: `When(x => x.Race is not null, () => RuleFor(x => x.Race).MaximumLength(100))` ← opcional
  - `Description`: `When(x => x.Description is not null, () => RuleFor(...).MaximumLength(500))`
  - Eliminar reglas de `Affiliation` (ya no requerida)

- [ ] T019 [US2] Modificar `src/Api/Characters/CreateCharacterEndpoint.cs`:
  - Actualizar `CreateCharacterRequest` para incluir `string LastName`, `string? Description`
  - Actualizar la construcción de `CreateCharacterCommand` con los nuevos campos
  - Añadir `.WithName("CreateCharacter")`, `.WithTags("Characters")`,
    `.Produces<Character>(201)`, `.ProducesProblem(422)`, `.ProducesProblem(400)`

### Tests — User Story 2

- [ ] T020 [US2] Modificar `tests/DragonBall.Tests/Characters/CreateCharacterValidatorTests.cs`:
  - Añadir test: `LastName` vacío (`""`) → ValidationException con error en campo `LastName`
  - Añadir test: `LastName` nulo → ValidationException con error en campo `LastName`
  - Añadir test: `LastName` con solo espacios (`"   "`) → ValidationException (whitespace tratado como vacío)
  - Corregir test existente: `PowerLevel = 0` → NO lanza excepción (era esperado error, ahora válido)
  - Verificar test: `Name` vacío → ValidationException sigue fallando correctamente
  - Usar `TestContext.Current.CancellationToken` en todas las llamadas async del test

- [ ] T021 [US2] Modificar `tests/DragonBall.Tests/Characters/CreateCharacterHandlerTests.cs`:
  actualizar los fixtures de prueba para incluir `LastName`; confirmar que el handler
  devuelve un `Character` con `LastName` correctamente asignado

- [ ] T022 [US2] Modificar `tests/DragonBall.Tests/Integration/CreateCharacterEndpointTests.cs`:
  actualizar los payloads JSON de prueba para incluir `lastName`;
  añadir test de integración: POST con `lastName` vacío → HTTP 422

**Checkpoint**: `POST /characters` con `powerLevel: 0` → 201. Con `lastName: ""` → 422.

---

## Phase 5: User Story 3 — Modificar un personaje existente (Priority: P2)

**Goal**: `PUT /characters/{id}` acepta actualizaciones parciales — solo los campos enviados
se modifican; los campos omitidos conservan su valor actual.

**Independent Test**: `PUT /characters/1` con `{"powerLevel": 99}` → 200 con `powerLevel` = 99
y el resto de campos sin cambios. `PUT /characters/999` → 404.

### Implementation — User Story 3

- [ ] T023 [US3] Crear `src/Application/Characters/UpdateCharacter/UpdateCharacterCommand.cs`:
  ```csharp
  public record UpdateCharacterCommand(
      int Id,
      string? Name = null, string? LastName = null, string? Race = null,
      int? PowerLevel = null, string? Description = null,
      string? Affiliation = null, string? ImageUrl = null);
  ```

- [ ] T024 [US3] Crear `src/Application/Characters/UpdateCharacter/UpdateCharacterValidator.cs`:
  validar solo los campos que se envían (patrón `When(x => x.Field is not null, () => ...)`):
  - `Name` presente: `NotEmpty().MaximumLength(100)`
  - `LastName` presente: `NotEmpty().MaximumLength(100)`
  - `PowerLevel` presente: `GreaterThanOrEqualTo(0)`
  - `Race` presente: `MaximumLength(100)`
  - `Description` presente: `MaximumLength(500)`

- [ ] T025 [US3] Crear `src/Application/Characters/UpdateCharacter/UpdateCharacterHandler.cs`:
  inyectar `ICharacterRepository` y `IValidator<UpdateCharacterCommand>`;
  método `Handle` DEBE ser `async Task<Character?>` y aceptar `CancellationToken cancellationToken`;
  validar el command con el validator (lanzar `ValidationException` si falla);
  llamar a `repository.Update(...)` con todos los campos del command;
  devolver `null` si el ID no existe

- [ ] T026 [US3] Crear `src/Api/Characters/UpdateCharacterEndpoint.cs`:
  ```csharp
  public record UpdateCharacter(int Id) : Put("/characters/{Id}")
  ```
  - `HandleAsync` recibe `[FromBody] UpdateCharacterRequest body` y `CancellationToken`
  - Responde 200 con `Character` actualizado / 404 / 422
  - Añadir `.WithName("UpdateCharacter")`, `.WithTags("Characters")`,
    `.Produces<Character>(200)`, `.ProducesProblem(404)`, `.ProducesProblem(422)`,
    `.ProducesProblem(400)` (JSON malformado)
  - `UpdateCharacterRequest` tiene todos los campos nullable

- [ ] T027 [US3] Modificar `src/Api/Program.cs`:
  registrar `builder.Services.AddScoped<UpdateCharacterHandler>()`

### Tests — User Story 3

- [ ] T028 [US3] Crear `tests/DragonBall.Tests/Characters/UpdateCharacterHandlerTests.cs`:
  - Test 1: Actualizar solo `PowerLevel` → los demás campos no cambian
  - Test 2: `Name` enviado vacío (`""`) → ValidationException
  - Test 3: `PowerLevel` enviado negativo → ValidationException
  - Test 4: ID inexistente → handler devuelve null
  - Usar `TestContext.Current.CancellationToken` en todas las llamadas async

- [ ] T029 [US3] Crear `tests/DragonBall.Tests/Integration/UpdateCharacterEndpointTests.cs`:
  - Test integración: `PUT /characters/1` con `{"powerLevel": 150}` → HTTP 200; solo `powerLevel` cambia
  - Test integración: `PUT /characters/999` → HTTP 404
  - Test integración: `PUT /characters/1` con `{"name": ""}` → HTTP 422
  - Test integración: `PUT /characters/1` con body `{}` → HTTP 200 sin cambios (no-op válido)
  - Usar `TestContext.Current.CancellationToken` en todas las llamadas async

**Checkpoint**: `PUT /characters/1 {"powerLevel":99}` → 200 solo cambia `powerLevel`.

---

## Phase 6: User Story 4 — Eliminar un personaje (Priority: P3)

**Goal**: `DELETE /characters/{id}` elimina el personaje de la colección en memoria.
Responde 204 si existía, 404 si no existía. El catálogo completo permanece intacto
excepto el personaje eliminado.

**Independent Test**: `DELETE /characters/1` → 204. `GET /characters/1` → 404.
`GET /characters` sigue devolviendo los demás personajes.

### Implementation — User Story 4

- [ ] T030 [US4] Crear `src/Application/Characters/DeleteCharacter/DeleteCharacterCommand.cs`:
  ```csharp
  public record DeleteCharacterCommand(int Id);
  ```

- [ ] T031 [US4] Crear `src/Application/Characters/DeleteCharacter/DeleteCharacterHandler.cs`:
  inyectar `ICharacterRepository`;
  método `Handle` DEBE ser `async Task<bool>` y aceptar `CancellationToken cancellationToken`;
  llamar a `repository.Delete(command.Id)`;
  devolver `bool` (true = eliminado, false = no encontrado)

- [ ] T032 [US4] Crear `src/Api/Characters/DeleteCharacterEndpoint.cs`:
  ```csharp
  public record DeleteCharacter(int Id) : Delete("/characters/{Id}")
  ```
  - `HandleAsync` debe aceptar `CancellationToken`
  - Responde 204 (No Content) si eliminado / 404 si no existe
  - Añadir `.WithName("DeleteCharacter")`, `.WithTags("Characters")`,
    `.Produces(204)`, `.ProducesProblem(404)`, `.ProducesProblem(400)`

- [ ] T033 [US4] Modificar `src/Api/Program.cs`:
  registrar `builder.Services.AddScoped<DeleteCharacterHandler>()`

### Tests — User Story 4

- [ ] T034 [US4] Crear `tests/DragonBall.Tests/Characters/DeleteCharacterHandlerTests.cs`:
  - Test 1: Eliminar personaje existente → devuelve `true`; el repositorio ya no lo contiene
  - Test 2: Eliminar ID inexistente → devuelve `false`
  - Usar `TestContext.Current.CancellationToken` en todas las llamadas async

- [ ] T035 [US4] Modificar `tests/DragonBall.Tests/Characters/InMemoryCharacterRepositoryTests.cs`:
  - Añadir test de `Update`: actualizar solo `PowerLevel` → demás campos sin cambios
  - Añadir test de `Delete`: eliminar ID existente → `GetById` devuelve null
  - Añadir test de edge case: eliminar todos los personajes → `GetAll()` devuelve lista vacía

- [ ] T036 [US4] Crear `tests/DragonBall.Tests/Integration/DeleteCharacterEndpointTests.cs`:
  - Test integración: `DELETE /characters/{id}` → HTTP 204
  - Test integración: `DELETE /characters/999` → HTTP 404
  - Test integración: eliminar + `GET /characters/{id}` → HTTP 404
  - Test integración edge case: `GET /characters` tras eliminar todos → HTTP 200 con `[]`
  - Usar `TestContext.Current.CancellationToken` en todas las llamadas async

**Checkpoint**: Suite completa `dotnet test` en verde. Los 5 endpoints visibles en Scalar.

---

## Phase 7: Frontend — Actualización de tipos y componentes

**Goal**: El frontal React muestra `lastName` y `description` en la lista y en el detalle.
Las dos rutas (`/` y `/characters/:id`) funcionan sin errores en consola.

**Independent Test**: Abrir `http://localhost:5173` → ver lista con nombre + apellido.
Clicar en un personaje → página de detalle con todos los campos incluyendo description.

- [ ] T037 Modificar `src/frontend/src/types/character.ts`:
  añadir `lastName: string` y `description?: string` a la interfaz `Character`

- [ ] T038 Modificar `src/frontend/src/test/fixtures.ts`:
  añadir `lastName` (obligatorio) y `description` (opcional) a todos los objetos `Character`
  de prueba usados en los tests de los componentes

- [ ] T039 Modificar `src/frontend/src/components/CharacterCard.tsx`:
  mostrar `{character.lastName}` junto al nombre (p.ej. `{character.name} {character.lastName}`)
  de forma que sea legible en la tarjeta

- [ ] T040 Modificar `src/frontend/src/components/CharacterCard.test.tsx`:
  actualizar los fixtures del test para incluir `lastName`; verificar que el componente
  renderiza el apellido correctamente

- [ ] T041 Verificar si existe `src/frontend/src/hooks/useCharacter.ts` (fichero distinto a `useCharacters.ts`):
  si no existe, crearlo como nuevo fichero con un hook `useCharacter(id: number)`
  que llame a `fetch(\`${API_URL}/characters/${id}\`)` y gestione estados `loading`, `error` y `character`;
  si ya existe, verificar que el tipo de retorno incluye `lastName` y `description?`

- [ ] T042 Modificar `src/frontend/src/pages/CharacterPage.tsx`:
  mostrar `lastName` en el encabezado de la página y `description` en el cuerpo
  (si no es null); usar el hook `useCharacter(id)` para cargar el personaje individual

- [ ] T043 Ejecutar `npm test` en `src/frontend/` y confirmar que todos los tests pasan
  (sin errores de TypeScript ni fallos de render por campos faltantes)

**Checkpoint**: Frontend compila con `npm run build` sin errores de tipo TypeScript.

---

## Phase 8: Polish — Documentación y verificación final

**Propósito**: Asegurar que todos los artefactos técnicos están completos, la documentación
es correcta y el sistema cumple el Definition of Done de la constitución.

- [ ] T044 Ejecutar `dotnet build` y confirmar cero errores y cero warnings en todos los proyectos

- [ ] T045 Ejecutar `dotnet test` y confirmar que TODOS los tests pasan;
  documentar el número final de tests (≥ 40 esperado tras añadir los nuevos)

- [ ] T046 Ejecutar `npm run build` en `src/frontend/` y confirmar que compila sin errores

- [ ] T047 Ejecutar `npm test` en `src/frontend/` y confirmar que todos los tests pasan

- [ ] T048 Arrancar el backend (`dotnet run --project src/Api`) y verificar manualmente en Scalar:
  - Los 5 endpoints aparecen bajo el tag `Characters`
  - `GET /characters` devuelve el dataset con `lastName` para todos los personajes
  - `POST /characters` con `lastName` vacío → 422 con mensaje descriptivo
  - `POST /characters` con `powerLevel: 0` → 201
  - `PUT /characters/1` con `{"powerLevel": 1}` → 200, solo `powerLevel` cambia
  - `DELETE /characters/1` → 204; `GET /characters/1` → 404

- [ ] T049 Arrancar el frontend (`npm run dev`) y verificar en `http://localhost:5173`:
  - La lista muestra nombre + apellido en cada tarjeta
  - Clicar en un personaje navega a `/characters/:id` y muestra el detalle completo
  - No hay errores en la consola del navegador

- [ ] T050 Actualizar `specs/001-characters-crud/spec.md`:
  marcar **Status** como `Implemented` en la cabecera del documento

- [ ] T051 Actualizar `specs/001-characters-crud/plan.md`:
  marcar todos los ítems del checklist "Definition of Done" como completados (`- [x]`)

---

## Dependencias y orden de ejecución

### Dependencias entre phases

- **Phase 1 (Setup)**: Sin dependencias. Empezar aquí.
- **Phase 2 (Foundational)**: Depende de Phase 1. **BLOQUEA** Phase 3–7.
- **Phase 3 (US1 — Lectura)**: Depende de Phase 2. Puede empezar en paralelo con Phase 7 (frontend) si hay dos personas.
- **Phase 4 (US2 — Creación)**: Depende de Phase 2. Puede ejecutarse tras Phase 3 o en paralelo.
- **Phase 5 (US3 — Actualización)**: Depende de Phase 2. Requiere que T007 (Update en repositorio) esté completo.
- **Phase 6 (US4 — Eliminación)**: Depende de Phase 2. Requiere que T007 (Delete en repositorio) esté completo.
- **Phase 7 (Frontend)**: Depende solo de T037 (tipo TypeScript). Puede ejecutarse en paralelo con Phases 3–6.
- **Phase 8 (Polish)**: Depende de que todas las Phases anteriores estén completas.

### Oportunidades de paralelismo (equipo con 2+ personas)

```
Persona A: Phase 2 → Phase 3 → Phase 4 → Phase 5 → Phase 6
Persona B: Phase 7 (puede empezar en paralelo desde T037 una vez Phase 2 completo)
Ambas: Phase 8 (verificación conjunta)
```

---

## Implementation Strategy

### MVP (solo US1 — lectura del catálogo)

1. Completar Phase 1 (Setup)
2. Completar Phase 2 (Domain — T005, T006, T007, T008)
3. Completar Phase 3 (US1 — T009 a T015)
4. **STOP y VALIDAR**: `GET /characters` con `lastName` + Scalar funcionando
5. Si MVP aprobado → continuar con Phase 4

### Entrega incremental

1. Phase 1 + Phase 2 → Base lista
2. Phase 3 → Lectura funcional (demo posible)
3. Phase 4 → Creación funcional (CRUD parcial)
4. Phase 5 → Actualización funcional
5. Phase 6 → Eliminación funcional (CRUD completo)
6. Phase 7 → Frontend actualizado
7. Phase 8 → Verificación y cierre

---

## Notas

- `[P]` en una tarea significa que puede ejecutarse en paralelo con otras tareas `[P]`
  del mismo bloque que toquen ficheros distintos
- Cada tarea debe completarse y compilar antes de marcarla como hecha
- Ejecutar `dotnet build` tras cada bloque (A, B, C, D, E) para detectar errores temprano
- Hacer commit tras cada Phase completada para facilitar rollback si es necesario
- Total de tareas: **51** (T001–T051)
