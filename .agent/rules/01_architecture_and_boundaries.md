# Architecture and Boundaries

Agents must always respect the architectural layering of this project.

## Constraints
- UI view code must not contain business logic.
- ViewModels encapsulate all non-UI logic.
- Services are responsible for data access and domain interactions.
- No cross-layer calls that bypass ViewModel → Service → Data layers.

## Why this matters
This directory structure and layering are enforced in code and team standards.
