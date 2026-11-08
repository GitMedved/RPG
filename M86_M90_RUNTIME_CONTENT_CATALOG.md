# M86–M90 — Runtime content catalog integration

## M86 — Catalog-backed story quest metadata
- QuestService now optionally accepts a BackyardContentCatalog and otherwise loads the release catalog from Resources.
- Story quest snapshots and claim payouts resolve display name, coin reward and experience reward from matching StoryGoal entries.
- Existing quest completion predicates remain in the domain service; missing catalog entries fall back to the original built-in values so a missing asset does not block the prototype.
- Area unlock IDs are read from the catalog when present, with legacy defaults retained for the two existing annex quests.

## M87 — Runtime Resources location
- Starter catalog builder now creates the catalog at Assets/Resources/MiaMax/Content/BackyardContentCatalog.asset.
- If the older prototype catalog exists at Assets/_Project/ScriptableObjects/BackyardContentCatalog.asset and the Resources asset does not, the builder moves the existing asset rather than discarding authored entries.
- Existing catalog entries remain untouched by the starter builder.

## M88 — Regression coverage
- Added a tenth Editor smoke scenario that injects a temporary catalog and verifies quest snapshot metadata, actual coin/XP payout, claimed title and persisted area unlock.
- The smoke summary now reports 10/10 when all ten checks pass.
- This check is authored but has not been executed in the Unity Editor.

## M89 — Integration audit
- Gameplay & Character Integration Audit now checks for the runtime Resources loader and the builder's Resources destination.
- The audit remains a static source/asset check, not proof that the asset is present in a build or that gameplay runs correctly.

## M90 — Safe fallback and access baseline
- A missing or incomplete catalog entry preserves the existing built-in quest behavior.
- The backyard is treated as the default accessible area; annex access still depends on saved unlock state.
- No save schema changes were made.

## Local acceptance
1. Pull branch codex/mia-max-foundation.
2. Run Tools → Mia & Max → Create or Update Starter Content Catalog. Confirm the catalog is located under Assets/Resources/MiaMax/Content/.
3. Run Tools → Mia & Max → Run Core Smoke Checks. Expected result: 10/10.
4. Run Tools → Mia & Max → Gameplay & Character Integration Audit.
5. In Play Mode, complete and claim the first quest, verify the catalog title and reward, restart the app, and confirm the save and unlocked areas persist.

Unity compilation, Editor execution, asset migration, Player build inclusion, Play Mode and device behavior remain unverified until run locally.
