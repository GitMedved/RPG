# Mia & Max — Procedural Audio Pack

The Editor generator is available at:

**Tools → Mia & Max → Audio → Generate Original Audio Pack**

It creates 18 original synthesized PCM WAV assets:

- 11 gameplay sound effects in `Assets/Resources/MiaMax/Audio/SFX`
- 4 ambience loops in `Assets/Resources/MiaMax/Audio/Ambience`
- 3 short original music themes in `Assets/Resources/MiaMax/Audio/Music`

The sound effects cover UI taps, planting, harvesting, workshop crafting, rewards, keepsakes and arcade feedback. Ambience includes backyard wind/birds and woodland wind/birds. Music themes are provided for the backyard, flower garden and woodland.

Runtime routing:
- `GameAudioService` loads the SFX by stable resource ID and starts the backyard theme.
- `BackyardAmbientPrototype` loads backyard wind and birds.
- `FlowerGardenPortalPrototype` switches to the appropriate area theme and woodland ambience when entering/leaving adventure areas.

The generated sounds use mathematical synthesis only. They contain no third-party recordings, sampled music, voice clones or spoken dialogue. Mia and Max voice recordings remain separate and are not fabricated by this generator.

After generation, run:
- `Tools → Mia & Max → Audio → Audio Asset Audit (Read Only)`
- `Tools → Mia & Max → Voice Asset Audit`

The audit verifies asset presence, duration, sample rate and basic import settings. It cannot approve sound quality, loudness, loop seams, accessibility mix, licensing outside this generated pack, or playback on target devices. Listen to all clips and validate the mix on Android and iOS before release.
