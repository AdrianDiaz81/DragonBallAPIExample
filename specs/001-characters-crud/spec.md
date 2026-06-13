# Feature Specification: Dragon Ball Characters — CRUD API

**Feature Branch**: `001-characters-crud`
**Created**: 2026-06-13
**Status**: Implemented
**Input**: REST API mínima para consultar, añadir, modificar y eliminar personajes de Dragon Ball.
Sin base de datos; los datos se cargan en memoria al arrancar la aplicación.

---

## User Scenarios & Testing *(mandatory)*

### User Story 1 — Consultar el catálogo de personajes (Priority: P1)

Como consumidor de la API, quiero obtener la lista completa de personajes de Dragon Ball
para poder mostrarlos en el frontal.

**Why this priority**: Sin datos no hay nada que mostrar. Es el punto de entrada de toda
la aplicación y el primer contrato que el frontend necesita.

**Independent Test**: Llamar a `GET /characters` devuelve un array JSON con al menos los
personajes de la carga inicial (no vacío). Verificable con Scalar o curl sin ningún setup previo.

**Acceptance Scenarios**:

1. **Given** la aplicación arranca con la carga inicial de datos,
   **When** se llama a `GET /characters`,
   **Then** se devuelve HTTP 200 con un array JSON de objetos `Character` (mínimo 5 personajes).

2. **Given** el sistema tiene personajes cargados,
   **When** se llama a `GET /characters/{id}` con un ID existente,
   **Then** se devuelve HTTP 200 con el objeto `Character` correspondiente.

3. **Given** el sistema tiene personajes cargados,
   **When** se llama a `GET /characters/{id}` con un ID inexistente,
   **Then** se devuelve HTTP 404 con un mensaje descriptivo.

---

### User Story 2 — Añadir un nuevo personaje (Priority: P2)

Como administrador de la API, quiero añadir nuevos personajes al catálogo para mantenerlo
actualizado.

**Why this priority**: La escritura depende de que la lectura funcione. Es el segundo
valor más importante para demostrar un CRUD completo.

**Independent Test**: Llamar a `POST /characters` con datos válidos y luego a `GET /characters`
confirma que el personaje aparece en la lista. El test no requiere persistencia en disco.

**Acceptance Scenarios**:

1. **Given** un payload con nombre, apellido y fuerza válidos (fuerza >= 0),
   **When** se llama a `POST /characters`,
   **Then** se devuelve HTTP 201 con el objeto `Character` creado (incluye ID generado).

2. **Given** un payload con nombre vacío o ausente,
   **When** se llama a `POST /characters`,
   **Then** se devuelve HTTP 422 con detalle del campo inválido (`name` requerido).

3. **Given** un payload con apellido vacío o ausente,
   **When** se llama a `POST /characters`,
   **Then** se devuelve HTTP 422 con detalle del campo inválido (`lastName` requerido).

4. **Given** un payload con `powerLevel` con valor negativo (p.ej. -1),
   **When** se llama a `POST /characters`,
   **Then** se devuelve HTTP 422 indicando que la fuerza no puede ser negativa.

5. **Given** un payload con `powerLevel` igual a 0,
   **When** se llama a `POST /characters`,
   **Then** se devuelve HTTP 201 (0 es un valor de fuerza permitido).

---

### User Story 3 — Modificar un personaje existente (Priority: P2)

Como administrador de la API, quiero actualizar parcialmente los datos de un personaje
existente para corregir errores o reflejar cambios del universo Dragon Ball sin tener
que reenviar todos los campos.

**Why this priority**: Paridad de prioridad con la creación; un CRUD sin update no
cumple el requisito mínimo funcional.

**Independent Test**: Llamar a `PUT /characters/{id}` con solo `powerLevel` y luego a
`GET /characters/{id}` confirma que solo ese campo ha cambiado; el resto permanece igual.

**Acceptance Scenarios**:

1. **Given** un personaje existente con ID conocido,
   **When** se llama a `PUT /characters/{id}` con un payload parcial válido (p.ej. solo `powerLevel`),
   **Then** se devuelve HTTP 200 con el objeto `Character` completo actualizado (solo el campo enviado cambia).

2. **Given** un ID que no existe en el catálogo,
   **When** se llama a `PUT /characters/{id}`,
   **Then** se devuelve HTTP 404.

