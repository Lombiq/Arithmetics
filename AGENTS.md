## Generated code: template-only editing (critical)

The T4 `.tt` templates are the source of truth for generated posit implementations, helper overloads, and tests.
Edit only the owning template or shared `.ttinclude`; never edit generated `.cs` files directly, even temporarily or for
formatting and analyzer fixes. Only the generator writes these outputs; direct edits will be overwritten by the next build.

Before changing posit code, tests, or generation, read [Editing generated posit code](Readme.md#editing-generated-posit-code)
for source ownership, template compiler constraints, MSBuild regeneration, and checked-in output requirements.
Identify hand-maintained code separately; not every `.cs` file is generated.
Every implementation ticket must include its template changes and regenerated outputs together and satisfy the documented
regeneration checks.

## Scientific maintainability

Keep each change surgical and complete: implement the assigned contract and its required evidence with the smallest clear
design. Reuse existing production helpers and template mechanisms rather than adding parallel implementations,
speculative abstractions, dependencies, or configuration. Keep independent test oracles independent.
Use targeted refactoring and simplification when they remove duplication, clarify numerical invariants, or enable the
assigned change; leave unrelated cleanup out. Verify behavior preservation separately from intentional numerical fixes.
Retain the mathematical justification for non-obvious rounding, intermediate widths, and termination bounds in concise
comments or linked documentation. Simpler code must not weaken correctness, coverage, or hardware guarantees.

## Posit compliance work

Before implementing or reviewing a Posit compliance ticket, read the
[specification and ticket execution requirements](.scratch/posit-2022-compliance/spec.md#ticket-execution).
They require reading the relevant original standard sections and recording a section-linked numerical contract before
coding; a ticket summary or another agent's account is not a substitute.

## Agent skills

### Issue tracker

For specs, tickets, and issue updates, use the local Markdown tracker. Read `docs/agents/issue-tracker.md`.

### Triage labels

For issue status changes, use the canonical roles in `docs/agents/triage-labels.md`.

### Domain docs

Before domain exploration or specification work, read the single-context rules in `docs/agents/domain.md`.
