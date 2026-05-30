# Architecture Overview

## Stack

### Backend
- .NET 10, ASP.NET Core Minimal APIs with MinApiLib
- Entity Framework Core with SQL Server
- CQRS pattern, FluentValidation
- xUnit v3 for unit and integration tests

### Frontend
- React 19 + TypeScript + Vite
- Tailwind CSS v4 (@tailwindcss/vite)
- React Router v7
- Vitest + Testing Library for tests

---

## Project Structure

```
src/
├── Api/              → Minimal API endpoints and app configuration
├── Application/      → Handlers, commands, queries (vertical slices)
├── Domain/           → Entities and business logic
└── frontend/
    ├── src/
    │   ├── types/        → TypeScript interfaces (Character, etc.)
    │   ├── hooks/        → Data hooks (useCharacters)
    │   ├── components/   → Reusable components (CharacterCard, CharacterGrid, SearchBar, FilterBar)
    │   └── pages/        → Page components (HomePage, CharacterPage)
    └── src/test/
        ├── fixtures.ts   → Shared test data
        └── setup.ts      → Global test setup (@testing-library/jest-dom)
tests/                → .NET unit and integration tests
```

---

## Guidelines

### 1. Vertical Slice Architecture (VSA)
- **Cohesion**: Check if the endpoint, request, response, and logic are in the same folder/slice.
- **Independence**: Ensure this slice doesn't reuse Request/Response models from other slices.
- **MinApiLib**: Verify the use of `PostEndpoint<T>`, `GetEndpoint<T>`, etc., and that `Configure()` sets the correct OpenAPI metadata.

### 2. C# & .NET Standards
- **Async/Await**: Verify that `CancellationToken` is passed to all asynchronous calls (DB, API, etc.).
- **Logging**: Ensure `LoggerMessageAttribute` is used for structured logging instead of simple `_logger.LogInformation()`.
- **Validation**: Check if `WithValidation()` is called in the slice configuration and a FluentValidation class exists.
- **DI**: Ensure `[RegisterInIServiceCollection]` is present on the handler record.
- **JSON serialization**: Use `ConfigureHttpJsonOptions` with `JsonNamingPolicy.CamelCase` to ensure camelCase responses compatible with the frontend.

### 3. .NET Testing
- **Integration**: Verify there is a test exercising the slice via `WebApplicationFactory<Program>`.
- **Mocks**: Use NSubstitute only for external infrastructure, not for domain logic.
- **CancellationToken**: Pass `TestContext.Current.CancellationToken` to all async calls to avoid xUnit1051 warnings.

### 4. React Best Practices
- **Component responsibility**: Each component has a single responsibility. Pages orchestrate; components render.
- **Custom hooks for data**: All API fetch logic lives in `src/hooks/`. Components never call `fetch` directly.
- **Fetch cleanup**: Use `AbortController` in `useEffect` hooks that perform fetch calls to prevent state updates on unmounted components:
  ```ts
  useEffect(() => {
    const controller = new AbortController();
    fetch(url, { signal: controller.signal })...
    return () => controller.abort();
  }, []);
  ```
- **Environment variables**: API base URLs must use `import.meta.env.VITE_API_URL` instead of hardcoded strings.
- **Memoization**: Use `useMemo` for derived data (filtered lists, unique sets) to avoid unnecessary recalculations on every render.
- **Router**: Navigation uses `useNavigate` from React Router v7. Never use `window.location`.
- **No duplicate constants**: Shared constants (e.g. `AFFILIATION_COLORS`) must be extracted to `src/utils/` and imported where needed.
- **Accessibility**: Interactive elements that are not `<button>` or `<a>` must have `role`, `tabIndex`, and keyboard event handlers.

### 5. Frontend Testing
- **Fixtures**: Shared test data lives in `src/test/fixtures.ts`. Never repeat inline mock objects across test files.
- **Router wrapping**: Components that use `useNavigate` or `<Link>` must be wrapped in `<MemoryRouter>` in tests.
- **Fetch mocking**: Mock `global.fetch` with `vi.spyOn` in hook tests; restore with `vi.restoreAllMocks()` in `afterEach`.
- **Coverage target**: All components, hooks, and pages must have test coverage.

### 6. CORS
- The backend allows requests from `http://localhost:5173` (configured in `Program.cs` with `AddCors`/`UseCors`).
- If the frontend port changes, update `Program.cs` accordingly.
