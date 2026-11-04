# M23 — Lifetime achievements and progression

## Scope
Adds seven persistent lifetime achievements to the existing solo backyard loop. Achievement progress is driven by successful domain actions and saved in the existing local player save.

## Achievements
| Achievement | Goal | Reward |
|---|---:|---:|
| Green Fingers | Harvest 10 crops | 20 coins, 15 XP |
| Little Maker | Collect 5 crafted products | 25 coins, 20 XP |
| Helpful Neighbour | Deliver 5 orders | 30 coins, 25 XP |
| Memory Keeper | Find all 5 keepsakes | 35 coins, 25 XP |
| Flower Friend | Collect all 3 garden flowers | 20 coins, 20 XP |
| Arcade Explorer | Claim 3 mini-game rewards | 25 coins, 20 XP |
| Rising Star | Reach level 5 | 40 coins, 30 XP |

Each reward can be claimed once. Claim state and progress are stored in PlayerSaveData. Rewards are validated for integer overflow and applied together before save notification.

## Integration
- Farming, production collection and order delivery record lifetime progress after a successful action.
- Keepsake and flower achievements derive progress from their persisted discovery lists.
- Arcade achievement derives progress from unique mini-game reward claims.
- Level achievement derives progress from the current player level.
- Adventure Journal shows an achievement summary, a compact progress list and a claim-next-reward action.

## Limitations and acceptance
- Progress is local/offline and is not server-authoritative or tamper-proof.
- Achievement definitions and rewards are fixed in code for this prototype.
- The journal remains an IMGUI prototype; responsive Canvas presentation and authored icons are future visual work.
- Unity compile, Play Mode, save/load migration and Android/iOS device validation must be run in the Editor. Remote repository edits do not establish those results.