3. **Given** un payload de actualización donde `name` se envía vacío,
   **When** se llama a `PUT /characters/{id}`,
   **Then** se devuelve HTTP 422 con detalle de validación (campo enviado inválido).

4. **Given** un payload de actualización con `powerLevel` negativo,
   **When** se llama a `PUT /characters/{id}`,
   **Then** se devuelve HTTP 422 indicando que la fuerza no puede ser negativa.

5. **Given** un payload completamente vacío `{}`,
   **When** se llama a `PUT /characters/{id}`,
   **Then** se devuelve HTTP 200 con el objeto `Character` sin cambios (no-op válido).

---

### User Story 4 — Eliminar un personaje (Priority: P3)

Como administrador de la API, quiero eliminar un personaje del catálogo cuando ya no es
relevante.

**Why this priority**: La eliminación es el último paso del CRUD. El sistema es funcional
sin ella, pero el requisito la exige.

**Independent Test**: Llamar a `DELETE /characters/{id}` y luego a `GET /characters/{id}`
devuelve 404. El resto del catálogo permanece intacto.

**Acceptance Scenarios**:

1. **Given** un personaje existente con ID conocido,
   **When** se llama a `DELETE /characters/{id}`,
   **Then** se devuelve HTTP 204 (No Content).

2. **Given** un ID que no existe,
   **When** se llama a `DELETE /characters/{id}`,
   **Then** se devuelve HTTP 404.

3. **Given** que se elimina un personaje con éxito,
   **When** se llama a `GET /characters/{id}` con ese mismo ID,
   **Then** se devuelve HTTP 404.

---

### Edge Cases

- `GET /characters` cuando todos los personajes han sido eliminados en runtime → HTTP 200
  con array vacío `[]` (no 404).
- `PUT /characters/{id}` con body vacío o malformado (JSON inválido) → HTTP 400 Bad Request.
- `POST /characters` con `powerLevel` no numérico (p.ej. `"mucho"`) → HTTP 400 Bad Request.
- IDs en la URL con formato no numérico (p.ej. `/characters/abc`) → HTTP 400 Bad Request.
- Nombre o apellido con solo espacios en blanco → HTTP 422 (se trata como vacío, se aplica trim).
- `powerLevel` ausente del payload → HTTP 422 (campo requerido).

---

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: El sistema DEBE exponer un endpoint `GET /characters` que devuelva todos los
  personajes en memoria.
- **FR-002**: El sistema DEBE exponer un endpoint `GET /characters/{id}` que devuelva un
  personaje por su ID; si no existe, devuelve 404.
- **FR-003**: El sistema DEBE exponer un endpoint `POST /characters` que acepte un payload
  JSON y cree un nuevo personaje en memoria con ID autogenerado.
- **FR-004**: El sistema DEBE exponer un endpoint `PUT /characters/{id}` que actualice
  parcialmente un personaje existente (solo los campos presentes en el payload); si no
  existe, devuelve 404. Los campos omitidos conservan su valor actual.
- **FR-005**: El sistema DEBE exponer un endpoint `DELETE /characters/{id}` que elimine un
  personaje en memoria; si no existe, devuelve 404.
- **FR-006**: El sistema DEBE rechazar con HTTP 422 cualquier personaje cuyo `name` esté
  vacío, sea nulo o contenga solo espacios en blanco.
- **FR-007**: El sistema DEBE rechazar con HTTP 422 cualquier personaje cuyo `lastName` esté
  vacío, sea nulo o contenga solo espacios en blanco.
- **FR-008**: El sistema DEBE rechazar con HTTP 422 cualquier personaje cuyo `powerLevel`
  sea un número negativo (< 0).
- **FR-009**: Al arrancar la aplicación, el sistema DEBE cargar en memoria un dataset inicial
  de al menos 5 personajes de Dragon Ball (sin base de datos ni fichero externo).
- **FR-010**: La API DEBE documentarse automáticamente con Scalar en la ruta `/scalar/v1`.
- **FR-011**: Todos los endpoints DEBEN estar agrupados bajo el tag `Characters` en la
  documentación Scalar.

### Restricciones Técnicas

- **RT-001**: SIN base de datos. El almacenamiento es una colección en memoria
  (p.ej. `List<Character>` registrada como Singleton en DI).
- **RT-002**: SIN autenticación ni autorización. Los endpoints son públicos.
- **RT-003**: SIN lógica de persistencia entre reinicios. Los datos de la carga inicial
  se restauran con cada arranque.
