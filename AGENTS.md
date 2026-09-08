# Psycho Cat Development Guidelines

## Project

- Engine: Unity 6.3 LTS
- Render Pipeline: URP
- Platform: Windows PC
- Language: C#
- Game: Psycho Cat

## Architecture

- All game-specific content belongs under Assets/_Project.
- Gameplay scripts belong under Assets/_Project/Scripts.
- Prefer small, focused and modular components.
- Avoid God classes.
- Prefer composition over large inheritance hierarchies.
- Keep systems loosely coupled.
- Use interfaces and events where communication between independent gameplay systems is appropriate.
- Avoid unnecessary global state and static managers.
- Avoid hardcoded scene object references when a cleaner dependency can be used.
- Expose designer-tunable gameplay values through the Unity Inspector when appropriate.
- Do not introduce abstractions until they are needed.

## Unity Rules

- Do not add third-party packages without explicit approval.
- Do not modify Packages unless explicitly requested.
- Do not modify ProjectSettings unless explicitly requested.
- Do not change render pipeline configuration without explicit approval.
- Never manually create or edit Unity .meta files unless explicitly required.
- Do not modify unrelated assets or scenes.
- Keep prefabs reusable and focused.
- Avoid putting game-specific assets directly in the root Assets folder.

## Development Workflow

- Keep every task small and reviewable.
- Do not implement features outside the requested scope.
- Inspect existing implementations before creating duplicate systems.
- Do not automatically commit changes.
- Before finishing a task, check the changed files and report them.
- Prefer readable, simple code over clever code.
- Comments should explain WHY when necessary, not repeat obvious code.

## Planned Gameplay Areas

The project will eventually contain:

- First-person Player Controller
- Interaction System
- Pick-up / Drop / Throw objects
- Noise / Sound Event system
- Psycho Cat AI
- Patrol
- Investigate
- Ambush
- Sabotage / Hack
- Chase
- Doors
- Light switches
- Environment interactions
- Phone / diegetic UI
- Gameplay systems

These are future systems.
Do NOT implement them unless a task explicitly requests them.

## Current Vertical Slice Goal

The initial prototype will eventually allow this scenario:

Player enters a small house
→ walks around
→ interacts with an object
→ picks up a glass
→ throws/drops it
→ the impact creates a noise event
→ the cat hears the noise
→ the cat investigates the location.

This is only project context.
Do NOT implement this vertical slice during the current setup task.
