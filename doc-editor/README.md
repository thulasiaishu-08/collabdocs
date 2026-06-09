# CollabDocs — Collaborative Document Editor

Full-stack collaborative document editor built with React + Vite (frontend) and ASP.NET Core 8 + SQLite (backend).

---

## Prerequisites

| Tool | Version | Install |
|------|---------|---------|
| .NET SDK | 8.0+ | https://dotnet.microsoft.com/download |
| Node.js | 18+ | https://nodejs.org |

---

## Project Structure

```
doc-editor/
├── backend/
│   ├── DocEditor.API/          ← ASP.NET Core Web API
│   │   ├── Controllers/        ← Auth, Documents, Shares, Users
│   │   ├── Data/               ← DbContext + DataSeeder
│   │   ├── DTOs/               ← Request/Response records
│   │   ├── Middleware/         ← MockAuthMiddleware
│   │   ├── Models/             ← EF Core entities
│   │   ├── Services/           ← IDocumentService + DocumentService
│   │   ├── Program.cs
│   │   └── appsettings.json
│   └── DocEditor.Tests/        ← xUnit test project
└── frontend/
    └── src/
        ├── components/         ← Login, Layout, Sidebar, DocumentEditor,
        │                         RenameModal, ShareModal, UploadModal
        ├── context/            ← AuthContext
        └── services/           ← api.js (fetch wrapper)
```

---

## Quick Start

### 1. Backend

```bash
cd backend/DocEditor.API

# Restore & run (creates doceditor.db automatically)
dotnet run
```

API runs at **http://localhost:5000**  
Swagger UI at **http://localhost:5000/swagger**

### 2. Frontend

```bash
cd frontend

npm install
npm run dev
```

App runs at **http://localhost:5173**

### 3. Run Tests

```bash
cd backend

# Create solution (first time only)
dotnet new sln -n DocEditor
dotnet sln add DocEditor.API/DocEditor.API.csproj
dotnet sln add DocEditor.Tests/DocEditor.Tests.csproj

dotnet test
```

---

## Demo Credentials

| Username | Password  |
|----------|-----------|
| alice    | password1 |
| bob      | password2 |

Use the **Quick Login** buttons on the login screen, or type credentials manually.

---

## Features

| # | Feature | Implementation |
|---|---------|---------------|
| 1 | Mock login | `/api/auth/login` → token = userId string; `MockAuthMiddleware` validates `Bearer {userId}` header |
| 2 | Create document | POST `/api/documents` |
| 3 | Rename document | PATCH `/api/documents/{id}/rename` — owner only |
| 4 | Rich text editing | React Quill (Snow theme) with full toolbar |
| 5 | Save & reopen | Auto-save after 1.5 s idle; SQLite persistence |
| 6 | Upload .txt / .md | POST `/api/documents/upload` (multipart/form-data) |
| 7 | Share with user | POST `/api/shares`; unshare via DELETE `/api/shares/{id}` |
| 8 | My / Shared sections | Separate sidebar sections; `GET /api/documents` and `GET /api/documents/shared` |
| 9 | SQLite persistence | EF Core 8 with `EnsureCreated()` |
| 10 | Validation | Titles required & ≤200 chars; file type checked; self-share rejected |
| 11 | xUnit tests | 7 tests covering CRUD, sharing, access control |

---

## API Reference

### Auth
| Method | Path | Body | Notes |
|--------|------|------|-------|
| POST | `/api/auth/login` | `{username, password}` | Returns `{token, userId, username}` |

### Documents  *(all require `Authorization: Bearer {token}`)*
| Method | Path | Notes |
|--------|------|-------|
| GET | `/api/documents` | My documents |
| GET | `/api/documents/shared` | Documents shared with me |
| GET | `/api/documents/{id}` | Full document (owner or shared) |
| POST | `/api/documents` | Create `{title, content?}` |
| PUT | `/api/documents/{id}` | Save content `{content}` |
| PATCH | `/api/documents/{id}/rename` | Rename `{title}` — owner only |
| DELETE | `/api/documents/{id}` | Delete — owner only |
| POST | `/api/documents/upload` | Upload `.txt`/`.md` as `multipart/form-data` |
| GET | `/api/documents/{id}/shares` | List shares — owner only |

### Shares
| Method | Path | Notes |
|--------|------|-------|
| POST | `/api/shares` | Share `{documentId, sharedWithUserId}` |
| DELETE | `/api/shares/{id}` | Remove share — owner only |

### Users
| Method | Path | Notes |
|--------|------|-------|
| GET | `/api/users` | List all users (for share UI) |
