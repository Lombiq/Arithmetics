# Domain docs

Use a single-context layout rooted in this repository: `GLOSSARY.md` for terminology and `docs/adr/` for architectural decisions.

Before exploring a domain, read the glossary and ADRs relevant to that area, when present. If a `GLOSSARY-MAP.md` exists later, follow its pointers to the relevant context glossaries and ADRs instead.

Proceed silently when these files are absent; create them lazily through domain-modeling when terminology or decisions are resolved.

Use the glossary's terms consistently in specifications, tickets, code, and tests. Flag conflicts with existing ADRs explicitly rather than silently overriding them.
