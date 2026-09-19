# Psycho Cat — Project Bible / Codex Context

> **Document role:** This file is the shared product/design context for Psycho Cat. It exists so developers and Codex understand *what game we are building*, *which systems already exist*, *what is confirmed*, and *what is still undecided*.
>
> **Priority rule for Codex:**
> 1. The current task prompt defines the immediate scope.
> 2. `AGENTS.md` defines coding/project rules.
> 3. This document defines product/design intent and shared context.
> 4. If these conflict, stop and report the conflict instead of silently inventing a solution.

---

## 1. Project Identity

**Working title:** Psycho Cat  
**Platform:** PC / Windows  
**Engine:** Unity 6.3 LTS (6000.3.19f1)  
**Render pipeline:** URP  
**Input:** Unity New Input System  
**Business model:** Premium, one-time purchase target for Steam  
**Languages planned:** Turkish and English

### High-level genre

First-person **survival + escape-room + absurd comedy / horror**.

The player is responsible for an unusually intelligent, malicious cat inside a house. At first the situation feels like a strange pet-sitting / animal-training job. As the days progress, the cat reveals greater intelligence, manipulates the smart home, sabotages the player, and eventually escalates into occult / laboratory-driven behavior and transformations.

### Core identity sentence

> **The cat is not only the enemy; the cat turns the entire house into a weapon.**

Psycho Cat should not feel like a generic “evil animal chases the player” horror game. The distinctive fantasy is living in a house where an increasingly powerful cat understands the environment, manipulates technology, creates absurd situations, and later uses experimental / magical abilities to change the rules of play.

---

## 2. Player Fantasy

The player should feel like:

- a capable person trying to keep a normal household under control,
- a caretaker / animal trainer responsible for a difficult cat,
- someone balancing ordinary human needs with increasingly abnormal threats,
- someone who gradually realizes the cat is far more intelligent and dangerous than expected,
- someone who can use the environment to counter, distract, trick, or temporarily control the cat.

The player should **not** feel like a heavily armed action hero.

The core tension comes from limited attention, household responsibilities, time pressure, environmental problems, and the cat’s unpredictable behavior.

---

## 3. Design Pillars

### 3.1 Normal life versus escalating chaos

The game begins with believable domestic routines:

- eating,
- drinking,
- sleeping,
- feeding the cat,
- cleaning,
- managing toys,
- taking out trash,
- dealing with household devices.

The comedy/horror works because increasingly absurd events invade this normal routine.

### 3.2 The house is a gameplay system

Doors, lights, cameras, phone functions, appliances, smart-home systems, movable props, and other household objects should have gameplay value.

The environment should not exist only as decoration.

### 3.3 The cat changes the rules

The cat should evolve beyond a single chase AI.

Early game:
- patrol,
- observe/investigate,
- react to sounds,
- perform small sabotages.

Later game:
- smart-home manipulation,
- deliberate traps,
- laboratory activity,
- occult / experimental abilities,
- transformations into other animal/creature forms.

A transformation should ideally change gameplay, not only the visual model.

### 3.4 Absurd comedy inside real tension

The game should create streamer-friendly “what the hell is this cat doing?” moments.

Examples already discussed/noted:

- reverse cucumber joke / jumpscare,
- absurd online orders,
- FBI / police-style absurd escalation connected to cat-related events,
- household systems behaving maliciously,
- cat-created situations that are funny and threatening at the same time.

### 3.5 Systemic counterplay

The player should sometimes be able to turn the environment against the cat.

Examples:

- throwing an object to create a distraction,
- using sound to manipulate cat investigation,
- using a vacuum or another household device as counter-sabotage,
- solving or reversing smart-home sabotage.

---

## 4. Core Gameplay Loop

A typical loop should resemble:

