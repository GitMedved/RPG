# Mia & Max runtime artwork

## Backyard scene image

The tap-first prototype loads the illustrated backyard as a full-screen image:

`Assets/Resources/MiaMax/Art/BackyardScene.png`

Unity runtime key:

`Resources.Load<Texture2D>("MiaMax/Art/BackyardScene")`

Use the generated portrait 9:16 backyard illustration from the project conversation. Import settings:
- Texture Type: Default
- sRGB: On
- Alpha Source: None
- Max Size: 2048
- Compression: Normal or High Quality

The runtime draws this image as the world backdrop and uses screen-space tap hotspots. Do not add the old procedural cube/sphere blockout back into the startup flow.

## Important asset status

The generated illustration is not stored in Git by this README. It must be added as the PNG file above. The repository currently contains Mia and Max reference images, but not production 3D FBX character models or animation clips. Do not treat reference images or primitive proxies as final character assets.
