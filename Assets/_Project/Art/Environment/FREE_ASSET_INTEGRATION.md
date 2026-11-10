# Mia & Max — Free Environment Asset Integration

Status: curated sources selected; one pack has a one-click Unity Editor downloader. Imported assets are **not** automatically production-approved.

## 1. Stylized nature — first import

**Quaternius — Stylized Nature MegaKit (standard/free subset)**
- Source page: https://quaternius.com/packs/stylizednaturemegakit.html
- Verified downloadable archive mirror: https://opengameart.org/content/stylized-nature-megakit
- Direct archive used by the Editor importer: https://opengameart.org/sites/default/files/stylized_nature_megakitstandard.zip
- License: CC0 1.0; commercial use allowed; attribution not required.
- Pack page describes 116 models in the full kit; the OpenGameArt standard archive contains 68 models. Textured FBX/OBJ/Blend/glTF formats are advertised by the publisher.
- Intended use: trees, shrubs, flowers, grass, rocks and garden dressing for the backyard and later woodland areas.
- Unity menu: Tools > Mia & Max > Assets > Download Quaternius Stylized Nature (CC0)
- Import destination: Assets/_Project/Art/Environment/Runtime/Models/QuaterniusStylizedNature/
- Download archive is stored temporarily under Library and removed after successful extraction.

The importer does not alter scenes, existing placeholder prefabs, runtime bootstrap code, or production approval flags. Unity must finish importing before assets can be inspected. Prefabs, URP materials, colliders, LODs, lightmap/static settings, atlasing and mobile profiling still need a separate art-integration pass.

## 2. Additional compatible sources

**Kenney — Nature Kit**
- https://kenney.nl/assets/nature-kit
- 330 3D files; CC0; trees, rocks and foliage.
- Use as a secondary source only where it fills gaps without creating a visibly inconsistent art style.

**Poly Haven — models**
- https://polyhaven.com/models
- https://polyhaven.com/license
- CC0; commercial use and redistribution permitted. Prefer selected low-poly/clean props only if they match the game's stylized look and mobile budget.

**ambientCG — PBR textures**
- https://ambientcg.com/
- https://docs.ambientcg.com/license/
- CC0; commercial use and inclusion of raw files in a game project permitted.
- Suitable texture families to source for the house/patio/fence/path: plaster or painted wall, wood planks, paving stones, concrete, soil and grass. Choose 1K or 2K maps for mobile; avoid importing 8K source maps into runtime.

## 3. License and production controls

- Keep the source URL and license recorded for every pack and individually downloaded asset.
- Do not assume that all assets on a marketplace share one license; verify the exact pack page and included license file.
- Do not upload the downloaded packs as a standalone asset library or redistribute the raw assets separately from Mia & Max.
- CC0 source status does not mean the asset is visually approved, optimized, safe for mobile, or ready for release.
- Before promotion to production: inspect mesh scale/orientation, normals, UVs, texture maps, material setup, polygon count, bounds, colliders, LODs, overdraw and draw calls on target devices.
- Existing Mia and Max character identity references remain authoritative. Do not replace either hero with generic stock characters.

## 4. Current limitations

The first importer retrieves the Quaternius nature archive only. The other sources above are curated candidates, not yet downloaded into the repository. No third-party binary assets are committed by this change, and no existing scene or prefab is replaced.