- **RT-004**: El frontal React DEBE consumir la API desde `http://localhost:5000` (o el
  puerto configurado) y exponer dos vistas:
  - **Lista** (`/`): grid de `CharacterCard` con todos los personajes; al clicar navega al detalle.
  - **Detalle** (`/characters/:id`): página `CharacterPage` con todos los campos del personaje.
  - Las operaciones de escritura (POST, PUT, DELETE) se prueban exclusivamente desde Scalar.
  - CORS configurado para `http://localhost:5173`.

### Key Entities

- **Character**: Representa un personaje de Dragon Ball.
  - `id` (int): Identificador único autogenerado, positivo.
  - `name` (string): Nombre del personaje. Obligatorio, no vacío, máx. 100 caracteres.
  - `lastName` (string): Apellido del personaje. Obligatorio, no vacío, máx. 100 caracteres.
  - `race` (string): Raza del personaje (p.ej. "Saiyan", "Namekian", "Human"). Opcional,
    texto libre. Si se envía, máx. 100 caracteres. No se valida contra un enum de valores fijos.
  - `powerLevel` (int): Nivel de fuerza. Obligatorio, >= 0.
  - `description` (string?): Descripción libre del personaje. Opcional, máx. 500 caracteres.

### Contrato de la API

| Método | Ruta | Request Body | Response |
|--------|------|-------------|----------|
| GET | `/characters` | — | `200 Character[]` |
| GET | `/characters/{id}` | — | `200 Character` / `404` |
| POST | `/characters` | `CreateCharacterRequest` | `201 Character` / `422` |
| PUT | `/characters/{id}` | `UpdateCharacterRequest` | `200 Character` / `404` / `422` |
| DELETE | `/characters/{id}` | — | `204` / `404` |

**CreateCharacterRequest**:
```json
{
  "name": "Goku",
  "lastName": "Son",
  "race": "Saiyan",
  "powerLevel": 9000,
  "description": "El protagonista de la serie."
}
```

**UpdateCharacterRequest**: todos los campos son opcionales — solo se envían los que se
desean modificar. Los campos no incluidos en el payload conservan su valor actual.
Si un campo se envía, se le aplica la misma validación que en POST (p.ej. `name` enviado
no puede ser vacío; `powerLevel` enviado no puede ser negativo).

---

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: `GET /characters` responde en < 100 ms con los datos de la carga inicial tras
  el arranque del servidor.
- **SC-002**: `POST /characters` con datos válidos devuelve HTTP 201 y el objeto creado
  con un ID único no repetido.
- **SC-003**: Los tres errores de validación (nombre vacío, apellido vacío, fuerza negativa)
  devuelven HTTP 422 con descripción del campo fallido.
- **SC-004**: `GET /scalar/v1` muestra todos los endpoints documentados sin errores de schema.
- **SC-005**: `dotnet test` ejecuta al menos un test unitario por cada regla de negocio
  crítica (FR-006, FR-007, FR-008) y todos pasan en verde.
- **SC-006**: El frontal React muestra la lista de personajes en `/` y el detalle de un
  personaje en `/characters/:id`, ambas sin errores en consola de navegador.

---

## Clarifications

### Session 2026-06-13

- Q: ¿PUT es reemplazo total o actualización parcial? → A: Actualización parcial — solo los campos enviados se modifican; los omitidos conservan su valor. Si un campo se envía, se valida igual que en POST.
- Q: ¿Qué operaciones expone la UI del frontal React? → A: Lista de personajes (`/`) + página de detalle al clicar (`/characters/:id`). Escritura solo desde Scalar.
- Q: ¿El campo `race` es texto libre o enum cerrado? → A: Texto libre. Si se envía, máximo 100 caracteres. Sin validación de valores concretos.

---

## Assumptions

- El dataset inicial se define como código estático en C# (no requiere archivo JSON externo).
- Los IDs se generan como enteros auto-incrementales en memoria (no GUIDs).
- La concurrencia no es un requisito: el sistema está diseñado para un único usuario de demo.
- El frontal expone dos vistas de solo lectura: lista (`/`) y detalle (`/characters/:id`).
  Las operaciones de escritura se prueban desde Scalar o herramientas REST.
- `race` y `description` son opcionales para permitir personajes con información mínima.
- El proyecto de tests está en `tests/` y sigue las convenciones de xUnit v3 del CLAUDE.md.
