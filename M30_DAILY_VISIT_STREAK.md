# M30 — Daily Visit Streak and Gift Track

## Changes
- Added persistent UTC visit streak data: last visit date, last claimed reward date, current streak and best streak.
- The first app session starts a one-day streak. A visit on the next UTC calendar day increments it; missing a full UTC day resets the current streak to one. Best streak is retained.
- Added a repeating seven-day reward cycle:
  | Day | Coins | XP |
  |---:|---:|---:|
  | 1 | 10 | 5 |
  | 2 | 15 | 5 |
  | 3 | 20 | 10 |
  | 4 | 25 | 10 |
  | 5 | 30 | 15 |
  | 6 | 40 | 20 |
  | 7 | 60 | 30 |
- Reward can be claimed once per UTC day. Claim date is saved before applying the reward in the same state mutation; overflow is checked.
- Added a compact visit-streak label and daily gift button to the upper-left HUD. Successful claims show a short confirmation toast.
- Added save normalization for old saves and streak counters. No schema version bump; missing new fields initialize safely.

## Acceptance checks in Unity
1. Start a fresh profile; confirm Visit streak shows 1 day and Day 1 gift can be claimed.
2. Claim Day 1; confirm +10 coins and +5 XP and that the button changes to CLAIMED.
3. Interact with the button repeatedly; confirm no duplicate reward.
4. Save/restart on the same UTC date; confirm the claimed state persists.
5. In a test save, set LastVisitUtcDate to yesterday; launch and confirm the streak increments by one.
6. Set LastVisitUtcDate to two days ago; launch and confirm the current streak resets to one while Best Streak remains.
7. Confirm Day 7 reward is 60 coins +30 XP and the following day cycles to Day 1.
8. Load a pre-M30 save; confirm it initializes without a migration error.

## Limitations
- This is an offline retention prototype. UTC time comes from the device clock and can be manipulated; no server-side validation or cloud account sync exists.
- The reward track repeats every seven consecutive visit days.
- No push notifications or background reminders are included.

## Validation status
Source-level checks only. Unity compile, Play Mode, save/reload, UTC boundary, streak-gap and mobile HUD layout checks remain to be performed locally.
