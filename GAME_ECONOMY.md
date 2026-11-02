# Game Economy — MVP

**Design intent:** rewarding, legible and non-punitive. All figures below are **initial balancing hypotheses**, not final values. Store all values in content data and tune through playtests.

## 1. Currencies
| Currency | Role | MVP sources | MVP sinks |
|---|---|---|---|
| Coins | Main soft currency | Harvest surplus, orders, quests, mini-game rewards | Seeds, starter decorations, production unlocks |
| Gems | Premium currency (future-ready only) | None in MVP | No real-money sink in MVP |
| Adventure Stars | Story progression | Story/forest objectives | Adventure gate milestones |
| Event Tokens | Seasonal content (future) | Not active in MVP | Not active in MVP |

No paid random rewards. No real-money purchases in the prototype.

## 2. Item groups
Seeds, crops, ingredients, crafted products, building materials, decorations, collectibles, fish and adventure quest items. Each item has a stable ID, category, stack policy and optional sell/use rules. Quest-critical and unique collectibles should not be accidentally sold.

## 3. Starter crop tuning hypothesis
| Crop | Seed cost | Grow time | Harvest | Suggested sell value | XP |
|---|---:|---:|---:|---:|---:|
| Carrot | 3 | 2 min | 2 | 4 each | 3 |
| Strawberry | 5 | 5 min | 2 | 7 each | 5 |
| Tomato | 7 | 8 min | 3 | 6 each | 7 |

These short prototype timings are for rapid iteration. Production release timings must be tuned against real session behaviour and offline return cadence. Avoid making the player wait with nothing useful to do.

## 4. Production recipes (MVP)
| Recipe | Inputs | Duration | Output | Purpose |
|---|---|---:|---|---|
| Carrot Soup | 3 carrots | 3 min | 1 soup | First production tutorial |
| Strawberry Juice | 3 strawberries | 4 min | 1 juice | Second recipe |
| Wooden Toy | 2 wood | 5 min | 1 toy | Workshop and collection |

Production values should be balanced so that crafted goods create interesting order choices rather than always being strictly better or worse than raw materials.

## 5. Reward bands (hypothesis)
- Tiny interaction / tutorial: 2–5 coins or a small item.
- Short daily task: 10–20 coins + modest XP.
- Order: input value plus a modest convenience premium and XP.
- Mini-game completion: score-banded reward with a hard cap; avoid infinite coin farming.
- Story milestone: Adventure Stars + unique cosmetic/decoration or unlock.

## 6. Anti-exploit and integrity rules
- All currency mutation through EconomyService.
- Transactions require a reason/source ID and validated non-negative amount.
- Reject NaN, infinity, negative rewards and unknown currency IDs.
- Do not let replaying a claimed reward grant it twice; use unique claim IDs.
- Cap mini-game rewards and apply server-independent local sanity limits; no server is required in offline MVP.
- Keep quest rewards and one-time unlocks idempotent.

## 7. Progression pacing
Levels should unlock new actions, recipes, plots, decoration slots or story steps—not merely increase prices. First session should show at least one visible improvement. The first additional location should be achievable through a short, clearly communicated chain, not a long grind.

## 8. Ethical monetisation principles
Future direct purchases should be cosmetic or clearly bounded premium content. No pay-to-win, forced ads, manipulative timers, energy walls, loot boxes or pressure messaging. Parent gate before purchase, external link, privacy/ad settings or cloud/account action. Real billing is outside MVP and requires Apple/Google integration plus child-safety/privacy/legal review.

## 9. Balancing checklist
- Can a new player afford the next useful action?
- Does every activity return a useful reward?
- Is there always a non-timer activity available?
- Can a player stop and return without loss?
- Are rewards understandable before the action?
- Are one-time rewards protected against duplicate claims?
- Are all sinks optional or clearly explained?
