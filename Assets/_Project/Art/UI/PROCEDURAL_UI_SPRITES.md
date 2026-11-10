# Mia & Max — Procedural UI Sprite Pack

Generate the first cohesive sprite set from Unity:

**Tools → Mia & Max → UI → Generate Original UI Sprite Pack**

The generator writes ten original PNG sprites to `Assets/Resources/MiaMax/UI`:
- Rounded panel and button surfaces with 9-slice borders
- A badge surface
- Coin, seed, harvest, star, home, journal and map icons

The mobile HUD uses the rounded panel/button surfaces and adds star, journal, map, coin and harvest icons to its primary controls. Reusable UI cards use the same rounded panel surface. If the resources are absent, the existing color-only UI remains as a safe fallback.

These are original vector-style procedural graphics, not hand-illustrated final brand art. They still require visual review at phone/tablet sizes, color/contrast review, safe-area checks and device performance testing.
