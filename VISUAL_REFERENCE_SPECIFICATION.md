# Visual Reference Specification — Mia & Max

**Status:** Approved visual source references received in chat on 2026-11-02. These references override generic art-direction assumptions. They are not yet checked into this Git repository.

## 1. Character source references

### Max — Max.png
- Ginger/orange fluffy cat.
- Large green eyes, pink nose, cream muzzle and chest/belly.
- Distinct orange tabby striping on head, body and legs.
- Upright anthropomorphic pose shown in the full-body reference; quadruped anatomy and cat movement remain important for gameplay animation.
- Fluffy orange tail.
- Keep eye colour, coat palette, light muzzle/chest areas, ear shape and friendly expression consistent.
- Sheet includes full-body, face close-ups and side profile.

### Mia — Mia.jpg
- Young blonde girl with two long braids.
- Large green eyes and gentle friendly expression.
- White short-sleeve T-shirt, blue jeans and white sneakers in supplied full-body view.
- Keep hairstyle/braids, green eye colour, face proportions and outfit faithful to this reference.
- Sheet includes full-body, front/face and side profile views.

**Character lock:** Do not redesign, recolour, change hairstyle, change outfit or invent signature accessories without approval. Compare every 3D model against supplied views before acceptance.

## 2. Environment source reference

### Mia & Max Backyard — backyard_collage.png
The board provides a wide establishing view and detail views of the same sunny suburban backyard.

Observed layout and fixed visual elements:
- Large two-storey beige/light cream clapboard house with dark shingle roof, white trim and multiple white-framed windows.
- Patio/deck immediately beside the house, with outdoor seating and a black kettle-style grill.
- Broad central green lawn.
- White picket fence enclosing the yard.
- Large mature leafy tree on one side of the yard.
- Wooden A-frame swing set with two swings on the left side.
- Raised rectangular wooden sandbox/play bed with toys in the lawn area.
- Shrubs and flower beds around the fence and house; potted plants by the patio.
- Bright daytime, blue sky, scattered white clouds, lush greenery and warm sunlight.

### Spatial continuity rules
- Treat the wide view as the primary composition reference.
- Side and detail panels are supporting views of the same yard, not separate locations.
- Preserve relative placement of house, patio, central lawn, fence, tree, swing set and sandbox.
- Do not mirror the layout or move landmarks between camera angles.
- The house is the fixed anchor; patio sits against it; lawn occupies the open centre; fence and landscaping bound the yard.
- Build a blockout first and compare camera views to the board before adding detail.

## 3. Style and rendering
The board labels the target as a Pixar-style 3D animation reference. For the game, translate this into warm, polished, family-friendly stylized 3D suitable for real-time mobile rendering—not a pre-rendered film asset budget. Use soft forms, appealing materials, readable silhouettes and sunny natural lighting while preserving the concrete reference details above.

## 4. Unity implementation guidance
- Create a Backyard scene blockout with named landmark objects: House, Patio, CentralLawn, WhiteFence, MainTree, SwingSet, Sandbox, ShrubBeds and FlowerPots.
- Use modular fence, foliage and patio pieces; preserve consistent world scale.
- Establish one canonical world orientation and record the camera position used for the establishing screenshot.
- Use the wide reference for layout validation and detail panels for material/prop validation.
- Character placeholder objects must be clearly marked; do not approximate final face or hair/fur details with unapproved designs.
- Final character models, rigs and animations must match these references.

## 5. Reference asset handling
The three images were provided as conversation attachments and inspected, but have not yet been committed as binary files in the GitHub repository. Before Codex/Unity can consume them directly from the repo, add the originals under:
- Reference/Characters/Mia.jpg
- Reference/Characters/Max.png
- Reference/Environment/backyard_collage.png

Keep originals unchanged. Store any optimized runtime derivatives separately under Assets/_Project/Art/ and retain originals as source references.
