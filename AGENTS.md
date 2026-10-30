## Generated code: template-only editing (critical)

The T4 `.tt` templates are the source of truth for generated posit implementations, helper overloads, and tests.
Edit only the owning template or shared `.ttinclude`; never edit generated `.cs` files directly, even temporarily or for
formatting and analyzer fixes. Only the generator writes these outputs; direct edits will be overwritten by the next build.

Before changing posit code, tests, or generation, read [Editing generated posit code](Readme.md#editing-generated-posit-code)
for source ownership, template compiler constraints, MSBuild regeneration, and checked-in output requirements.
Identify hand-maintained code separately; not every `.cs` file is generated.
Every implementation ticket must include its template changes and regenerated outputs together and satisfy the documented
regeneration checks.

## Agent skills

### Issue tracker

For specs, tickets, and issue updates, use the local Markdown tracker. Read `docs/agents/issue-tracker.md`.

### Triage labels

For issue status changes, use the canonical roles in `docs/agents/triage-labels.md`.

### Domain docs

Before domain exploration or specification work, read the single-context rules in `docs/agents/domain.md`.
