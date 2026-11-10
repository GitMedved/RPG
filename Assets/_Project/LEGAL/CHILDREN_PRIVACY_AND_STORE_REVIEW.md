# Mia & Max Adventures — Children's Privacy & Store Legal Review Pack

**Status: preparation only — NOT legal advice, NOT a completed legal review, and NOT a compliance certification.**
Product: Mia & Max Adventures. Studio label: AleksDevelop (do not represent as incorporated entity without evidence). Platforms: Android/iOS. Audience: family-friendly; child users are foreseeable.
Review date: 2026-11-10.

**Launch scope decision: United States + European Union/EEA simultaneously.** Treat this as a multi-jurisdiction launch. The EU/EEA review must identify each actual launch Member State (including Poland only if it is in the distribution plan); do not assume one GDPR child-consent age or one national rule applies across all Member States.

## 1. Decisions required before public distribution
Owner and qualified counsel must confirm the United States and each intended EU/EEA launch Member State/language; intended age bands and child-directed/mixed/general classification; all accounts, cloud saves, analytics, crash reporting, ads, attribution, push notifications, purchases, external links, UGC, microphone/camera/location/device identifiers; every SDK, provider, recipient, hosting region, retention period and international transfer; legal publisher identity and privacy contact; required age screen, parental consent/verification and parental gates; and third-party voice-generation vendor terms and commercial rights.

Do not publish until these questions have written answers and counsel signs off.

## 2. Conservative baseline pending legal review
- Offline-first single-player; no chat, social feed, public profiles, user uploads or user-to-user messaging.
- No account required for core gameplay.
- No targeted/behavioural advertising; preferably no advertising in first release.
- No precise location, contacts, photos, microphone, camera or advertising identifier access.
- Avoid third-party analytics in a child-directed build unless specifically reviewed and permitted.
- Keep saves local where practical. Review cloud backup separately before adding.
- Review external links, purchases, account creation and data collection for parental gates and legally valid consent.
- Provide clear, accessible privacy information and workable access, correction, deletion and consent-withdrawal processes where applicable.

These are risk-reduction recommendations, not legal advice.

## 3. Data inventory — verify against actual release build

| Data / signal | Intended baseline | Owner must verify |
|---|---|---|
| Name, email, date of birth | Not required | Search code, SDKs, forms |
| Account/authentication | None planned | Confirm no hidden SDK account |
| Save/progression | Local device save | Fields, backup, deletion |
| Device IDs / advertising ID | Not required | Inspect manifests, SDKs, traffic |
| Analytics / crash logs | Not approved here | SDK list, payloads, retention |
| Location / contacts / photos | Not required | Android permissions and iOS strings |
| Microphone / camera | Not required | Confirm no permission or SDK access |
| Purchases / ads | Not approved here | Decide business model and safeguards |
| Network requests | Core play should not require network | Capture and classify outbound traffic |
| Voice generation assets | Static authored dialogue | Vendor terms, commercial rights; no child recordings |

## 4. EU / EEA

**Applies to the EU/EEA part of the simultaneous launch.** Do not treat 'EU' as a single national consent-age rule: document the applicable Article 8 threshold for every Member State where the service is offered, and review relevant national child-protection, consumer, marketing and digital-service requirements. Assess international data transfers where US-based vendors or infrastructure receive EU/EEA personal data.
Identify a lawful basis for each processing purpose. If relying on consent for an online service offered directly to a child, verify the Member State's GDPR Article 8 age threshold (13–16) and implement reasonable, effective parental authorisation below it. Use child-accessible plain language. Assess minimisation, storage limitation, security, processor contracts, transfers, rights and whether a DPIA is required. Check national consumer and advertising-to-children rules in each launch market.

## 5. United States

**Applies to the US part of the simultaneous launch.** Assess federal COPPA and relevant state privacy/children's-design laws separately; do not assume COPPA is the only applicable US requirement. Review transfers and disclosures to service providers, including voice-generation and crash/analytics vendors.
Determine whether COPPA applies based on child-directed status or actual knowledge of users under 13 and the shipped app/SDK data practices. If applicable, assess verifiable parental consent, direct notice, parental rights, minimisation, security, retention/deletion and disclosure restrictions. Review the 2025 COPPA Rule amendments and compliance timetable with counsel. Check applicable state privacy and children's-design laws.

## 6. Apple App Store
- Decide age rating and Kids Category.
- If Kids Category: choose age band; implement parental gates for purchases, external links and other designated adult actions.
- Review Kids Category limits on third-party analytics, advertising and transmission of personal/device information for every SDK.
- Privacy policy URL in App Store Connect and accessible inside app.
- Privacy policy accurately describes data, purposes, recipients, retention/deletion and consent withdrawal.
- App Privacy disclosures match actual binary and SDK behaviour.
- Review purchase, external link, tracking, analytics and AI/vendor disclosures.
- Verify rights to all artwork, audio, fonts, music and character assets.

## 7. Google Play
- Accurate Target Audience and Content declaration and IARC rating.
- Data Safety form matches app and SDK behaviour.
- Determine Families Policy applicability; child-appropriate content/functionality.
- Review identifiers, location and all SDK transmissions.
- If ads are introduced, use applicable Families self-certified SDKs and compliant child-appropriate, non-personalised formats.
- Review monetisation, purchase flows and parental controls.
- Privacy policy URL and in-app access; maintain SDK inventory.

## 8. IP and asset clearance
For every shipped asset retain creator/source, creation date, source files, licence/contract, commercial-use permission, territory, duration and attribution requirements. Cover character likenesses, reference images, 3D models, rigs, animations, textures, UI, fonts, music, SFX and voice performances. For generated voices retain provider, voice ID, terms at generation, commercial-use rights and exact audio. No voice cloning without documented permission.

## 9. Evidence required for sign-off
- Territory matrix covering the United States and each EU/EEA Member State actually enabled for distribution, with applicable child-consent ages, age-rating/target-audience rules and counsel notes.
- Separate US and EU/EEA privacy-policy/legal review, with consistent descriptions of the same shipped data flows.
- Final SDK/dependency inventory and versions.
- Android manifest and iOS privacy manifest/permission declarations.
- Network capture from clean install and representative play.
- Data inventory, retention/deletion map, privacy policy and parent-facing notice.
- Parental consent/gate design and tests, if applicable.
- Store questionnaire answers and screenshots.
- Asset-rights register and vendor agreements.
- Written qualified-counsel sign-off for selected territories and audience classification.

## 10. Official references — re-check before submission
- Apple App Review Guidelines: https://developer.apple.com/app-store/review/guidelines/
- Apple Kids: https://developer.apple.com/kids/
- Google Play Families Policy: https://support.google.com/googleplay/android-developer/answer/9893335
- European Commission — children's data safeguards: https://commission.europa.eu/law/law-topic/data-protection/information-business-and-organisations/legal-grounds-processing-data/are-there-any-specific-safeguards-data-about-children_en
- European Commission — information for individuals: https://commission.europa.eu/law/law-topic/data-protection/information-individuals_en
- US FTC COPPA amendments: https://www.ftc.gov/legal-library/browse/federal-register-notices/16-cfr-part-312-coppa-final-rule-amendments

**Release gate: LEGAL_REVIEW_PENDING** until data inventory, territories, audience classification, asset/vendor rights and qualified legal sign-off are complete.
