# Magicthirst -- Green

itch here: https://yojick.itch.io/magicthirst

📁: _Code is in `Assets/Src/`_

## Gameplay

- Player fights groups of enemies in arenas
- Uses melee + abilities (dash, parry, teleport, magic casts, shots)
- Enemies use the same systems (shared ability pipeline)
- Combat is based on timing (parry window, direction checks)

## Systems

### Intent → Impact System

_Located in `Levels/IntentsImpacts/`._

<details>
  <summary>Short: `Action → Intent → Impacts → Modified → Applied`</summary>

Full: `Caster → Intent → Mapper → Impacts → [Broker] → [Modifiers] → Victim`

</details>

Intents — player/AI actions (shoot, parry, teleport)  
Impacts — actual effects (damage, impulse, teleport)

This allows:
- reuse of abilities between player and AI
- easy addition of new mechanics
- interception (parry, status effects)

📁 Main files to look at:
- `Levels/IntentsImpacts/IntentsImpacts.cs`
- `Levels/IntentsImpacts/DeferredBroker.cs`
- All abilities are in `Levels/Abilities/`

### Entities and their Components

_Located in `Levels/Entities/`._ 

// Currently works on ScriptableObjects, may be migrated to just `[Serializable]`s

📁:
- `Levels/Core/Entity.cs` – Composition root for all gameplay objects
- `Levels/Core/CoreObject.cs` – Base for injectable, disposable components
- Key components:
    - `Health.cs`
    - `Weaponry.cs`
    - `TeleportChip.cs`
    - `ProjectilesParrying.cs` (advanced parry system with timing window + direction check)
    - `StatusesRepository.cs` (status effects + modifiers)

### AI System

_Located in `Levels/AI/Fsm.cs`._

- `Fsm.cs` + `FsmState.cs` — finite state machine
- States for bandits (`BanditIdle`, `BanditAlerted`, `BanditFighting`) and turrets
- Includes editor tool that generates PlantUML diagrams

## DI

_Located in `Levels/DI/`._

- `DI/GameLifetimeScope.cs` – Root scope
- `DI/LevelLifetimeScope.cs` – Per-level scope
- `DI/EntityContextScope.cs` – Per-entity scope
- `DI/ConsumerLifetimeScope.cs` & `DI/ProducerLifetimeScope.cs` – TODO, Separate handling for local player input vs remote players

Wires everything above.

### Multiplayer to be done somewhere in the far-far future

## Glossary to be done