1. Player receives / notices a household need or responsibility.
2. Player moves through the house and interacts with objects.
3. The cat patrols, investigates, watches, or prepares a sabotage.
4. The player must decide whether to continue the routine or respond to the cat.
5. A sabotage/event changes priorities.
6. The player solves, avoids, or counters the problem.
7. Player resources / needs continue to matter.
8. The day advances toward completion.
9. New days introduce stronger or stranger behaviors.

Short form:

`Routine -> Cat behavior -> Sabotage -> Response -> Resource pressure -> Day progression`

The game should avoid becoming a simple checklist simulator. Daily chores are useful because they create vulnerability and opportunity for the cat.

---

## 5. Day / Progression Structure

The current concept uses a **multi-day structure**, with notes suggesting roughly **5–7 days** and the broader GDD using a one-week framing.

Exact day count is still an open design decision, but progression should clearly escalate.

### Suggested escalation philosophy

**Early days**  
The cat is strange, intelligent, annoying, and suspicious.

- basic patrol/investigation,
- small household sabotage,
- strange use of devices,
- simple scares / jokes.

**Middle days**  
The player realizes the cat is intentionally manipulating the house.

- more aggressive smart-home sabotage,
- stronger traps,
- phone/camera-related events,
- clues to a hidden laboratory / secret activity.

**Late days**  
The conflict becomes overtly unnatural.

- laboratory becomes important,
- magical / experimental systems appear,
- the cat produces strange things,
- the cat can transform into other animal/creature forms,
- gameplay rules become less predictable.

The escalation should feel like:

> “This cat is weird.” -> “This cat is doing this on purpose.” -> “This cat is controlling the house.” -> “This cat is building something.” -> “This is no longer a normal cat.”

---

## 6. Player Needs / Resource Management

Handwritten notes explicitly mention:

- water,
- food,
- sleep,
- stamina.

The existing broader design direction has emphasized **Hunger + Sleep** as the most important needs to avoid feature bloat. Water/stamina remain possible but should not be added automatically unless a task explicitly calls for them.

### Intended purpose

Needs exist to create decisions and vulnerability, not to become a deep survival simulator.

Examples:

- the player may need to eat while the cat is doing something suspicious,
- sleeping creates risk because the player gives up awareness,
- exhaustion may affect perception / visuals,
- events and chores compete with personal needs.

### Notes-derived ideas

- cereal was written as a simple hunger example,
- bed is the obvious sleep recovery location,
- notes suggest sleeplessness could cause hallucination / distorted perception,
- sleep may become necessary after events / as the day advances.

Treat these as design candidates unless explicitly promoted into implementation tasks.

---

## 7. Daily Household Responsibilities

Notes include examples such as:

- water,
- food,
- toilet/bathroom needs,
- putting cat toys back into a box,
- tea / household routine ideas,
- trash,
- robot vacuum,
- feeding / special food.

There is also a note implying **special food every day** and a harsh fail condition if food is unavailable.

The exact daily task list and fail rules are not finalized.

### Design rule

Daily tasks should:

- create reasons to move around the house,
- expose the player to the cat,
- create timing conflicts,
- support comedy/horror events,
- not feel like repetitive busywork.

---

## 8. Cat AI

The cat is a central systemic actor, not a scripted prop.

### Current/target early AI states

Existing design and task planning include:

- Navigation,
- Patrol,
- Investigate,
- Sound Perception,
- Basic Hack / Sabotage.

The broader design also includes future behavior concepts such as:

- Ambush,
- Charge,
- Observe,
- more advanced Hack/Sabotage.

### Behavioral intent

The cat should:

- move independently through the environment,
- react to meaningful sounds,
- investigate locations rather than magically knowing everything,
- use the house strategically,
- create pressure without behaving like a constant chase enemy.

### Existing design values

Earlier notes/GDD included rough values such as:

- cat walking speed around player crouch-speed territory,
- run roughly 15% faster than normal walk conceptually,
- approximately 120-degree vision cone.

These are tuning references, not immutable constants.

---

## 9. Sound / Investigation Gameplay

