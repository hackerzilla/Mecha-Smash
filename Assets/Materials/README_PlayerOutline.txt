Player Outline Material Setup Instructions
==========================================

Since Unity materials are binary assets that must be created in the Unity Editor, please follow these steps:

1. In Unity Editor, navigate to Assets/Materials/ folder
2. Right-click → Create → Material
3. Name it "PlayerOutline"
4. In the Inspector:
   - Set Shader to "Custom/SpriteOutline"
   - Set Outline Color to white (1, 1, 1, 1) - this will be overridden per-player at runtime
   - Set Outline Width to 1-2 (adjust as desired)

5. Assign this material to the MechController prefab's "Outline Material Template" field

The material color will be set dynamically per-player using material instances, so the default color here doesn't matter.
