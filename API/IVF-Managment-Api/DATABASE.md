# Database setup

The API uses **EF Core + SQLite** with code-first migrations. Schema lives in C# (`IVF-Managment-Data/Models/` + `IvfDbContext.cs`) and gets compiled into migration files under `IVF-Managment-Data/Data/Migrations/`. Those migration files are checked into git — that's how everyone stays on the same schema. Each developer has their own local `ivf.db` file with their own test data; the file itself is never committed.

## Prerequisites

Install the EF Core CLI tool once, globally:

```bash
dotnet tool install --global dotnet-ef
```

(If you already have it: `dotnet tool update --global dotnet-ef`.)

## First time (after `git clone`)

From `API/IVF-Managment-Api/`:

```bash
dotnet restore
dotnet ef database update --project IVF-Managment-Data --startup-project IVF-Managment-Api
```

The second command creates `IVF-Managment-Api/ivf.db` and applies every migration in order. The file is empty — no patients, no doctors. Populate it by hitting the API.

Then run the API:

```bash
cd IVF-Managment-Api
dotnet run
```

Endpoints at `http://localhost:5267`. Swagger UI at `http://localhost:5267/swagger`.

## Daily use

Just `dotnet run`. The DB persists between runs.

## After someone else changes the schema

When you pull and see new files in `IVF-Managment-Data/Data/Migrations/`, run:

```bash
dotnet ef database update --project IVF-Managment-Data --startup-project IVF-Managment-Api
```

(from `API/IVF-Managment-Api/`). This applies the new migrations to your existing `ivf.db` — your data is preserved where possible.

## When YOU change the schema

1. Edit the entity class (e.g. add a property to `Patient.cs`) or add a new entity + a `DbSet` to `IvfDbContext.cs`.
2. Generate the migration:
   ```bash
   dotnet ef migrations add DescriptiveName \
       --project IVF-Managment-Data \
       --startup-project IVF-Managment-Api \
       --output-dir Data/Migrations
   ```
   Use a clear name: `AddPatientPhotoUrl`, `RenameDoctorLicenseColumn`, etc.
3. Apply it locally to verify it works:
   ```bash
   dotnet ef database update --project IVF-Managment-Data --startup-project IVF-Managment-Api
   ```
4. Commit the new files in `IVF-Managment-Data/Data/Migrations/` along with your model changes.

## Common operations

| What | Command |
|---|---|
| Apply pending migrations | `dotnet ef database update --project IVF-Managment-Data --startup-project IVF-Managment-Api` |
| List all migrations | `dotnet ef migrations list --project IVF-Managment-Data --startup-project IVF-Managment-Api` |
| Roll back to a specific migration | `dotnet ef database update <MigrationName> --project IVF-Managment-Data --startup-project IVF-Managment-Api` |
| Roll back ALL migrations (empty schema) | `dotnet ef database update 0 --project IVF-Managment-Data --startup-project IVF-Managment-Api` |
| Remove the most recent (unapplied) migration | `dotnet ef migrations remove --project IVF-Managment-Data --startup-project IVF-Managment-Api` |
| Nuke local DB and start fresh | Delete `IVF-Managment-Api/ivf.db`, then `database update` |

## Connection string

`IVF-Managment-Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=ivf.db"
}
```

The path is relative to the API's working directory, so `ivf.db` ends up next to `appsettings.json` when you `dotnet run`.

## Troubleshooting

- **`No project was found`** — you're in the wrong folder. Run `ef` commands from `API/IVF-Managment-Api/` (the folder containing `.sln`), with both `--project` and `--startup-project` flags.
- **`Unable to create a 'DbContext' of type 'IvfDbContext'`** — the API isn't building. Run `dotnet build` first to see the real compile error.
- **`SQLite Error 19: NOT NULL constraint failed`** — the entity says non-nullable but the inbound data is null. Either send the field, or change the entity property to `string?` and add a migration.
- **`The migration 'X' has already been applied`** — your DB is ahead of the code. Pull latest, or roll back with `database update <PreviousMigration>`.
- **Want a clean DB?** Delete `IVF-Managment-Api/ivf.db` and re-run `database update`. No migration history is lost — that lives in git.

## What NOT to do

- **Don't commit `ivf.db`.** It's binary, can't be merged, and contains your local test data.
- **Don't edit a migration file after committing it.** Once a migration ships, it's frozen — making changes means teammates' DBs get out of sync. To fix a mistake, write a new migration that corrects it.
- **Don't run `dotnet ef migrations remove` after pushing.** Same reason — write a new migration instead.