Sound is important because it connects environment interaction with Cat AI.

Target chain:

`Player/environment action -> Sound event -> Cat perception -> Investigate -> New behavior`

Examples:

- throwing a glass,
- breaking an object,
- operating a loud appliance,
- intentionally making noise to misdirect the cat.

This allows physical props to become tactical tools rather than decorative physics toys.

---

## 10. Smart-Home & Sabotage Systems

Handwritten notes list sabotage concepts including:

- stove sabotage,
- online ordering through a phone mechanic / mini-game,
- cat-toilet-related absurd police/FBI event,
- lighting/electric sabotage,
- general smart-house sabotage.

Earlier GDD direction also frames smart-home control as one of the game’s strongest identities.

### Sabotage design principles

A sabotage should ideally:

1. change the player’s current priority,
2. require a meaningful response,
3. reveal personality/intelligence of the cat,
4. interact with existing household systems,
5. create a memorable or funny consequence.

Avoid sabotage that is only a popup message with no gameplay consequence.

---

## 11. Phone / Camera Mechanics

Notes mention:

- a phone mechanic,
- room/camera viewing,
- security camera concepts,
- ordering / mini-game use.

The phone should eventually be more than a menu.

Potential functions already supported by the concept:

- checking rooms / security feeds,
- receiving suspicious notifications,
- dealing with cat-created online orders,
- interacting with smart-home controls,
- triggering or solving mini-game-like events.

Exact UI and feature scope remain open.

---

## 12. Laboratory, Magic & Transformations

This is an important later-game expansion of the cat fantasy.

### Confirmed current creative direction

In later levels/days:

- the cat is effectively a wizard / occult experimenter,
- the cat has a laboratory,
- the cat creates unusual substances/devices/experiments,
- the cat can transform into other animals or creature-like forms.

### Transformation rule

A new form should ideally introduce a **new gameplay rule**, not merely a cosmetic model swap.

Examples discussed conceptually:

- small/rodent-like form: access to tight spaces,
- dog-like form: faster/aggressive pressure,
- bird/owl-like form: vertical/high-position threat,
- spider-like form: walls/ceilings,
- deceptively harmless animal form: misdirection.

These exact forms are examples, not final content commitments.

### Reveal philosophy

Do not expose the full supernatural concept immediately.

Prefer gradual foreshadowing:

- strange noises,
- locked/hidden room,
- unusual light or smoke,
- unexplained objects,
- evidence of experiments,
- eventual laboratory reveal.

The laboratory can become the lore and escalation center of the second half of the game.

---

## 13. Comedy / Horror Language

The project should sit between:

- tension,
- absurdity,
- domestic comedy,
- occasional jumpscares,
- weird/occult escalation.

### Existing note example

“Reverse cucumber prank / jumpscare” captures the intended tone well: take a familiar cat joke and reverse it so the cat becomes the one manipulating the player.

### Streamer-friendly philosophy

Good moments should be easy to understand visually and verbally:

- “Why did the cat order this?”
- “Why did the lights just go out?”
- “Why is it watching me through the camera?”
- “What is happening in that laboratory?”
- “That is not even a cat anymore.”

Avoid relying entirely on long exposition or hidden lore text.

---

## 14. Art Direction

There are two source directions that should currently be treated as a **working synthesis, not a fully locked art bible**.

### Handwritten direction

- gothic but edgy,
- a generally orderly house,
- occasional deliberately strange/outlandish object,
- wooden furniture,
- warm/yellow and dark tones,
- Scandinavian influence.

### Earlier GDD direction

- modern,
- realistic,
- dark / liminal,
- performance-conscious lighting.

### Current working synthesis

A believable modern home with warm wood / Scandinavian foundations, gradually pushed toward darker, gothic, liminal, and uncanny presentation as the game escalates.

This is still an open art-direction decision and should not be treated as locked without designer approval.

---

## 15. Environment Interaction Philosophy

