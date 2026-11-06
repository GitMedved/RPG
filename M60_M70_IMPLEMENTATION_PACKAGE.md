# M60–M70 — UI, gameplay content and character production foundations

**Branch:** `codex/mia-max-foundation`  
**Scope:** engineering foundations for the next release phase. These milestones are source-level implementation, not Unity acceptance.

## M60 — Visual hierarchy, cards and screen states
- Added `MobileUiComponents`: shared card surfaces, consistent heading/body labels, responsive text sizing and reusable state-message panels.
- Supports honest loading and empty states; callers provide the actual state text. No fabricated percentage or timer is shown.
- Existing `MobileUiTheme` remains the single palette and button-state source.

## M61 — Reusable UI and navigation foundation
- Added `MobileUiNavigationStack` with stable route keys, push/back/reset operations and a route-changed event.
- Kept presentation and gameplay callbacks outside the stack so existing HUD screens are not silently rerouted.
- Added shared button typography to the theme application path.

## M62 — Responsive screen composition
- Shared card helpers use layout groups and content sizing rather than fixed pixel-height assumptions.
- Existing safe-area and compact landscape detail-panel logic remains in `MobileHudCanvas`.
- Final per-screen migration and device layout acceptance are still required.

## M63 — Accessibility foundations
- Button typography now applies bounded best-fit sizing and wrapping through the shared theme.
- M59 contrast-aware foreground selection and palette audit remain active.
- This does not replace review of body text, focus order, screen readers, touch targets, localization or actual devices.

## M64 — Core gameplay-loop integration
- Added a consolidated Editor audit for the required farming, production, orders, quest, upgrade, mini-game and reward-service source dependencies.
- The audit deliberately reports file presence, not whether a fresh player can complete the loop.

## M65 — Economy and reward integrity review
- Kept the existing validated `GameSession` wallet operations, persistent upgrade costs and unique mini-game reward claims as the authoritative systems.
- Added these services to the integration audit so future UI/content work cannot accidentally omit a core dependency.
- No coin values, reward rates, upgrade costs or save schema were changed in this package. Economy balance still needs playtesting and a documented tuning pass.

## M66 — Quest presentation contract
- Added read-only `QuestSnapshot` data with stable ID, title, claim availability and reward values.
- Canvas presentation can now render quest cards without mutating quest/save state.
- Claiming remains owned by `QuestService.TryClaimNext`; the snapshot API does not grant rewards.

## M67 — Backyard content catalog
- Added an authorable `BackyardContentCatalog` ScriptableObject for crops, recipes, orders, collectibles, story goals and mini-games.
- Entries use stable IDs, area/unlock references, item requirements and explicit reward fields.
- Validation catches missing/duplicate IDs, missing labels/areas, negative quantities and reward quantities without an item ID.
- The catalog is an authoring/index layer only. Existing domain services remain authoritative; no unreviewed catalog asset is auto-created or injected into the running game.

## M68 — Production art intake
- Consolidated the existing reference-locked FBX import pipeline and production asset audit into the integration gate.
- Mia remains expected to use a reviewed humanoid rig; Max uses a reviewed generic/quadruped rig. Root motion remains disabled.
- Production meshes, UVs, atlases, materials, LODs and licensed asset provenance remain external deliverables.

## M69 — Character animation contract expansion
- Extended `CharacterAnimationBridge` with optional Garden, Reaction, Jump, Sleep and Play triggers.
- Extended the Animator state-machine builder to create states only when matching imported clips exist.
- Missing clips remain missing and are reported; no synthetic animations or placeholder clips are created.
- Max-specific cat actions are supported by the shared contract but require authored Max clips and manual review.

## M70 — Integration audit
- Added **Tools → Mia & Max → Gameplay & Character Integration Audit**.
- Checks required domain/UI/mini-game source files, content catalog structure and expected animation clip-name coverage for Mia and Max.
- The audit is static. It does not prove transitions, rig deformation, gameplay balance, scene wiring, save integrity, Unity compilation or device performance.

## Local Unity actions required

1. Pull the feature branch:
   `git pull --ff-only origin codex/mia-max-foundation`
2. Open with the recorded Unity 6000.3.x baseline and resolve packages.
3. Run **Tools → Mia & Max → UI Migration Audit**, **Production Asset Audit**, **Voice Asset Audit**, **Gameplay & Character Integration Audit** and **Release Readiness Audit**.
4. Create and populate a `BackyardContentCatalog` asset only after reviewing the intended first-release content IDs and rewards.
5. Import approved Mia/Max FBX models and authored clips; run **Apply Character FBX Import Profiles** and **Build Character Animator State Machines** only after the source assets are present.
6. Compile, enter Play Mode, complete a fresh-profile loop, test save/reload and inspect UI on short portrait and landscape screens.
7. Record physical Android/iOS build and device QA evidence separately.

**Acceptance status:** source foundations committed; Unity compile, Editor audits, content authoring, production art, animation review, economy balance, full gameplay loop and device tests remain pending.
