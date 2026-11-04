# M37 — Core Editor Smoke Checks

## Purpose

Adds a repeatable, fast validation command for core domain and local-save behavior without requiring a scene to be running.

Unity menu: **Tools → Mia & Max → Run Core Smoke Checks**

## Coverage

The command runs five grouped checks:

1. **Session economy and inventory** — rejects invalid coin/item changes, checks wallet spending and stack removal.
2. **Garden/workshop upgrades** — checks upgrade costs/levels, max-level guard and the configured production-time modifier.
3. **Production, orders and daily progress** — starts and collects a completed soup job, prevents duplicate collection/delivery and checks daily craft/delivery counters.
4. **Daily goal reward idempotency** — completes all four goals, verifies the reward values and rejects a second claim.
5. **Save validation and backup recovery** — checks value clamping, writes two save generations, corrupts the primary file, verifies restoration from the last known-good backup, then repeats the corruption/recovery cycle to ensure the backup remains intact.

The save test uses a uniquely named temporary folder under the operating system temp directory and deletes it in a finally block. The restore path now removes the invalid primary before writing the recovered data, so the normal save rotation cannot replace the known-good backup with the corrupt file.

## Result reporting

Each successful group writes a `[PASS]` entry to the Console. The final dialog reports the number of passed groups. On failure, the command stops, logs the exception/stack trace and shows the failed group count.

## Important limitations

- This is a lightweight Editor smoke-check menu, not a Unity Test Framework test assembly.
- It does not validate scene boot, character movement, colliders, UI touch targets, animations, audio, mini-game presentation, or platform-specific behavior.
- It does not replace Play Mode, save/reload testing on a real profile, Android/iOS builds or physical-device checks.
- No Unity Editor execution or compilation was performed as part of the remote source change; run the command locally after the branch is pulled.

## Acceptance

- Unity compiles without errors.
- The menu command completes with **5/5** groups.
- The Console contains five `[PASS]` lines and no exception.
- Existing player save data remains untouched; the test only uses an isolated temporary directory.