Interactable environment systems are foundational.

Important categories:

- doors,
- light switches,
- pickup/throw objects,
- breakable/noisy props,
- phone/cameras,
- appliances,
- smart-home devices,
- cat-specific sabotage targets.

### Rule

When possible, build reusable generic interaction foundations and let environment-specific objects use them.

Do not create a unique input/raycast system for every object type.

---

## 16. Current Technical Foundation

### Engine / project

- Unity 6.3 LTS
- URP
- Windows PC target
- New Input System
- Unity MCP integrated for editor inspection/wiring/validation

### Existing project organization

Core project folders live under:

`Assets/_Project/`

Major categories include:

- Art
- Audio
- Prefabs
- Scenes
- Scripts
- UI
- Settings

### Existing gameplay foundation

Implemented core systems currently include:

- `PlayerMovement`
- `PlayerLook`
- `IInteractable`
- `PlayerInteraction`
- `InteractionPromptUI`
- `PrototypeInteractable`
- `PlayerPickup`
- `PickupItem`

### Current interaction model

The interaction architecture is generic:

`Player -> center raycast -> IInteractable -> Interact(interactor)`

Do not introduce duplicate interaction raycasts unless a future task explicitly requires a separate system for a justified reason.

### Current pickup foundation

Generic physics pickup/drop/throw exists:

- E: pick up / drop
- LMB through `Player/Throw`: throw
- HoldPoint under FPS camera
- Rigidbody state preservation/restoration
- player/object collision isolation while held
- safe disable/play-mode cleanup

This is a **generic foundation**, not a finished glass mechanic.

Ece’s environment glass work should reuse `PickupItem` rather than rebuilding pickup from scratch.

---

## 17. Prototype Scene

Current prototype scene:

`Assets/_Project/Scenes/Prototype/Prototype_Player.unity`

It has been used to validate:

- first-person movement,
- look,
- interaction,
- prompt UI,
- pickup/drop/throw,
- simple physics test objects.

Prototype objects are temporary test infrastructure. Do not mistake Cube1/Cube2/Cube3 for final game content.

---

## 18. Team & First-Week Ownership

### Emre — Core / Integration Lead

- `CORE-001 Project Setup` — complete
- `CORE-002 Player Controller` — complete
- `CORE-003 Interaction System` — complete
- Generic pickup/drop/throw foundation — complete, created as shared infrastructure
- `CORE-004 Integration Build` — pending team PRs
- `CORE-005 Code Review` — pending team PRs

`CORE-004 Integration Build` and `CORE-005 Code Review` are primarily integration/review tasks, not new-feature tasks. Small glue fixes/refactors may happen if integration requires them.

### Aleyna — Cat AI

First-week plan:

- `CAT-001 Navigation`
- `CAT-002 Patrol State`
- `CAT-003 Investigate State`
- `CAT-004 Sound Perception`
- `CAT-005 Basic Hack State`

### Ece — Environment / Interactables

First-week plan:

- `ENV-001 House Greybox`
- `ENV-002 Door Interaction`
- `ENV-003 Light Switch`
- `ENV-004 Pickable / Throwable Glass`
- `ENV-005 Sound Emitter Integration`

Important: generic pickup/drop/throw now exists in core. `ENV-004` should integrate the actual glass object with the shared `PickupItem` foundation rather than implementing a second pickup system.

### Berkay — Design / Visual Direction

Flexible design responsibilities discussed:

- Psycho Cat character visual direction,
- art-direction / moodboard,
- salon/kitchen/environment references,
- PC interaction/UI language,
- optional diegetic phone/tablet UI direction.

The goal is to establish the game’s visual feel, not necessarily deliver final production assets immediately.

---

## 19. Git / Integration Workflow

Current intended team flow:

- `main` = stable/release-oriented branch
- `develop` = team integration branch
- Emre feature branches for core work
- `feature/cat-ai` = Aleyna
- `feature/environment` = Ece

