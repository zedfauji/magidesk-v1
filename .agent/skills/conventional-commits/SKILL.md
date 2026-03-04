---
name: conventional-commits
description: Enforce Conventional Commits when making commit messages.
---
# Conventional Commits Enforcer

When asked to commit changes, format the commit message as:

`<type>[<scope>]: <description>`

Allowed types:
- feat
- fix
- docs
- style
- refactor
- perf
- test
- chore

Rules:
1) Analyze diff context to determine type.
2) Infer scope from affected components.
3) Generate description in imperative mood.
4) If breaking, add footer:
   `BREAKING CHANGE: <impact>`

Use this when user issues commands like:
- "commit changes"
- "make commit"
- "push update"
