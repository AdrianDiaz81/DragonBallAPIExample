<!--
SYNC IMPACT REPORT
==================
Version change: (draft) → 1.0.0
Modified principles: Initial authoring — no prior principles existed
Added sections:
  - I. Arquitectura y Estructura del Proyecto
  - II. Seguridad (Sin Autenticación ni Autorización)
  - III. Validaciones de Dominio
  - IV. Estándares de Código C#
  - V. Testing Disciplinado
  - VI. Documentación con Scalar
  - Definition of Done (DoD)
  - Governance
Removed sections: (none — first formal version)
Templates reviewed:
  - .specify/templates/plan-template.md    ✅ aligned (Constitution Check gate present)
  - .specify/templates/spec-template.md   ✅ aligned (requirements + acceptance criteria structure compatible)
  - .specify/templates/tasks-template.md  ✅ aligned (test tasks optional, story-first structure)
Follow-up TODOs: none
-->

# Dragon Ball Constitution

## Core Principles

### I. Arquitectura y Estructura del Proyecto

El proyecto se organiza en capas bien delimitadas con mínima fricción:

- **Backend**: .NET 10, ASP.NET Core Minimal APIs con MinApiLib, EF Core + SQL Server.
  - `src/Api` — endpoints y configuración de la app.
  - `src/Application` — handlers, commands, queries (Vertical Slice Architecture).
  - `src/Domain` — entidades y lógica de negocio pura; sin dependencias de infraestructura.
  - `tests/` — tests unitarios e integración (xUnit v3).
- **Frontend**: React 19 + TypeScript + Vite + Tailwind CSS v4 + React Router v7.
  - `src/frontend/src/types/` — interfaces TypeScript.
  - `src/frontend/src/hooks/` — hooks de datos.
  - `src/frontend/src/components/` — componentes reutilizables.
  - `src/frontend/src/pages/` — páginas.

**Reglas de simplicidad (NON-NEGOTIABLE)**:

- Genera la solución más simple que cumpla el requisito.
- NO uses interfaces innecesarias. Los servicios son clases concretas.
- NO apliques patrones que no sean estrictamente necesarios (Repository, Unit of Work, etc.
  solo si hay razón demostrable).
- Consolida en el menor número de archivos posible.
- El código DEBE compilar y ejecutarse en el primer intento sin configuración adicional.
- Mínimo de 3 proyectos .NET: `Api`, `Application`, `Domain`. No añadir capas sin justificar.

### II. Seguridad — Sin Autenticación ni Autorización

Este proyecto NO implementa autenticación ni autorización. Solo existe lógica de negocio.

- PROHIBIDO añadir middleware de autenticación (`UseAuthentication`, `UseAuthorization`).
- PROHIBIDO añadir atributos `[Authorize]` o políticas de acceso.
- PROHIBIDO añadir claims, tokens JWT, cookies de sesión u OAuth.
- El CORS DEBE permitir peticiones desde `http://localhost:5173` (configurado en `Program.cs`).
- Si en el futuro se requiere autenticación, se documenta como breaking change y se versiona
  la constitución en MAJOR.

**Rationale**: El dominio es de demostración (Dragon Ball). La complejidad de seguridad
añadiría ruido sin valor de negocio real.

### III. Validaciones de Dominio

Las validaciones de negocio DEBEN residir en el dominio o en los validators de la capa
Application. Nunca en el endpoint directamente.

- FluentValidation es el único mecanismo para validar comandos y queries.
- Reglas mínimas obligatorias en toda entidad:
  - Nombre: DEBE ser no nulo, no vacío y de longitud máxima 100 caracteres.
  - IDs: DEBEN ser positivos (> 0) cuando sean numéricos.
- Los errores de validación retornan HTTP 422 (Unprocessable Entity) usando
  `Results.ValidationProblem(statusCode: StatusCodes.Status422UnprocessableEntity)`.
- Las entidades de dominio en `src/Domain` DEBEN proteger sus invariantes en el constructor
  o en métodos de fábrica; nunca exponer setters públicos para campos críticos.

**Rationale**: La lógica dispersa en endpoints genera inconsistencias. Centralizar en
FluentValidation + Domain Objects garantiza que las reglas se aplican siempre, independiente
del caller.

### IV. Estándares de Código C#

