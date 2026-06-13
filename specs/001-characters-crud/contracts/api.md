# API Contract: Dragon Ball Characters

**Base URL**: `http://localhost:5000` (dev) | `http://localhost:5255` (http profile)
**Documentation**: `http://localhost:5000/scalar/v1`
**OpenAPI JSON**: `http://localhost:5000/openapi/v1.json`

---

## GET /characters

Devuelve todos los personajes en memoria.

**Response 200**
```json
[
  {
    "id": 1,
    "name": "Goku",
    "lastName": "Son",
    "race": "Saiyan",
    "powerLevel": 9000,
    "description": null,
    "affiliation": "Z Fighters",
    "imageUrl": "https://dragonball-api.com/characters/goku_normal.webp"
  }
]
```

**Scalar tags**: `Characters`
**WithName**: `GetCharacters`

---

## GET /characters/{id}

Devuelve un personaje por su ID.

**Path params**: `id` (int, requerido)

**Response 200**
```json
{
  "id": 1,
  "name": "Goku",
  "lastName": "Son",
  "race": "Saiyan",
  "powerLevel": 9000,
  "description": null,
  "affiliation": "Z Fighters",
  "imageUrl": "https://dragonball-api.com/characters/goku_normal.webp"
}
```

**Response 404**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404
}
```

**Scalar tags**: `Characters`
**WithName**: `GetCharacterById`

---

## POST /characters

Crea un nuevo personaje en memoria.

**Request Body** (`application/json`)
```json
{
  "name": "Goku",
  "lastName": "Son",
  "race": "Saiyan",
  "powerLevel": 9000,
  "description": "El protagonista de la serie Dragon Ball Z.",
  "affiliation": "Z Fighters",
  "imageUrl": "https://example.com/goku.webp"
}
```

| Campo | Tipo | Requerido | Validación |
|-------|------|-----------|-----------|
| name | string | ✓ | NotEmpty, ≤ 100 chars |
| lastName | string | ✓ | NotEmpty, ≤ 100 chars |
| race | string? | — | ≤ 100 chars si presente |
| powerLevel | int | ✓ | ≥ 0 |
| description | string? | — | ≤ 500 chars si presente |
| affiliation | string? | — | sin restricción de longitud adicional |
| imageUrl | string? | — | sin restricción |

**Response 201** — objeto `Character` completo con `id` generado.

**Response 422** (validación fallida)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 422,
  "errors": {
    "Name": ["El nombre es obligatorio."],
    "LastName": ["El apellido es obligatorio."],
    "PowerLevel": ["El nivel de poder no puede ser negativo."]
  }
}
```

**Response 400** — JSON malformado o tipo incorrecto.

**Scalar tags**: `Characters`
**WithName**: `CreateCharacter`

---

## PUT /characters/{id}

Actualiza parcialmente un personaje existente. Solo los campos presentes en el body
se modifican; los omitidos conservan su valor actual.

**Path params**: `id` (int, requerido)

**Request Body** (`application/json`) — todos los campos son opcionales
```json
{
  "powerLevel": 150000,
  "description": "Ahora más poderoso tras el entrenamiento en el Hyperbolic Time Chamber."
}
```

| Campo | Tipo | Requerido | Validación (si presente) |
|-------|------|-----------|--------------------------|
| name | string? | — | NotEmpty, ≤ 100 chars |
| lastName | string? | — | NotEmpty, ≤ 100 chars |
| race | string? | — | ≤ 100 chars |
| powerLevel | int? | — | ≥ 0 |
| description | string? | — | ≤ 500 chars |
| affiliation | string? | — | — |
| imageUrl | string? | — | — |

**Response 200** — objeto `Character` completo con todos los campos (actualizados + no modificados).

**Response 404** — ID no existe.

**Response 422** — campo enviado inválido (p.ej. name enviado como "").

**Response 400** — JSON malformado.

**Scalar tags**: `Characters`
**WithName**: `UpdateCharacter`

---

## DELETE /characters/{id}

Elimina un personaje del catálogo en memoria.

**Path params**: `id` (int, requerido)

**Response 204** — eliminación exitosa, body vacío.

**Response 404** — ID no existe.

**Scalar tags**: `Characters`
**WithName**: `DeleteCharacter`

---

## Errores comunes (todos los endpoints)

| Escenario | HTTP | Body |
|-----------|------|------|
| ID en URL no numérico (`/characters/abc`) | 400 | Problem Details |
| JSON body inválido | 400 | Problem Details |
| Campo de validación fallido | 422 | ValidationProblem con `errors` por campo |
| ID no encontrado | 404 | Problem Details |
| Operación exitosa sin body | 204 | (vacío) |
