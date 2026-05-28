## Stack
- .NET 10, ASP.NET Core Minimal APIs con MinApiLib
- Entity Framework Core con SQL Server
- CQRS, FluentValidation para validaciones
- xUnit v3 (xunit.v3 3.2.2 + xunit.runner.visualstudio 3.1.5) para tests
- React 19 + TypeScript + Vite (frontend)
- Tailwind CSS v4 (@tailwindcss/vite)
- React Router v7

## Estructura del proyecto
- src/Api          → endpoints y configuración de la app
- src/Application  → handlers, commands, queries (vertical slices)
- src/Domain       → entidades y lógica de negocio
- src/frontend     → proyecto React (Vite + TypeScript + Tailwind)
  - src/frontend/src/types/       → interfaces TypeScript
  - src/frontend/src/hooks/       → hooks de datos (fetch)
  - src/frontend/src/components/  → componentes reutilizables
  - src/frontend/src/pages/       → páginas (HomePage, CharacterPage)
- tests/           → tests unitarios e integración

## Comandos habituales
dotnet build
dotnet run --project src/Api --launch-profile http

# Frontend
cd src/frontend
npm install
npm run dev     # http://localhost:5173
npm run build

## CORS
- El backend permite peticiones desde http://localhost:5173 (configurado en Program.cs con AddCors/UseCors).
- Si se cambia el puerto del frontend, actualizar Program.cs.

## Convenciones MinApiLib
- Los parámetros del constructor del endpoint record son resueltos por DI — NO usar `[FromBody]` en el constructor.
- El binding del body se hace en el método `HandleAsync` con `[FromBody]` como parámetro.
- `Results.ValidationProblem()` devuelve 400 por defecto; pasar `statusCode: StatusCodes.Status422UnprocessableEntity` para retornar 422.

## Tests .NET (xUnit v3 + FluentAssertions + NSubstitute)
- El proyecto de tests necesita `<Using Include="Xunit" />` en el `.csproj` (xUnit v3 no añade global usings automáticamente).
- Los tests de integración usan `WebApplicationFactory<Program>` con `IClassFixture`.
- Usar `TestContext.Current.CancellationToken` en llamadas async para evitar el warning xUnit1051.
- Cobertura actual: 31 tests — unitarios (handlers, validators, repository) + integración (endpoints).

# Ejecutar tests .NET
dotnet test

## Tests Frontend (Vitest + Testing Library)
- Stack: `vitest`, `@testing-library/react`, `@testing-library/user-event`, `@testing-library/jest-dom`, `jsdom`.
- Configuración en `vite.config.ts` (environment: jsdom, globals: true, setupFiles).
- Setup global en `src/test/setup.ts` (importa `@testing-library/jest-dom`).
- Fixtures compartidos en `src/test/fixtures.ts`.
- Componentes con react-router-dom deben envolverse en `<MemoryRouter>` en los tests.
- Cobertura actual: 24 tests — SearchBar, FilterBar, CharacterCard, CharacterGrid, useCharacters hook.

# Ejecutar tests frontend
cd src/frontend
npm test                 # modo watch
npm run test:coverage    # con reporte de cobertura