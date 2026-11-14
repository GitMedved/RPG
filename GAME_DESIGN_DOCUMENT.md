# Mia & Max Adventures — Game Design Document

**Tagline:** Little Moments, Big Adventures!  
**Status:** New direction — touch-led adventure prototype  
**Platforms:** Android + iOS · **Engine:** Unity 6 / URP · **View:** Portrait · **Mode:** Solo, offline-first  
**Audience:** Family-friendly, English-first international audience

## 1. Product promise

Mia & Max is a cozy, story-driven interactive world. The player does not directly steer Mia or Max around the map. Instead, the player explores the world by tapping places and objects, choosing contextual actions, gathering materials, making things, caring for the environment, waiting for meaningful results, playing short mini-games and unlocking new story chapters.

The emotional centre is the friendship between Mia and her ginger-cat companion Max. Their presence is expressed through approved character art, short animations, reactions, gestures and story moments—not joystick-controlled locomotion.

This is **not a farming game**. Growing plants may be one activity among many, but the game is about little projects, discoveries and adventures.

## 2. Core pillars

1. **Tap-led interaction:** every meaningful world object can present a small, clear set of contextual actions.
2. **Visible world change:** completed actions build, repair, decorate, restore or reveal something in the world.
3. **Connected projects:** collect resources, combine them into useful objects, and use those objects to unlock new activities or story scenarios.
4. **Gentle time-based outcomes:** some actions resolve immediately; others take a short time and produce a visible result. No punitive timers or energy walls.
5. **Mia & Max personality:** expressive animation, reactions, gestures, short scenes and character-led story prompts.
6. **Playful variety:** short mini-games provide resources, keepsakes and story items.
7. **Respectful progression:** clear goals, offline-friendly saves, no forced ads, combat, gambling or paid randomized rewards.

## 3. Fixed product decisions

- One Unity/C# project for Android and iOS.
- Stylized, high-quality 3D world with a fixed, composed camera; portrait-first touch UX.
- No direct character movement, virtual joystick, free-roam controller or character-follow camera in the core game.
- Single-player, primarily offline, local versioned saves.
- English player-facing content; localization-ready strings.
- Mia and Max use approved character references and production rigs. Do not invent final character designs or use primitive stand-ins as release art.
- Free-to-play concept with optional cosmetics/premium content only; parent gate for purchases/external links.
- No multiplayer in MVP, no combat, gambling, paid randomized loot boxes, forced advertising or punitive energy.

## 4. Core gameplay loop

Discover an object or story hint → tap it → read a concise contextual prompt → choose an action → confirm or play a short interaction → show a character/world animation → receive materials, progress or a visible world change → unlock the next action or story beat.

Longer progression loop:

**Explore → collect → craft/combine → build/restore → wait or solve → receive result → unlock a new scenario/location.**

The player should always have another useful action available while a timed result is pending.

## 5. World and story structure

### Opening location: Mia's Backyard

A polished, readable, compact story space based on the approved backyard reference: beige two-storey house, lawn, white fence, mature tree, swings, sandbox/play area, patio, shrubs and flower beds. These are scene landmarks, not a farm-management grid.

The first playable story is **The Little Birdhouse**:
1. Find a bundle of fallen twigs and a blue feather.
2. Use the crafting bench to prepare the pieces.
3. Build a birdhouse.
4. Choose a place for it and watch a short construction animation.
5. Wait for birds to discover it.
6. Care for nearby flowers and collect the blooms.
7. Use the new keepsake/material reward to open the next story space.

The first chapter teaches tap selection, contextual actions, inventory, crafting, short timers, rewards and progression without requiring character locomotion.

### First expansion: Secret Garden / Magic Forest

Unlocked by completing the opening chain and assembling a story item. Introduces new resources, a friendly animal that needs help, a small repair/building project, environmental puzzles and a discovery/treasure mini-game. No combat.

### Later story spaces

Sunny Beach, Flower Meadow, Fishing Lake, Cozy Village, Mountain Adventure and Treasure Island are future content candidates, not MVP commitments. Each should reuse the same world-action, resource, crafting, quest and mini-game systems while introducing a small number of distinct mechanics.

## 6. Object interaction contract

