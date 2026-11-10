# Mia & Max — Voice Asset Production Handoff

**Status: one Mia test take generated; 15 production takes still pending. Test take is not yet committed as a WAV asset or approved for shipping.**

## Intent
- English, US-oriented delivery preferred.
- Mia: bright, sweet, playful, warm, natural and child-friendly.
- Max: expressive funny cat voice; short phrases, meows and purr.
- Public/stock voices only; no voice cloning.
- 16 separate WAVs under Assets/Resources/MiaMax/Audio/Voice.

## Script
Exact resource IDs and current draft lines are in M49_VOICE_LINES_DRAFT.md (10 Mia, 6 Max). Approve exact text before synthesis.

## Voice selection
Mia test voice selected: Cassidy - Expressive & Imaginative (public voice ID `95d52e056b7b495dab8f090b44fa2ec4`, English, ACTIVE). One test line was generated: `mia_greeting_01` — “Hi, friend! Ready for a little backyard adventure?” Duration 3.1666 s; WAV; generated 2026-11-10. HeyGen result URL: https://resource2.heygen.ai/text_to_speech/6a44461d68cc4a39ba6f4d5245aed20a/95d52e056b7b495dab8f090b44fa2ec4/id=5e3586ec2bc042c4bc81ff5d420ffbf5.wav . This URL is a hosted test result, not a committed project asset. Binary retrieval into the repository has not succeeded; do not mark the Unity voice asset audit as passed. Max voice and remaining lines are pending. Synthesis consumes quota; no alternate takes or automatic retries after errors.

## File naming
Mia: mia_greeting_01.wav, mia_curiosity_01.wav, mia_discovery_01.wav, mia_plant_01.wav, mia_harvest_01.wav, mia_interact_01.wav, mia_quest_01.wav, mia_success_01.wav, mia_encourage_01.wav, mia_goodbye_01.wav.
Max: max_meow_happy_01.wav, max_meow_curious_01.wav, max_meow_surprised_01.wav, max_purr_01.wav, max_effort_01.wav, max_triumph_01.wav.

## Technical and quality gates
WAV mono, 22.05 kHz or higher, dry voice, no music. Preserve originals. Listen to every take; compare to script; check pronunciation, omissions/repetitions, emotional fit, cutoffs, clipping, loudness and duration. Max purr/meow must sound intentional. Imported AudioClip presence alone is not approval.

## Rights record
Retain provider, voice ID, service/model, generation date, commercial terms, source audio and final file. No real-person voice cloning without documented consent. Before release, verify the applicable HeyGen commercial-use terms for the selected plan and archive a copy of the terms in force at generation. A generated test take is not by itself evidence of commercial-use clearance.
