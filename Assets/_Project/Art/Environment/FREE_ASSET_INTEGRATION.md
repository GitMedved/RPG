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

## 2. Additional compatible sources and Unity importers

**Kenney — Nature Kit**
- Official page: https://kenney.nl/assets/nature-kit
- 330 3D files; CC0; trees, rocks and foliage.
- Official archive URL used by importer: https://kenney.nl/media/pages/assets/nature-kit/37ac38a37b-1677698939/kenney_nature-kit.zip
- Unity menu: Tools > Mia & Max > Assets > Kenney > Download Nature Kit (CC0)
- Destination: Assets/_Project/Art/Environment/Runtime/Models/Kenney/NatureKit/

**Kenney — Furniture Kit**
- Official page: https://kenney.nl/assets/furniture-kit
- 140 3D files; CC0. Primarily interior furniture; use selectively for indoor rooms and workshop interiors, not as the default patio set.
- Unity menu: Tools > Mia & Max > Assets > Kenney > Download Furniture Kit (CC0)
- Destination: Assets/_Project/Art/Environment/Runtime/Models/Kenney/FurnitureKit/

**Kenney — Building Kit**
- Official page: https://kenney.nl/assets/building-kit
- 80 3D files; CC0; modular building pieces suitable for testing house/workshop construction.
- Unity menu: Tools > Mia & Max > Assets > Kenney > Download Building Kit (CC0)
- Destination: Assets/_Project/Art/Environment/Runtime/Models/Kenney/BuildingKit/

The Kenney importer downloads archives to Library, checks that a ZIP can be opened, rejects path traversal, and refuses to overwrite a non-empty destination. The furniture/building download actions use Kenney's official per-pack download endpoint. If Kenney changes its endpoint, the download will fail visibly rather than silently importing an HTML page. The project manifest now includes Unity glTFast 6.20.0 so Unity can import GLB/glTF source files; Unity Package Manager must finish resolving it before GLB import can be assessed.

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

The importers retrieve source archives into the Unity project when the user runs their menu commands. No third-party binary assets are committed by the code change itself. No existing scene or prefab is replaced. After downloading, run Tools > Mia & Max > Assets > Run Free Environment Asset Inventory. Imported models still need visual review, URP material setup, collision/LOD preparation and mobile profiling before they can replace placeholders.


## 5. Safe visual review scene

After downloading one or more packs and waiting for Unity to finish importing, run:

- Unity menu: **Tools > Mia & Max > Assets > Build Free Environment Review Scene**
- Output: `Assets/_Project/Scenes/ArtReview/FreeEnvironmentPreview.unity`

The editor utility scans the environment model source folder for FBX, OBJ, GLB and glTF files, then places loadable models on a neutral review grid with a camera, lights and labels. It normalizes preview instances for side-by-side inspection. It creates a separate additive review scene and restores the previously active scene; it does not edit or replace the backyard scene, source models, prefabs, or production approval flags. If the review scene already exists, it refuses to overwrite it. Delete or rename that review scene manually only when you intentionally want to regenerate it.

This is a visual inspection aid, not an automated art-quality or mobile-performance approval. Some imported formats may not load as GameObjects until their importer/package is configured. Review silhouettes, scale, style consistency, materials, texture resolution, geometry and performance before selecting assets for production integration.
