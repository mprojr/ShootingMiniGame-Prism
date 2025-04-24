# Project Context: ShootingMiniGame-Prism

## Project Overview
- Type: Unity VR Shooting Game
- Start Date: April 24, 2025
- Last Updated: April 24, 2025
- Unity Version: 6000.0.36f1
- Description: A fast-paced, action-packed VR mini-game where players engage in shooting challenges using various weapons across different levels.

## Team Members
- Ayyan - [GitHub](https://github.com/ayyan67)
- Alifyah Hussain - [GitHub](https://github.com/alifyah-m)
- Jason Nguyen - [GitHub](https://github.com/JasonNguyen47)
- Danyal Khan - [GitHub](https://github.com/DanyalKhan21)
- Kevin Le - [GitHub](https://github.com/keb-web)
- Martin Rodriguez - [GitHub](https://github.com/mprojr)

## Current State
- VR-based shooter game with Oculus integration
- Multiple levels/scenes (Kitchen, Basement, Garage, Living Room)
- Weapon mechanics implemented with different gun types
- Game management system with health, timer, and difficulty progression
- Perk and ability system for gameplay variety
- Menu system for game navigation

### Core Game Mechanics
1. **Player Movement**: VR-based movement using Oculus headset
2. **Shooting**: Different gun types (one-handed, two-handed) with customizable fire rates and bullet sizes
3. **Health System**: Player has health that decreases when taking damage
4. **Timer System**: 95-second rounds with progression between levels
5. **Perks/Abilities**:
   - Passive perks: Increased attack speed, larger bullets, decreased wall dot speed
   - Active abilities: Invincibility, double fire rate, slow all wall dots

## Recent Changes
- Initial context file creation

## Project Structure
- **Assets/Scripts**: Core gameplay scripts
  - **Guns&Shooting**: Weapon mechanics
  - **Health**: Health system
  - **UI**: User interface elements
  - **Walls**: Environment mechanics
- **Assets/Scenes**: Different game levels and menus
- **Assets/Materials**: Visual materials
- **Assets/Models**: 3D models
- **Assets/Prefabs**: Reusable game objects

## Next Steps
1. Review full gameplay loop from start to finish
2. Test VR controls and interactions
3. Ensure proper scene transitions and game progression
4. Review game balance (difficulty, health, weapons)
5. Check performance optimization for VR
6. Implement any missing UI elements or prompts
7. Test cross-device compatibility (different VR headsets)

## Important Files
- **/Assets/Scripts/GameManager.cs**: Core game management
- **/Assets/Scripts/PlayerMovement.cs**: Player control and movement
- **/Assets/Scripts/Guns&Shooting/LeftOneHandGun.cs**: Left-handed weapon mechanics
- **/Assets/Scripts/Guns&Shooting/PlayerAbility.cs**: Player abilities
- **/Assets/Scenes/*.unity**: Game levels and menus

## Technical Notes
- Uses Unity Universal Render Pipeline (URP)
- Oculus integration for VR functionality
- OVRInput for controller input handling
- Scene management for level progression

## Changes Log
- 2025-04-24: Initial project analysis and context file creation
- 2025-04-24: Modified WallDotMovement.cs to make spider models face the player at all times
- 2025-04-24: Updated WallDotMovement.cs to make the original WallDot invisible while keeping the spider model visible
