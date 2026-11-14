# Mia & Max — Project Cleanup and Environment Production Audit

Audit date: 2026-11-14  
Audited branch: `codex/mia-max-foundation`  
Audited commit: `8ab0a116eed174bb2355b5a73ee3d1ac54e15b1c`

## Executive findings

The repository snapshot contains source code and project configuration, but it does not currently contain the imported environment model files or generated runtime prefab assets.

| Check | Repository snapshot |
|---|---:|
| Tracked files and tree entries | 802 |
| Editor C# tools under `Assets/_Project/Scripts/Editor` | 35 |
| Tracked Unity prefab assets (`.prefab`) | 0 |
| Tracked model source files (`.fbx`, `.obj`, `.glb`, `.gltf`) | 0 |
| Tracked Unity scenes | 2 |
| Main gameplay scene | `Assets/_Project/Scenes/Backyard.unity` |
| Additional scene | `Assets/_Project/Scenes/UntitledBackup.unity` |

The imported Kenney model directory is not present in this Git tree. The user's Unity workspace may still contain locally imported models; this must be checked by Unity Editor tools after pulling the branch.

## Cleanup policy

Do not delete Editor scripts merely because they live under a Tools/Editor folder. Code under an Editor-only assembly/folder is excluded from player builds and is not runtime payload. The main cleanup targets are duplicate workflows, obsolete preview generators, redundant menu entries, stale generated review scenes, and unused source assets.

Before deleting any script:
1. Search for calls, type references, menu workflows, and serialized references.
2. Check whether it is the only way to reproduce a required asset.
3. Preserve a replacement workflow for any asset-generation function.
4. Remove the script and its matching `.meta` file together.
5. Recompile in Unity and run the relevant read-only audits.

## Editor tool inventory and initial disposition

### Keep as production or project maintenance tools

- `BackyardSceneSetup.cs` — creates/registers the playable Backyard scene.
- `BackyardContentCatalogBuilder.cs` — creates the runtime content catalog.
- `BackyardProductionModelGenerator.cs` — creates the current 15-asset procedural production candidate set; its own report explicitly says visual approval and gameplay integration are still pending.
- `BackyardProductionPrefabAudit.cs` — checks mesh, UV, renderer, material, texture, triangle and LOD requirements.
- `CharacterAssetPipeline.cs` and `CharacterAnimatorStateMachineBuilder.cs` — character asset/animation setup.
- `ProceduralAudioAssetGenerator.cs` — creates project audio assets.
- `ProceduralUiArtGenerator.cs` — creates UI art assets.
- `WorldTextureLibraryBuilder.cs` — creates the reusable world texture library.
- `KenneyFreePackImporter.cs` and `FreeEnvironmentAssetImporter.cs` — import source art packs.
- `ReleaseCandidateWorkflow.cs`, `ReleaseIdentitySetup.cs`, `UrpProjectSetup.cs` — project/release setup workflows.

### Keep during art review; consolidate later

- `BackyardEnvironmentCandidateReviewBuilder.cs`
- `CuratedBackyardEnvironmentSetBuilder.cs`
- `FreeEnvironmentPreviewBuilder.cs`
- `FreeEnvironmentMaterialShowcaseBuilder.cs`
- `BackyardProductionLayoutPreviewBuilder.cs`
- `CharacterProductionReviewSceneBuilder.cs`
- `BackyardPlaceholderGalleryBuilder.cs`
- `BackyardPlaceholderLayoutPreviewBuilder.cs`

These are review-only utilities. They should not be shipped as runtime content, but are useful until the final art set is approved and reproducible.

### Keep as diagnostics until the final validation pass

- `AudioAssetAudit.cs`
- `BackyardPlaceholderPrefabAudit.cs`
- `EditorSmokeChecks.cs`
- `FreeEnvironmentAssetInventory.cs`
- `FreeEnvironmentMaterialDiagnostics.cs`
- `GameplayReleaseAudit.cs`
- `MissingScriptReferenceAudit.cs`
- `ProductionAssetAudit.cs`
- `ReleaseReadinessAudit.cs`
- `UiMigrationAudit.cs`
- `VoiceAssetAudit.cs`

### Review for consolidation after dependencies are checked

- `BackyardEnvironmentAssetPipeline.cs`
- `BackyardPlaceholderPrefabGenerator.cs`

Do not delete these until their responsibilities and callers have been checked against the complete project and local Unity assets.

## Current environment production blockers

1. No final environment prefab assets are tracked in this branch.
2. No imported model source files are tracked in this branch.
3. `BackyardProductionModelGenerator` creates a separate procedural candidate set; it does not convert the selected Kenney source models into final production assets.
4. `CuratedBackyardEnvironmentSetBuilder` creates a review scene only. It does not create production prefabs or modify the gameplay scene.
5. `BackyardPrototypeBootstrap` still creates the temporary blockout at runtime. Replacing it safely requires a prefab-backed environment integration path while preserving the existing gameplay interaction roots and service components.
6. The production acceptance brief requires visual review, mobile budgets, provenance/license notes, and owner approval. Structural audit success alone is not visual approval.

## Planned implementation order

### Phase A — Safe project cleanup
- Group editor menus under a consistent Development/Art Pipeline/Validation hierarchy.
- Keep required generators and audits.
- Remove only proven duplicate or obsolete preview tools, with matching metadata.
- Keep imported third-party source packs separate from optimized runtime assets.

### Phase B — Production asset preparation
- Scan the user's imported local model folders.
- Resolve the approved source models by exact asset name.
- Normalize scale, pivots, normals, materials and texture import settings.
- Build clean visual prefabs without gameplay scripts or gameplay colliders.
- Add simple colliders only to separate gameplay roots where needed.
- Record source pack, license, dimensions, triangle counts, texture sizes and review status.

### Phase C — Cohesive art direction
- Use warm cream, natural wood, warm white, muted greens, earthy soil and warm sand.
- Normalize roughness/smoothness and material response for URP/Lit.
- Review silhouettes, scale, density, lighting and camera readability together in a dedicated review scene.
- Profile renderer count, triangles, texture memory and overdraw on representative mobile hardware before release.

### Phase D — Runtime integration
- Replace temporary visual blockout objects with final prefab references.
- Preserve stable gameplay roots, interaction components, farming/production/quest systems and player navigation.
- Keep review scenes out of Build Settings.
- Run compile, missing-script, prefab, scene and release audits in Unity.
- Do not mark the environment production-approved until the visual review and device checks pass.

## Safety boundary

This audit does not delete project files, change gameplay scenes, replace production assets, or modify imported source models. The next implementation pass must use the actual local Unity asset inventory; a Git snapshot alone cannot prove that locally imported model files are absent from the user's machine.