- **Nombres**: descriptivos y en inglés. Sin abreviaciones ambiguas (`chrId` → `characterId`).
- **Código muerto**: PROHIBIDO. No dejar métodos, campos, using ni variables sin uso.
  El compilador DEBE emitir advertencias de código muerto tratadas como errores (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` es recomendado).
- **Async**: Todo método de I/O DEBE ser async y aceptar `CancellationToken`.
  Usar `TestContext.Current.CancellationToken` en tests (requisito xUnit v3).
- **Records**: Preferir `record` para DTOs, commands y queries inmutables.
- **Null safety**: Activar `<Nullable>enable</Nullable>` en todos los proyectos.
- **Formato**: seguir las convenciones de C# estándar (PascalCase para tipos y métodos,
  camelCase para variables locales y parámetros).
- **MinApiLib**: parámetros del constructor resueltos por DI; `[FromBody]` solo en `HandleAsync`.
- **Frontend**: TypeScript estricto (`strict: true` en `tsconfig.json`), sin `any` implícito.

**Rationale**: El código se lee más veces de las que se escribe. Nombres claros y ausencia
de código muerto reducen la carga cognitiva y los bugs silenciosos.

### V. Testing Disciplinado

- **Una prueba unitaria por cada regla de negocio crítica**. No más, no menos por defecto.
- Los tests DEBEN ser simples: un Arrange mínimo, un Act, un Assert claro.
- PROHIBIDO usar mocks complejos (> 2 niveles de setup). Preferir stubs simples o
  implementaciones en memoria.
- Stack de tests .NET: `xUnit v3` + `FluentAssertions` + `NSubstitute`.
- Stack de tests frontend: `Vitest` + `@testing-library/react` + `@testing-library/jest-dom`.
- Los tests de integración .NET usan `WebApplicationFactory<Program>` con `IClassFixture`.
- Los componentes React con `react-router-dom` se envuelven en `<MemoryRouter>` en tests.
- Fixtures compartidos en `src/frontend/src/test/fixtures.ts`.
- Cobertura mínima: toda regla de negocio en `Domain` y `Application` DEBE tener al menos
  un test que la ejercite directamente.

**Reglas críticas de negocio que requieren test unitario** (ejemplos base):
  1. Creación de personaje con nombre vacío → ValidationException.
  2. Búsqueda por ID inexistente → resultado vacío / 404.
  3. Filtrado por raza → solo devuelve personajes de esa raza.

**Rationale**: Los mocks complejos prueban la implementación, no el comportamiento.
Un test simple y directo da más confianza y se mantiene más fácilmente.

### VI. Documentación con Scalar

- Scalar DEBE estar habilitado en todos los entornos (desarrollo y producción para este
  proyecto de demostración).
- La ruta por defecto es `/scalar/v1` (o la configurada en `Program.cs`).
- Todo endpoint DEBE tener al menos:
  - Nombre descriptivo (`WithName`).
  - Tags agrupados por recurso (`WithTags`).
  - Tipos de respuesta declarados (`Produces<T>`, `ProducesProblem`).
- El `OpenApiDocument` generado DEBE ser válido (sin advertencias de schema).
- PROHIBIDO deshabilitar Scalar sin justificación documentada.

**Rationale**: Scalar proporciona documentación interactiva de cero configuración. Es el
contrato público de la API y facilita el desarrollo del frontend sin acuerdos verbales.

## Definition of Done (DoD)

Una tarea o feature se considera **Done** cuando cumple TODOS los criterios siguientes:

1. **Compila**: `dotnet build` y `npm run build` ejecutan sin errores ni advertencias.
2. **Ejecuta**: `dotnet run --project src/Api` arranca sin excepciones. El frontend
   levanta con `npm run dev` y no muestra errores en consola.
3. **Responde correctamente**: los endpoints responden con los códigos HTTP esperados
   y el contrato de datos definido (verificable desde Scalar o curl).
4. **Tests verdes**: `dotnet test` y `npm test` pasan al 100%.
5. **Sin código muerto**: no hay variables, imports ni métodos sin uso.
6. **Validaciones activas**: los errores de dominio retornan 422 con descripción clara.
7. **Scalar actualizado**: los nuevos endpoints aparecen en `/scalar/v1` con tipos correctos.

## Governance

- Esta constitución rige por encima de cualquier otra convención de equipo o herramienta.
- **Enmiendas**: cualquier cambio DEBE incrementar la versión según semver:
  - `MAJOR` — eliminación o redefinición incompatible de un principio.
  - `MINOR` — nuevo principio o sección con guía material nueva.
  - `PATCH` — clarificaciones, correcciones tipográficas, sin cambio semántico.
- Todo PR DEBE verificar el cumplimiento de los principios I–VI antes de merge.
- La complejidad añadida (nuevas capas, patrones, dependencias) DEBE justificarse en
  la sección "Complexity Tracking" del plan correspondiente.
- `LAST_AMENDED_DATE` se actualiza en cada enmienda; `RATIFICATION_DATE` no cambia.
- Archivo de guía de desarrollo en tiempo de ejecución: `CLAUDE.md` (raíz del repositorio).

**Version**: 1.0.0 | **Ratified**: 2026-06-13 | **Last Amended**: 2026-06-13
