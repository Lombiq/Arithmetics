# Issue tracker: Local Markdown

All paths below are relative to the Lombiq.Arithmetics repository root.

- One feature per directory: `.scratch/<feature-slug>/`.
- Specification: `.scratch/<feature-slug>/spec.md`.
- Implementation tickets: one file each at `.scratch/<feature-slug>/issues/<NN>-<slug>.md`, numbered from `01` in dependency order.
- Record `Status:` near the top using the roles in `triage-labels.md`.
- Record `Blocked by:` using ticket numbers and titles, or `None`.
- Implementation tickets use `Status: done` when completed; a ticket is startable when all its blockers are done.
- Append discussion under `## Comments`.

Publishing means creating or updating these local files. Fetching means reading the referenced file. Keep tracker operations local unless the user explicitly requests external publication.

## Wayfinding

Use `.scratch/<effort>/map.md` for the map and numbered child files under `issues/`. Record `Type: research`, `prototype`, `grilling`, or `task`. Set `Status: claimed` before work and `Status: resolved` after appending the answer under `## Answer`. Wayfinding blockers clear when resolved. Add a pointer to each resolved answer in the map's Decisions-so-far section.
