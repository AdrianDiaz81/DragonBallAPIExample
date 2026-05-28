# Dragon Ball API

Proyecto fullstack para gestionar y visualizar personajes de Dragon Ball. Incluye una API REST en .NET 10 y un frontend en React que muestra los personajes con imágenes al estilo de [dragonball-api.com](https://web.dragonball-api.com/).

## Stack

### Backend
- **.NET 10** — ASP.NET Core Minimal APIs con [MinApiLib](https://github.com/fernandoescolar/MinApiLib)
- **CQRS** — handlers separados por comando/query (vertical slices)
- **FluentValidation** — validaciones en la capa de aplicación
- **Scalar** — documentación OpenAPI en desarrollo
- **xUnit v3 + FluentAssertions + NSubstitute** — tests unitarios e integración

### Frontend
- **React 19 + TypeScript + Vite**
- **Tailwind CSS v4**
- **React Router v7**
- **Vitest + Testing Library** — tests de componentes y hooks

---

## Estructura del proyecto

```
dragonball/
├── src/
│   ├── Api/                        # Endpoints y configuración de la app
│   │   └── Characters/
│   │       ├── GetCharactersEndpoint.cs
│   │       └── CreateCharacterEndpoint.cs
│   ├── Application/                # Handlers, commands, queries
│   │   └── Characters/
│   │       ├── GetCharacters/
│   │       ├── CreateCharacter/
│   │       └── InMemoryCharacterRepository.cs
│   ├── Domain/                     # Entidades y contratos
│   │   └── Characters/
│   │       ├── Character.cs
│   │       └── ICharacterRepository.cs
│   └── frontend/                   # Proyecto React
│       └── src/
│           ├── components/         # CharacterCard, CharacterGrid, SearchBar, FilterBar
│           ├── hooks/              # useCharacters
│           ├── pages/              # HomePage, CharacterPage
│           ├── types/              # Interfaces TypeScript
│           └── test/               # Fixtures y setup de tests
└── tests/
    └── DragonBall.Tests/           # Tests unitarios e integración .NET
        ├── Characters/             # Handler, validator, repository tests
        └── Integration/            # Endpoint tests con WebApplicationFactory
```

---

## Puesta en marcha

### Requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)

### 1. Arrancar el backend

```bash
dotnet run --project src/Api --launch-profile http
```

La API queda disponible en `http://localhost:5263`.
La documentación OpenAPI (Scalar) en `http://localhost:5263/scalar/v1`.

### 2. Arrancar el frontend

```bash
cd src/frontend
npm install
npm run dev
```

El frontend queda disponible en `http://localhost:5173`.

---

## API

### `GET /characters`

Devuelve la lista de todos los personajes.

**Respuesta `200 OK`:**
```json
[
  {
    "id": 1,
    "name": "Goku",
    "race": "Saiyan",
    "powerLevel": 9000,
    "affiliation": "Z Fighters",
    "imageUrl": "https://dragonball-api.com/characters/goku_normal.webp"
  }
]
```

### `POST /characters`

Crea un nuevo personaje.

**Body:**
```json
{
  "name": "Bardock",
  "race": "Saiyan",
  "powerLevel": 10000,
  "affiliation": "None"
}
```

**Respuestas:**
- `201 Created` — personaje creado con `Location: /characters/{id}`
- `422 Unprocessable Entity` — validación fallida

**Reglas de validación:**
- `name` — requerido, máx. 100 caracteres
- `race` — requerido, máx. 50 caracteres
- `powerLevel` — mayor que 0
- `affiliation` — requerido, máx. 100 caracteres

---

## Tests

### Backend — 31 tests

```bash
dotnet test
```

| Suite | Descripción |
|---|---|
| `CreateCharacterValidatorTests` | Validaciones de nombre, raza, powerLevel y afiliación |
| `CreateCharacterHandlerTests` | Handler: comando válido, validación fallida |
| `GetCharactersHandlerTests` | Handler: retorna lista, lista vacía |
| `InMemoryCharacterRepositoryTests` | Repositorio: seed, add, id incremental, imageUrl presente |
| `GetCharactersEndpointTests` | `GET /characters` → 200, lista no vacía, imageUrl presente |
| `CreateCharacterEndpointTests` | `POST /characters` → 201, 422 en casos inválidos |

### Frontend — 24 tests

```bash
cd src/frontend
npm test                  # modo watch
npm run test:coverage     # con reporte de cobertura
```

| Suite | Descripción |
|---|---|
| `SearchBar.test.tsx` | Renderizado, valor actual, llamada a onChange |
| `FilterBar.test.tsx` | Opciones de raza/afiliación, selección, valor reflejado |
| `CharacterCard.test.tsx` | Nombre, raza, badge, power level, imagen, colores de badge |
| `CharacterGrid.test.tsx` | Nº de cards, nombres, empty state |
| `useCharacters.test.ts` | Loading inicial, éxito, error HTTP, error de red |

---

## Frontend — funcionalidades

- **Grid responsivo** de personajes (2 columnas en móvil → 5 en desktop)
- **Búsqueda** en tiempo real por nombre
- **Filtros** por raza y afiliación
- **Skeleton loading** mientras carga la API
- **Vista detalle** de cada personaje al hacer click
- **Tema oscuro** con colores naranja/dorado
