MechSilhouetteOutline Material Setup
=====================================

This outline system renders each mech to a RenderTexture then applies an
outline shader to the composite silhouette, solving:
1. Outline extending beyond sprite bounds
2. Unified silhouette (no overlapping outlines on individual sprites)
3. Per-player outline isolation (mechs don't affect each other's outlines)

STEP 1: Create Layers (Edit > Project Settings > Tags and Layers)
------------------------------------------------------------------
Create per-player sprite layers (for outline camera isolation):
- Add User Layer 8: "MechSprites1"
- Add User Layer 9: "MechSprites2"
- Add User Layer 10: "MechSprites3"
- Add User Layer 11: "MechSprites4"
- Add User Layer 12: "MechOutline"

Add Sorting Layer:
- Add Sorting Layer: "MechOutline" (between Ground and Player)

STEP 2: Create Material
-----------------------
1. Right-click in Assets/Materials folder
2. Create > Material
3. Name it "MechSilhouetteOutline"
4. Set Shader to: Custom/MechSilhouetteOutline
5. Default properties:
   - Outline Color: White (1,1,1,1)
   - Outline Width: 4

STEP 3: Configure Main Camera
-----------------------------
1. Select Main Camera in your game scene
2. In Camera component, find Culling Mask
3. CHECK all per-player sprite layers:
   - MechSprites1
   - MechSprites2
   - MechSprites3
   - MechSprites4
4. CHECK "MechOutline" layer
5. Keep all other layers as normal

NOTE: Main camera sees ALL sprite layers and the outline layer.
Each player's outline camera only sees its own sprite layer.

STEP 4: Configure Mech Prefab (TestMech or your mech prefab)
------------------------------------------------------------
1. Open mech prefab
2. Add "MechOutlineRenderer" component to root GameObject
3. Configure MechOutlineRenderer:
   - Outline Material: Drag "MechSilhouetteOutline" material
   - Outline Width: 4
   - Outline Color: White (set per-player at runtime)
   - Render Texture Size: 512
   - Quad Scale: 5 (adjust to fit your mech size)
   - Mech Outline Layer Name: "MechOutline"

NOTE: Player index is set automatically by PlayerController at runtime.
Each player's sprites go on their own layer (MechSprites1, MechSprites2, etc.)

STEP 5: Test
------------
1. Play the game with 2+ players
2. Each mech should have a unified outline around its silhouette
3. Outline should extend beyond sprite bounds
4. When mechs overlap, their outlines should NOT affect each other
5. Each player should have their own unique outline color

TROUBLESHOOTING
---------------
- Outline not visible: Check Main Camera culling mask INCLUDES MechOutline layer
- Sprites invisible: Check Main Camera culling mask INCLUDES MechSprites1-4 layers
- Interior is opaque: Make sure shader returns transparent for sprite pixels
- Outline color wrong: SetOutlineColor() is called from PlayerController
- Outline affects other players: Make sure MechSprites1-4 layers exist
- "Layer not found" warning: Create all 4 MechSprites layers in Tags and Layers
