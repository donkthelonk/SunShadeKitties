# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

**SunShade Kitties** — a Unity 2D platformer where the player controls a kitty that switches between a **Sun (light)** and **Shade (dark)** state. Each state has different interactions with light and dark elements in the world.

- **GitHub:** https://github.com/donkthelonk/SunShadeKitties
- **Unity version:** 6000.3.11f1 (Unity 6)
- **Render pipeline:** Universal Render Pipeline (URP) 2D
- **Input system:** Unity Input System (new, not legacy)

## Running the Project

Open in Unity Hub by adding the project folder. Play from within the Unity Editor — there is no CLI build command for day-to-day development.

## Architecture

### Input

Input bindings live in `Assets/InputSystem_Actions.inputactions`. The generated C# class `InputSystem_Actions` is used directly in scripts (not via `PlayerInput` component). When adding new actions, add them to the `.inputactions` file first, then reference them in scripts via `inputActions.Player.<ActionName>`.

### Sun/Shade State System

The core mechanic is the `KittyState` enum (`Sun` / `Shade`). State is owned by `PlayerController` and broadcast via a static `PlayerController.OnStateChanged` event. Other systems (lighting, interactables, visuals) subscribe to this event rather than polling the player's state directly.

### Scripts

All game scripts live in `Assets/Scripts/`. No editor scripts exist yet.

### State Interactables

`StateObject` — attach to any platform or world object to make it only solid/visible in one kitty state. Set `activeInState` to `Sun` or `Shade`. When inactive, the collider is disabled and the sprite fades to `inactiveAlpha` (default 25%) so the player can see it exists but pass through it.

### Scene Setup Notes

- The `groundCheck` Transform must be positioned at the player's **feet** (e.g. Y = -0.5), not the center. The ground layer mask on `PlayerController` must match the layer assigned to ground/platform objects.
- The Player object must **not** be on any layer included in the `groundLayer` mask — otherwise `OverlapCircle` detects the player's own collider and `isGrounded` is always true.
- Use a dedicated `Platform` layer for Sun/Shade platforms (not `Default`). Include both `Ground` and `Platform` in the `groundLayer` mask.

### Jump Feel

`PlayerController` has coyote time and jump buffer, both tunable in the Inspector under **Jump Feel**. Defaults: 0.15s each. After jumping, a `jumpCooldown` (0.2s) prevents the coyote timer from resetting until the player has physically left the ground.

### Unity 6 API Notes

- Use `Rigidbody2D.linearVelocity` (not `.velocity` — renamed in Unity 6)
- Use `Physics2D.OverlapCircle` for ground checks
- URP 2D lighting: `Light2D` components control world lighting; the Sun/Shade state should drive which lights are active or what intensity they use