Every interactable world object should have:
- stable ID independent of display text;
- display name and concise description;
- available actions and their prerequisites;
- action duration (zero for instant actions);
- resource costs and rewards;
- resulting state/visual change;
- optional quest/progression events;
- save-safe state.

Tap targets must be generous for fingers. Context UI must be readable, safe-area-aware and never depend on floating debug labels in the 3D world. Confirm/cancel states must be obvious.

## 7. MVP systems and acceptance

| System | MVP acceptance target |
|---|---|
| Main world | One beautiful, fixed-camera backyard scene; no character locomotion |
| Touch interaction | Tap world objects and choose contextual actions |
| First story | Complete Little Birdhouse chain from gathering to visible result |
| Inventory | Stable item IDs, stacks and clear rewards |
| Crafting | One workshop with recipes, prerequisites and timed output |
| World state | Built/restored objects visibly change and persist |
| Time actions | Safe offline completion; no lost progress or punitive expiry |
| Progression | Quest chain, stars/XP, unlockable story gate |
| Mini-games | At least two short, touch-first mini-games returning validated rewards |
| Characters | Mia and Max appear through approved models/rigs, animation and reactions; no joystick |
| Save | Local versioned save, autosave checkpoints, load validation |
| UI | Portrait HUD, context panel, inventory, quest/story, mini-game transitions and settings |
| Mobile | Large touch targets, safe area, pause/resume safety, performance budgets |
| Shop | Non-purchasable mock only, behind parent gate |

**MVP completion definition:** a fresh player can start in the backyard, learn tap interactions, collect materials, craft and build the birdhouse, complete a second care/repair project, play at least two mini-games, unlock the first new story space, save, close and restore progress. All core flows work offline.

## 8. Art direction

Warm, bright, tactile stylized 3D with carefully authored silhouettes, rounded forms, rich but controlled colour, readable materials, soft natural lighting and polished composition. Materials should use coherent albedo/roughness treatment and appropriate scale; avoid flat default primitives, overexposure, missing textures, floating labels and unrelated asset packs.

The generated World Material Atlas is a starter material reference for grass, honey wood, cream plaster and garden stone. It must be imported at `Assets/Resources/MiaMax/Art/WorldMaterialAtlas.jpg` so runtime materials can load it. It is not a substitute for bespoke environment models, UVs, character meshes or final material authoring.

Preserve the supplied Mia and Max reference designs exactly. Production character meshes, facial rigs, fur/hair, UVs and animation clips must be reviewed against those references before release. Never claim generated placeholder geometry is final art.

## 9. Content and technical architecture

Use data-authored definitions for objects, actions, items, recipes, quests, rewards, locations and mini-games. Stable string IDs must be used in saves; never use display names as identifiers.

Keep the following systems separate:
- world presentation and fixed camera;
- tap selection and contextual action UI;
- object state and action execution;
- inventory/economy transactions;
- crafting queues and offline-safe timers;
- quest/story progression;
- mini-game adapters and reward validation;
- character animation/reaction presentation;
- versioned local save/migration.

## 10. UX and accessibility

Large touch targets, safe-area-aware portrait layouts, readable contrast, clear selection states, optional reduced motion, adjustable audio, pause/resume safety and no critical information communicated by colour alone. Teach one interaction at a time. Do not require precision dragging for core progression.

## 11. Monetisation and safety

MVP shop is a non-purchasable mock. Future options may include direct-purchase costumes, Max accessories, decoration packs and premium themed content. Any purchase, external link, ad/privacy preference or account/cloud action must sit behind a parent gate. Platform billing, child privacy, age-rating and regional compliance require specialist review before release.

## 12. Out of scope for MVP

Joystick-controlled characters, free-roam movement, character-follow camera, farming as the central loop, multiplayer, live events, cloud accounts, social features, analytics SDKs, real-money billing, ad SDKs, large open world, combat, full voice acting and unreviewed production character rigs.

## 13. Release risks

Scope breadth; missing approved character models/rigs; production art availability; Unity editor/toolchain validation; mobile performance; save migration; mini-game scope; children's privacy/platform compliance. A code commit is not equivalent to a Unity compile, visual review or device release test.
