# Insurance Claims API

A small ASP.NET Core Web API for registering insurance claims and recording payments made against them.

## Tech stack

- **.NET 10** / ASP.NET Core Web API
- **Entity Framework Core** with **SQLite**
- **AutoMapper** for entity → resource mapping
- **Swagger / Swashbuckle** for interactive API docs

## Running locally

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- No separate database server needed — the app uses a local SQLite file.

### Option A — run with the .NET SDK

```bash
git clone https://github.com/paaks100/InsuranceClaims.git
cd InsuranceClaims
dotnet restore
dotnet run
```

On startup, the app automatically:
1. Applies any pending EF Core migrations (creating `claims.db` if it doesn't exist yet).
2. Seeds the database with sample data — 50 policies, 100 claims, and randomly generated payments against the approved ones — via `HasData` in the migrations, so it's there the very first time you run it.

No manual `dotnet ef database update` step is required.

The console output will show the URL(s) Kestrel is listening on (typically `http://localhost:5250` and `https://localhost:7025` for local development, depending on `launchSettings.json`). From there:

- Swagger UI: `http://localhost:5250/docs`
- Claims list: `http://localhost:5250/api/claims/list`

### Option B — run with Docker

```bash
docker build -t insurance-claims .
docker run -p 8080:8080 insurance-claims
```

The container listens on port `8080`. Once it's up:

- Swagger UI: `http://localhost:8080/docs` (note: Swagger is only wired up when `ASPNETCORE_ENVIRONMENT=Development`; the Docker image runs in `Production` by default, so Swagger won't be available unless you pass `-e ASPNETCORE_ENVIRONMENT=Development` when running the container)
- Claims list: `http://localhost:8080/api/claims/list`

The SQLite file is created inside the container's filesystem and is **not persisted** across container restarts — every fresh container starts from the seeded data again.

### Resetting local data

To wipe local data and start over from the seed set, just delete `claims.db` (and its `-shm`/`-wal` companions, if present) from the project root and re-run the app; migrations will recreate and reseed it.

## API overview

| Method | Route | Purpose |
|---|---|---|
| `POST` | `/api/policies/register` | Register a policy |
| `GET` | `/api/policies/list` | List all policies |
| `GET` | `/api/policies/{id}/retrieve` | Get a single policy |
| `POST` | `/api/claims/register` | Register a claim against a policy |
| `GET` | `/api/claims/{id}/retrieve` | Get a single claim, with payments |
| `PATCH` | `/api/claims/{id}/approve` | Set the approved amount on a claim |
| `POST` | `/api/claims/{id}/payments/add` | Record a payment against a claim |
| `GET` | `/api/claims/list` | List claims with filters (`StartDate`, `EndDate`, `Currency`, `Status`) plus a currency-grouped totals row |

## Assumptions

- **Date-range filtering** on the list endpoint filters by `DateNotified`, not `LossDate` — the brief didn't specify which date the range should apply to.
- **A claim must be approved before a payment can be recorded** against it, and a payment that would push total paid above the approved amount is rejected. This isn't stated explicitly in the brief but felt like the safer default for an insurance workflow.
- **Cross-currency payments** are handled by capturing the payment's original amount and currency, an exchange rate, and the equivalent amount in the claim's own currency — all totals and balances are always calculated in the claim's currency, using the converted amount.
- Claim status is derived as **Reserved**, **Settled, payment outstanding**, or **Settled and paid**, per the brief. An additional `Denied` status was added internally for an approved amount of exactly zero; with more time I'd fold this back into the three stated statuses so the derivation logic exactly matches the spec.
- **SQLite** was chosen for zero-setup local development and to keep the take-home simple. See the note below on its limitations when hosted.
- No authentication/authorization — out of scope for the brief.

## What I'd do differently with more time

- **Persistent storage on the hosted deployment.** SQLite inside a container on Render's free tier doesn't survive a restart or scale-to-zero, so anything created live is only there until the service idles out. I'd move to Render's managed Postgres for anything beyond a quick demo.
- **Replace AutoMapper with hand-rolled mapping**, or at minimum pin to a pre-commercial-license version — for a project this size, a couple of manual mapping methods remove an external licensing dependency for no real cost.
- **Automated tests** around the money arithmetic (totals grouping, currency conversion, balance/status derivation) — this is the part most likely to have subtle bugs and most worth locking down with tests.
- **A real exchange-rate source** (or at least a pluggable one) instead of requiring the caller to supply the rate and converted amount by hand.
- **Pagination** on the claims list endpoint, once the dataset is larger than the seed set.
