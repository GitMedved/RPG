# Mia & Max Adventures — Game Design Document

**Tagline:** Little Moments, Big Adventures!  
**Status:** Pre-production / MVP definition  
**Platforms:** Android + iOS · **Engine:** Unity · **View:** Portrait · **Mode:** Solo, offline-first  
**Audience:** Family-friendly, English-first international audience

## 1. Product promise
A warm, playful 3D world where players spend time with Mia and her ginger-cat companion Max, grow a backyard, craft useful items, explore nearby places and play short arcade activities. The emotional centre is the friendship between Mia and Max—not timers, monetisation or grind.

## 2. Pillars
1. **Cozy creation:** garden, harvest, craft, decorate and see the yard visibly improve.
2. **Gentle adventure:** explore, solve environmental puzzles, help friendly characters and uncover keepsakes; no combat.
3. **Playful variety:** short mini-games feed useful rewards into the shared world.
4. **Character warmth:** expressive reactions and companion moments make Mia and Max memorable.
5. **Respectful progression:** meaningful unlocks, no punishment for absence, no forced ads or pay-to-win pressure.

## 3. Fixed product decisions
- One Unity/C# project for Android and iOS.
- Stylized full 3D, mobile-optimized; URP is the recommended rendering pipeline.
- Portrait-first touch UX.
- Single-player, primarily offline, local versioned saves.
- English player-facing content; localization-ready strings.
- Free-to-play concept with optional cosmetics/premium content only; parent gate for purchases/external links.
- No multiplayer in MVP, no combat, gambling, paid randomized loot boxes, forced advertising or punitive energy.

## 4. Core session loop
Enter backyard → collect ready goods → plant/harvest → fulfil an order or quest → play a mini-game or explore → earn coins/XP/items → unlock and decorate → see a new story/activity become available.

**Session shapes:** 2–5 minutes (collect, plant, quick mini-game); 8–15 minutes (orders, quest chain, exploration); longer relaxed sessions (yard layout, collections, multiple activities).

## 5. World and content
### MVP location: Mia's Backyard
House, garden plots, fruit/flower area, Max's play area, workshop, pond and adventure gate. Start with a small readable area; expand through gates and visible construction milestones.

### First expansion: Magic Forest
A compact, separate exploration space unlocked through a short backyard quest chain. Focus on finding objects, helping a lost animal, repairing a simple bridge and discovering a treasure. No combat.

### Later locations (not MVP)
Sunny Beach, Flower Meadow, Fishing Lake, Cozy Village, Mountain Adventure and Treasure Island. Each should reuse world/quest/collection systems and add a small set of distinctive resources and activities.

## 6. MVP feature definition
| System | MVP acceptance target |
|---|---|
| Main world | One navigable 3D backyard scene with touch-friendly camera and interactables |
| Characters | Temporary Mia/Max stand-ins; move, idle, basic interaction; references govern final art |
| Farming | 3 crops, plots, plant/grow/harvest, inventory and XP integration |
| Inventory/economy | Item stack model, validated coin wallet, reward transaction path |
| Progression | XP, levels, gated unlocks and clear feedback |
| Production | 3 recipes, one production station/queue, collect completed output |
| Orders | Board with item requirements and validated rewards |
| Quests | Main/daily quest data model, objective progress, rewards |
| Mini-games | Match-3, fishing timing loop and Max endless-run prototype; all return validated rewards |
| Adventure | Map/gate UI and one small unlockable forest location |
| Decoration | Place, rotate, move/store a small starter set; placement validation |
| Collection | Discovery book with category, found state and completion reward |
| Save | Local versioned save, autosave checkpoints, load validation |
| UI | Main menu, HUD, quest/map/arcade/collection/settings navigation |
| Shop | Mock shop behind parent gate; no real billing in MVP |

**MVP completion definition:** a fresh player can start, complete the introductory quest chain, plant and harvest crops, fulfil an order, complete one activity in each mini-game, unlock the forest, save, close and restore progress. All core flows work without network access.

## 7. First-time user experience
1. Short, skippable welcome in the backyard.
2. Introduce Mia and Max using approved character assets or clearly labelled greybox stand-ins.
3. Guide the player through planting one crop and collecting it.
4. Explain inventory and fulfil a simple order.
5. Introduce Max's pond activity or a short arcade activity.
6. Present the first meaningful backyard improvement and forest unlock goal.

Avoid long exposition. Teach one interaction at a time.

## 8. Content data
Crops, recipes, quests, orders, rewards, locations, collectibles and decorations should be data-authored (ScriptableObjects after the Unity baseline is opened and validated). Stable string IDs must be used for saves; never use display names as identifiers.

## 9. UX and accessibility
Large touch targets, safe-area-aware portrait layouts, readable contrast, clear selection states, optional reduced motion, adjustable audio, pause/resume safety and no critical information communicated by colour alone. Use plain English and localization keys.

## 10. Monetisation guardrails
MVP shop is a non-purchasable mock. Future options may include direct-purchase costumes, Max accessories, decoration packs and premium themed content. Any purchase, external link, ad/privacy preference or account/cloud action must sit behind a parent gate. Platform billing, child privacy, age-rating and regional compliance require specialist review before release.

## 11. Art and audio direction
Warm, bright, tactile stylized 3D; soft shapes, expressive silhouettes, rich but controlled colour, readable vegetation and friendly lighting. Preserve supplied Mia/Max designs exactly. Until reference assets are available, use neutral primitives and label them as placeholders. Do not invent clothing, facial features or proportions. Audio placeholders must be replaceable and must not imply final voice casting.

## 12. Out of scope for MVP
Live events, cloud accounts, social features, multiplayer, analytics SDKs, real-money billing, ad SDKs, large open world, combat, full voice acting, full production-quality character rigs and the six later locations.

## 13. Product risks
Scope breadth; unknown source character assets; unverified editor/toolchain; mobile performance; save migration; mini-game scope; children's privacy and platform billing. See `MVP_ROADMAP.md` and `ASSET_REQUIREMENTS.md`.