Team PRs should target **`develop`**, not `main`.

When opening PRs, include:

- completed task IDs,
- scenes changed,
- important GameObjects/components added,
- known limitations,
- manual test steps,
- confirmation of Console errors/warnings.

Scene changes must be called out explicitly because Unity YAML merges can conflict.

---

## 20. Codex Development Rules

When Codex works on Psycho Cat:

### Always inspect before changing

- read `AGENTS.md`,
- read this document,
- inspect current branch/status,
- inspect existing implementation,
- inspect Input Actions before adding new actions,
- inspect scene via unityMCP before wiring anything.

### Prefer extension over duplication

Before creating a new system, check whether an existing system already solves part of the problem.

Examples:

- use `IInteractable` instead of a second interaction raycast,
- use `PickupItem` instead of a second pickup implementation,
- reuse existing input actions when semantically correct,
- do not hijack an unrelated action if its extra bindings create wrong behavior.

### Keep task scope narrow

Do not implement future ideas “while here.”

Examples of ideas that should only be added when explicitly tasked:

- inventory,
- crafting,
- full needs simulation,
- advanced transformation system,
- final phone UI,
- full smart-home framework,
- final save system.

### Preserve prototype stability

- do not modify unrelated scenes,
- do not touch Packages/ProjectSettings/URP unless task requires it,
- avoid unnecessary scene hierarchy changes,
- validate Console after compile,
- validate Play Mode,
- do not claim input was tested if MCP cannot actually simulate it.

### Git discipline

Unless explicitly instructed:

- do not commit,
- do not push,
- do not merge,
- report `git status` / `git diff` at the end.

---

## 21. Scope Guardrails

The project should resist premature “feature explosion.”

Do not turn Psycho Cat into a giant life simulator or a feature-heavy sandbox before the core fantasy works.

The near-term goal is to prove:

1. moving around the house feels good,
2. environmental interaction feels clear,
3. Cat AI feels alive and reactive,
4. sound / object interaction can manipulate the cat,
5. the cat can sabotage the environment,
6. one short sequence can deliver the game’s comedy/horror identity.

A strong 15–20 minute vertical slice is more valuable than many disconnected systems.

---

## 22. Current Open Design Decisions

These are intentionally **not locked** and should not be silently decided by Codex:

- exact number of days (5, 7, or another structure),
- exact final art style balance between realistic/modern and gothic/edgy,
- exact player-need set (Hunger/Sleep versus adding Water/Stamina),
- final cat animation style,
- final interaction UI style (diegetic vs screen-space),
- exact house size/layout,
- final lighting strategy,
- final phone/camera UX,
- exact save/checkpoint/game-over model,
- exact taming/end-state mechanics,
- exact laboratory reveal timing,
- exact transformation forms,
- final adaptive-difficulty rules,
- final daily task/failure rules,
- final open-ended ending structure.

When a task depends on one of these, Codex should ask/report rather than assume.

---

## 23. Vertical Slice Target

A convincing first vertical slice should eventually demonstrate a chain similar to:

1. Player wakes/starts in a believable home.
2. Player has a simple household objective.
3. Cat patrols naturally.
4. Player interacts with a door/light/object.
5. Player throws or breaks a noisy prop.
6. Cat hears the event and investigates.
7. Cat performs or begins a simple sabotage.
8. Player responds using the environment.
9. A memorable comedy/horror beat occurs.
10. The sequence ends without requiring the entire 5–7 day game to exist.

This is the most important proof that the core systems belong to the same game.

---

## 24. Product North Star

When evaluating a new mechanic, ask:

> **Does this make the player feel like they are trying to survive normal life inside a house controlled by an increasingly impossible cat?**

If the answer is no, the feature may not belong in the core experience.

Psycho Cat should progress from domestic absurdity to technological sabotage to occult/laboratory chaos without losing the simple central relationship:

> **One human, one house, one cat that should not be this intelligent.**
