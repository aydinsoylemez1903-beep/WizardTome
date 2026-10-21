# WizardTome — 3D Wizard Prototype

An original stylized-fantasy third-person wizard combat prototype for **Unity 6.3 LTS**.

## Quick start
1. Install Unity Hub and Unity Editor **6000.3.26f1** (or another Unity 6.3 LTS editor).
2. Clone/download this repository and open its root folder via **Unity Hub > Add project from disk**.
3. In the Unity Editor choose **WizardTome > Create Prototype Scene**.
4. Open `Assets/Scenes/Prototype.unity` if it isn't already open, then press **Play**.

The editor menu creates a saved scene. All prototype models are generated from Unity primitives when the game starts—no external assets are required.

## Controls (keyboard + mouse)
- **WASD / Arrow keys:** move relative to the camera
- **Mouse:** rotate camera (hold right mouse button)
- **Shift:** sprint
- **Space:** jump
- **Left mouse / F:** cast an arcane projectile
- **R:** reset character position

## MVP features
- Third-person orbit/follow camera
- CharacterController locomotion + gravity + jump
- Magic projectile with cooldown, mana cost and recharge
- Three stationary training golems with hit points and respawn
- Stylized training island, towers, crystal props, trees and lighting
- Minimal on-screen HUD and instructions

## Scope & limitations
This is a **source-only prototype**. The character and environments are intentionally primitive placeholder geometry. It has not yet been compiled or play-tested inside Unity here. Mobile touch controls, imported character models, animation clips, sound and production UI come later.

## Folder structure
- `Assets/Scripts/WizardTomeBootstrap.cs` creates the runtime test world.
- `Assets/Editor/WizardTomeSceneCreator.cs` provides the scene creation menu.
- `ProjectSettings/ProjectVersion.txt` records editor version.
- `Packages/manifest.json` uses Unity default built-in packages.

## GitHub
https://github.com/aydinsoylemez1903-beep/WizardTome
