using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// A self-contained stylized 3D art pass made of authored procedural meshes, not
    /// Unity CreatePrimitive objects. No downloaded assets or artist tools are needed.
    /// Keeps the original invisible gameplay colliders independent of visual meshes.
    /// These are cohesive illustrated PROTOTYPE models, not approved character likenesses.
    /// </summary>
    public sealed class TapAdventureArtWorld : MonoBehaviour
    {
        private readonly Dictionary<string, Material> palette = new Dictionary<string, Material>();
        private readonly List<Transform> blooms = new List<Transform>();
        private readonly List<Transform> floaters = new List<Transform>();
        private Transform birdhouseArt, gardenArt, featherArt, twigArt, stoneArt;
        private TapAdventureWorldBootstrap game;
        private bool lastBirdhouse, lastGarden, lastBloom, lastTwigs, lastFeather;
        private float naturalTime;
        public TapAdventureCharacterRig Mia { get; private set; }
        public TapAdventureCharacterRig Max { get; private set; }

        public static TapAdventureArtWorld Install(TapAdventureWorldBootstrap owner)
        {
            GameObject art = new GameObject("ART - Illustrated Backyard and Character Stage");
            art.transform.SetParent(owner.transform, false);
            TapAdventureArtWorld result = art.AddComponent<TapAdventureArtWorld>();
            result.game = owner;
            result.Build();
            return result;
        }

        private Material Mat(string id, Color color, float smooth = 0.15f)
        {
            if (palette.TryGetValue(id, out Material cached)) return cached;
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            Material material = new Material(shader) { name = "MiaMax Art - " + id, color = color };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smooth);
            palette[id] = material;
            return material;
        }

        private Material Resource(string name, Color fallback)
        {
            string id = "ref:" + name;
            if (palette.TryGetValue(id, out Material result)) return result;
            result = Resources.Load<Material>("MiaMax/Materials/" + name);
            if (result == null) result = Mat(name, fallback);
            palette[id] = result;
            return result;
        }

        private Transform Group(string name, Transform parent = null)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent == null ? transform : parent, false);
            return go.transform;
        }

        private Transform Model(string name, Mesh mesh, Material material, Transform parent,
            Vector3 position, Vector3 scale, Quaternion? rotation = null)
        {
            GameObject go = new GameObject(name);
            Transform t = go.transform;
            t.SetParent(parent, false);
            t.localPosition = position;
            t.localRotation = rotation ?? Quaternion.identity;
            t.localScale = scale;
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.On;
            renderer.receiveShadows = true;
            return t;
        }

        public Transform SoftBox(string name, Transform parent, Vector3 pos, Vector3 size, Material mat)
            => Model(name, ProceduralMeshes.RoundedBox, mat, parent, pos, size);

        public Transform Organic(string name, Transform parent, Vector3 pos, Vector3 size, Material mat,
            float wobble = 0.1f, int seed = 2)
            => Model(name, ProceduralMeshes.Blob(wobble, seed), mat, parent, pos, size);

        private Transform Column(string name, Transform parent, Vector3 pos,
            Vector3 scale, Material mat)
            => Model(name, ProceduralMeshes.TaperedColumn, mat, parent, pos, scale);

        private void Branch(Transform parent, string name, Vector3 start, Vector3 end,
            float radius, Material material)
        {
            Vector3 direction = end - start;
            Model(name, ProceduralMeshes.TaperedColumn, material, parent,
                (start + end) * 0.5f, new Vector3(radius, direction.magnitude, radius),
                Quaternion.FromToRotation(Vector3.up, direction.normalized));
        }

        private void AddHit(string id, Transform parent, Vector3 worldPos, float radius)
        {
            GameObject go = new GameObject("Touch zone - " + id);
            go.transform.SetParent(parent, false);
            go.transform.position = worldPos;
            SphereCollider collider = go.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = radius;
            TapAdventureTarget target = go.AddComponent<TapAdventureTarget>();
            target.Id = id;
            target.DisplayName = id;
        }

        private void Build()
        {
            // Hide all primitive renderers created by the old runtime before inserting art.
            // Their original gameplay colliders remain active as a backwards-compatible fallback.
            foreach (Renderer old in game.GetComponentsInChildren<Renderer>(true))
                old.enabled = false;

            BuildTerrain();
            BuildHouse();
            BuildFence();
            BuildAppleTree();
            BuildGarden();
            BuildWorkshop();
            BuildWater();
            BuildProps();
            BuildAdventureGate();
            BuildSecretGarden();
            BuildCharacters();
            // Native CC0 GLBs are real mesh assets, not additional primitives.
            // On any import issue the authored procedural scene remains visible.
            MiaMaxExternalArtPass.AddNature(transform);
            MiaMaxExternalArtPass.ApplyRiggedCharacterOverrides(Mia, Max);
            Sync(true);
        }

        private void BuildTerrain()
        {
            Transform t = Group("Landscaped lawn and cobblestones");
            Material lawn = Resource("Grass_Lawn", new Color(0.34f, 0.62f, 0.31f));
            Material leaf = Mat("Meadow light", new Color(0.56f, 0.74f, 0.36f));
            Material stone = Resource("Stone_Patio", new Color(0.68f, 0.64f, 0.51f));
            Material pale = Mat("Pale stone edges", new Color(0.87f, 0.81f, 0.63f));
            SoftBox("Soft edge lawn platform", t, new Vector3(0, -0.22f, 0),
                new Vector3(20.8f, 0.38f, 19.8f), lawn);
            SoftBox("Porch slab", t, new Vector3(0, 0.05f, 5.75f),
                new Vector3(8.9f, 0.18f, 3.25f), stone);
            for (int i = 0; i < 13; i++)
            {
                float x = ((i % 3) - 1) * 0.23f;
                float z = -8.2f + i * 1.12f;
                Organic("Stone " + i, t, new Vector3(x, 0.018f, z),
                    new Vector3(1.0f + (i % 2) * .15f, .10f, .72f), i % 3 == 0 ? pale : stone, .045f, i + 7);
            }
            // A scattering of small stylized meadow tufts, planted (not floated).
            for (int i = 0; i < 65; i++)
            {
                float x = Mathf.Sin(i * 12.9898f) * 8.7f;
                float z = Mathf.Cos(i * 4.711f) * 8.8f;
                if (Mathf.Abs(x) < 1.8f || z > 5f || Vector2.Distance(
                    new Vector2(x, z), new Vector2(-4.1f, -5.4f)) < 3.0f) continue;
                Organic("Meadow grass clump", t, new Vector3(x, .035f, z),
                    new Vector3(.20f, .13f, .16f), i % 3 == 0 ? leaf : lawn, .13f, i);
            }
        }

        private void BuildHouse()
        {
            Transform h = Group("Warm two-storey cottage");
            Material wall = Resource("Plaster_Cream", new Color(.91f, .81f, .64f));
            Material timber = Resource("Wood_Warm", new Color(.53f, .31f, .17f));
            Material roof = Resource("Roof_Shingle", new Color(.58f, .24f, .19f));
            Material trim = Mat("Ivory painted trim", new Color(.99f, .95f, .83f));
            Material blue = Mat("Blue cottage windows", new Color(.52f, .77f, .83f), .65f);
            Material dark = Mat("Dark door accents", new Color(.25f, .25f, .22f));
            SoftBox("Plaster cottage body", h, new Vector3(0, 2.15f, 8.0f),
                new Vector3(8.3f, 4.25f, 3.7f), wall);
            SoftBox("Front foundation", h, new Vector3(0, .16f, 6.13f),
                new Vector3(8.4f, .32f, .24f), trim);
            SoftBox("Eaves trim", h, new Vector3(0, 4.27f, 8),
                new Vector3(9.1f, .25f, 4.35f), trim);
            Model("Sloping tiled roof", ProceduralMeshes.GableRoof, roof, h,
                new Vector3(0, 4.36f, 8f), new Vector3(9.4f, 1.95f, 4.48f));
            for (int floor = 0; floor < 2; floor++)
                for (int side = -1; side <= 1; side += 2)
                {
                    float x = side * 2.55f;
                    float y = floor == 0 ? 2.04f : 3.53f;
                    SoftBox("Window outer casing", h, new Vector3(x, y, 6.05f),
                        new Vector3(1.48f, 1.28f, .17f), trim);
                    SoftBox("Window azure glass", h, new Vector3(x, y, 5.96f),
                        new Vector3(1.19f, .99f, .10f), blue);
                    SoftBox("Window cross mullion", h, new Vector3(x, y, 5.90f),
                        new Vector3(.10f, 1.05f, .09f), trim);
                    SoftBox("Window sill", h, new Vector3(x, y - .65f, 5.91f),
                        new Vector3(1.67f, .13f, .37f), trim);
                    SoftBox("Wooden shutter", h, new Vector3(x - .84f, y, 6.05f),
                        new Vector3(.27f, 1.17f, .14f), timber);
                    SoftBox("Wooden shutter", h, new Vector3(x + .84f, y, 6.05f),
                        new Vector3(.27f, 1.17f, .14f), timber);
                }
            SoftBox("Honey wood front door", h, new Vector3(0, 1.15f, 6.04f),
                new Vector3(1.20f, 2.30f, .19f), timber);
            SoftBox("Front door frame", h, new Vector3(0, 2.39f, 6.02f),
                new Vector3(1.48f, .20f, .22f), trim);
            Organic("Golden door knob", h, new Vector3(.41f, 1.14f, 5.90f),
                new Vector3(.11f, .12f, .12f), dark);
            SoftBox("Porch stair", h, new Vector3(0, .25f, 5.52f),
                new Vector3(2.5f, .18f, .74f), trim);
            SoftBox("Porch step", h, new Vector3(0, .12f, 5.06f),
                new Vector3(2.75f, .18f, .72f), stone);
            for (int side=-1;side<=1;side+=2)
            {
                Column("Porch post", h, new Vector3(side*3.7f,1.25f,5.75f),
                    new Vector3(.18f,2.50f,.18f), trim);
                Organic("Potted porch flower", h, new Vector3(side*3.25f,.54f,5.47f),
                    new Vector3(.36f,.45f,.37f), Mat("Terracotta",new Color(.67f,.38f,.26f)));
            }
            AddHit("house", h, new Vector3(0, 1.2f, 5.8f), 1.15f);
        }

        private void BuildFence()
        {
            Transform t = Group("White garden picket fence");
            Material white = Mat("Ivory fence paint", new Color(.96f,.95f,.86f));
            Material shadow = Mat("Fence warm shadows", new Color(.75f,.74f,.65f));
            for (int side=-1;side<=1;side+=2)
            {
                float x=side*9.05f;
                for(int j=0;j<20;j++)
                {
                    float z=-8.0f+j*.79f;
                    SoftBox("Painted picket",t,new Vector3(x,.75f,z),
                        new Vector3(.20f,1.40f,.25f),white);
                }
                for(int rail=0;rail<2;rail++)
                    SoftBox("Fence rail",t,new Vector3(x,.45f+rail*.60f,-.5f),
                        new Vector3(.27f,.13f,16f),rail==0?white:shadow);
            }
            for(int j=0;j<23;j++)
            {
                float x=-8.7f+j*.78f;
                SoftBox("Back fence picket",t,new Vector3(x,.74f,9.55f),
                    new Vector3(.24f,1.39f,.18f),white);
            }
            SoftBox("Fence top rail",t,new Vector3(0,1.15f,9.55f),
                new Vector3(18f,.12f,.19f),white);
        }

        private void BuildAppleTree()
        {
            Transform tree=Group("Old apple tree and birdhouse");
            Material bark=Resource("Bark_Tree",new Color(.45f,.27f,.17f));
            Material leaf=Resource("Leaf_Green",new Color(.29f,.56f,.29f));
            Material light=Mat("Spring foliage",new Color(.44f,.69f,.31f));
            Material deep=Mat("Canopy shadow green",new Color(.22f,.41f,.26f));
            Material fruit=Mat("Blushing apples",new Color(.86f,.26f,.27f),.36f);
            Column("Tapered apple tree trunk",tree,new Vector3(-5.2f,1.9f,.7f),
                new Vector3(.84f,3.9f,.85f),bark);
            Branch(tree,"Old limb",new Vector3(-5.2f,2.9f,.7f),
                new Vector3(-6.9f,4.2f,.7f),.28f,bark);
            Branch(tree,"Sunward limb",new Vector3(-5.2f,3.05f,.7f),
                new Vector3(-3.8f,4.4f,1.0f),.26f,bark);
            for(int i=0;i<9;i++)
            {
                float theta=i*2.3999632f;
                float x=-5.2f+Mathf.Cos(theta)*1.55f;
                float z=.7f+Mathf.Sin(theta)*1.0f;
                float y=4.0f+(i%3)*.37f;
                Organic("Sculpted leafy crown",tree,new Vector3(x,y,z),
                    new Vector3(2.20f,1.75f,1.94f),i%4==0?deep:(i%3==0?light:leaf),.12f,17+i);
            }
            for(int i=0;i<11;i++)
            {
                float a=i*2.399963f;
                Organic("Red apple",tree,new Vector3(-5.2f+Mathf.Cos(a)*1.45f,
                    3.9f+(i%3)*.30f,.7f+Mathf.Sin(a)*1.0f),
                    new Vector3(.21f,.24f,.21f),fruit,.03f,9+i);
            }
            AddHit("tree",tree,new Vector3(-5.2f,2f,.7f),1.1f);
            Transform mount=SoftBox("Birdhouse mounting shelf",tree,new Vector3(-4.5f,2.98f,.45f),
                new Vector3(1.02f,.13f,.83f),bark);
            birdhouseArt=Group("Finished birdhouse",tree);
            Material oak=Resource("Wood_Warm",new Color(.57f,.36f,.17f));
            Material cream=Mat("Birdhouse creamy edges",new Color(.94f,.83f,.59f));
            Material red=Resource("Roof_Shingle",new Color(.63f,.29f,.23f));
            SoftBox("Birdhouse walls",birdhouseArt,new Vector3(-4.5f,3.35f,.45f),
                new Vector3(.74f,.66f,.62f),oak);
            Model("Birdhouse gable roof",ProceduralMeshes.GableRoof,red,birdhouseArt,
                new Vector3(-4.5f,3.7f,.45f),new Vector3(.98f,.45f,.85f));
            Organic("Bird entrance",birdhouseArt,new Vector3(-4.5f,3.40f,.115f),
                new Vector3(.22f,.22f,.055f),Mat("Birdhouse opening",new Color(.18f,.14f,.12f)));
            Column("Bird landing perch",birdhouseArt,new Vector3(-4.5f,3.13f,.01f),
                new Vector3(.08f,.30f,.08f),cream);
            AddHit("birdhouse",tree,new Vector3(-4.5f,3.38f,.45f),.78f);
        }

        private void BuildGarden()
        {
            Transform bed=Group("Raised flower garden");
            Material wood=Resource("Wood_Warm",new Color(.56f,.35f,.21f));
            Material soil=Resource("Soil_Garden",new Color(.30f,.22f,.16f));
            Material leaf=Resource("Leaf_Green",new Color(.23f,.52f,.29f));
            Material pink=Mat("Coral blossoms",new Color(.97f,.49f,.48f));
            Material yellow=Mat("Sunshine flower",new Color(.99f,.77f,.30f));
            Material ivory=Mat("Flower centers",new Color(.99f,.90f,.53f));
            SoftBox("Raised garden base",bed,new Vector3(4.2f,.30f,.3f),
                new Vector3(3.86f,.55f,2.52f),wood);
            SoftBox("Rich dark garden soil",bed,new Vector3(4.2f,.58f,.3f),
                new Vector3(3.60f,.12f,2.25f),soil);
            for(int i=0;i<7;i++)
            {
                float x=2.70f+(i%4)*.86f;
                float z=-.3f+(i/4)*.95f;
                float h=.75f+(i%3)*.12f;
                Branch(bed,"Flower stem",new Vector3(x,.57f,z),
                    new Vector3(x+.08f,h+.54f,z),.055f,leaf);
                Organic("Sculpted leaf",bed,new Vector3(x-.18f,h*.8f,z),
                    new Vector3(.34f,.08f,.17f),leaf,.06f,3+i);
                Transform blossom=Group("Bloom " + i,bed);
                blossom.localPosition=new Vector3(x+.08f,h+.55f,z);
                for(int petal=0;petal<6;petal++)
                {
                    float angle=petal*Mathf.PI/3f;
                    Organic("Petal",blossom,new Vector3(Mathf.Cos(angle)*.20f,
                        Mathf.Sin(angle)*.20f,0f),new Vector3(.23f,.30f,.13f),
                        i%3==0?yellow:pink,.04f,petal+4);
                }
                Organic("Golden center",blossom,new Vector3(0,0,-.08f),
                    new Vector3(.21f,.21f,.19f),ivory);
                blooms.Add(blossom);
            }
            AddHit("flowers",bed,new Vector3(4.2f,.78f,.3f),1.54f);
        }

        private void BuildWorkshop()
        {
            Transform t=Group("Carpentry table and garden tools");
            Material oak=Resource("Wood_Warm",new Color(.58f,.38f,.24f));
            Material dark=Mat("Dark walnut",new Color(.39f,.25f,.16f));
            Material metal=Mat("Garden brushed metal",new Color(.71f,.76f,.70f),.58f);
            Material terracotta=Mat("Terra pot",new Color(.75f,.41f,.30f));
            SoftBox("Planed work surface",t,new Vector3(5.0f,.96f,4.2f),
                new Vector3(2.74f,.23f,1.39f),oak);
            for(int sx=-1;sx<=1;sx+=2)
                for(int sz=-1;sz<=1;sz+=2)
                    SoftBox("Workbench tapered leg",t,new Vector3(5+sx*1.09f,.47f,4.2f+sz*.47f),
                        new Vector3(.22f,.91f,.20f),dark);
            SoftBox("Back tool panel",t,new Vector3(5f,1.6f,4.83f),
                new Vector3(2.38f,1.03f,.18f),oak);
            for(int i=0;i<4;i++)
            {
                SoftBox("Tool rail holder",t,new Vector3(4.2f+i*.5f,1.53f,4.69f),
                    new Vector3(.13f,.48f,.13f),metal);
            }
            Organic("Painted watering can",t,new Vector3(4.4f,1.2f,4.2f),
                new Vector3(.50f,.42f,.43f),metal);
            Organic("Small clay pot",t,new Vector3(5.65f,1.19f,4.1f),
                new Vector3(.40f,.38f,.39f),terracotta);
            AddHit("workshop",t,new Vector3(5.0f,1.10f,4.2f),1.23f);
        }

        private void BuildWater()
        {
            Transform pond=Group("Water garden and boulders");
            Material water=Resource("Water_Pond",new Color(.36f,.73f,.81f));
            Material stone=Resource("Stone_Mossy",new Color(.57f,.62f,.49f));
            Material reed=Resource("Leaf_Green",new Color(.28f,.59f,.39f));
            Organic("Aqua pond",pond,new Vector3(-4.1f,.03f,-5.4f),
                new Vector3(3.65f,.12f,2.66f),water,.04f,9);
            for(int i=0;i<15;i++)
            {
                float a=i*Mathf.PI*2f/15f;
                float x=-4.1f+Mathf.Cos(a)*1.88f;
                float z=-5.4f+Mathf.Sin(a)*1.37f;
                Organic("Weathered moss stone",pond,new Vector3(x,.16f,z),
                    new Vector3(.48f,.30f,.42f),stone,.17f,i+20);
            }
            for(int i=0;i<7;i++)
            {
                float x=-5.9f+i*.18f;
                Branch(pond,"Pond reed",new Vector3(x,.10f,-4.3f),
                    new Vector3(x+(i%2==0?.07f:-.08f),.95f+(i%3)*.15f,-4.3f),.04f,reed);
            }
            AddHit("pond",pond,new Vector3(-4.1f,.20f,-5.4f),1.5f);
        }

        private void BuildProps()
        {
            Transform t=Group("Collectibles and playful details");
            Material bark=Resource("Wood_Warm",new Color(.55f,.35f,.19f));
            Material blue=Mat("Bright blue feather",new Color(.27f,.60f,.90f));
            Material pale=Mat("Shell",new Color(.95f,.84f,.68f));
            Material wicker=Resource("Wicker_Rattan",new Color(.66f,.47f,.32f));
            twigArt=Group("Gatherable twigs",t);
            for(int i=0;i<4;i++)
                Branch(twigArt,"Twigs",new Vector3(1.70f+i*.10f,.15f,-3.75f),
                    new Vector3(2.19f+i*.04f,.19f,-4.10f+i*.06f),.045f,bark);
            AddHit("twigs",twigArt,new Vector3(2f,.20f,-3.83f),.70f);
            featherArt=Group("Gatherable blue feather",t);
            Transform feather=Organic("Blue feather",featherArt,new Vector3(-1.8f,.30f,-6.8f),
                new Vector3(.23f,.06f,.73f),blue,.075f,6);
            feather.localRotation=Quaternion.Euler(0f,35f,18f);
            AddHit("feather",featherArt,new Vector3(-1.8f,.30f,-6.8f),.56f);
            Transform basket=Group("Max treasure basket",t);
            Model("Basket lower bowl",ProceduralMeshes.Basket,wicker,basket,
                new Vector3(5.6f,.26f,-5.3f),new Vector3(1.02f,.75f,1.02f));
            Organic("Basket shell",basket,new Vector3(5.40f,.69f,-5.3f),
                new Vector3(.25f,.21f,.21f),pale);
            Organic("Basket blue treasure",basket,new Vector3(5.8f,.68f,-5.4f),
                new Vector3(.24f,.11f,.34f),blue);
            AddHit("minigame",basket,new Vector3(5.6f,.45f,-5.3f),1f);
            for(int i=0;i<7;i++)
            {
                float x=-8f+(i%4)*4.7f;
                float z=-7.7f+(i/4)*12.2f;
                Organic("Hydrangea shrub",t,new Vector3(x,.46f,z),
                    new Vector3(.95f,.82f,.88f),
                    Mat("Shrub leaf",new Color(.25f,.51f,.32f)),.12f,50+i);
            }
        }

        private void BuildAdventureGate()
        {
            Transform gate = Group("Whimsical gate to the Secret Garden");
            Material timber = Resource("Wood_Warm",new Color(.57f,.38f,.21f));
            Material ivory = Mat("Garden gate ivory paint",new Color(.94f,.93f,.80f));
            Material copper = Mat("Garden gate copper hinges",new Color(.72f,.49f,.27f));
            Material moss = Resource("Leaf_Green",new Color(.35f,.56f,.30f));
            SoftBox("Left carved post",gate,new Vector3(5.82f,1.26f,.2f),
                new Vector3(.29f,2.52f,.33f),ivory);
            SoftBox("Right carved post",gate,new Vector3(7.78f,1.26f,.2f),
                new Vector3(.29f,2.52f,.33f),ivory);
            SoftBox("Curved entry header",gate,new Vector3(6.8f,2.56f,.2f),
                new Vector3(2.27f,.30f,.38f),ivory);
            for(int i=0;i<9;i++)
            {
                float x=5.92f+i*.22f;
                float height=1.92f+.32f*Mathf.Sin(i*Mathf.PI/8f);
                SoftBox("Cottage fence gate picket",gate,new Vector3(x,height*.5f,.2f),
                    new Vector3(.16f,height,.17f),timber);
            }
            SoftBox("Gate horizontal rail",gate,new Vector3(6.80f,.65f,.02f),
                new Vector3(1.90f,.16f,.16f),ivory);
            SoftBox("Gate horizontal rail",gate,new Vector3(6.80f,1.29f,.02f),
                new Vector3(1.90f,.16f,.16f),ivory);
            Organic("Hand painted brass latch",gate,new Vector3(6.94f,1.05f,-.05f),
                new Vector3(.17f,.12f,.09f),copper);
            for(int i=0;i<4;i++)
                Organic("Climbing ivy",gate,new Vector3(5.8f+.1f*Mathf.Sin(i*1.5f),
                    1f+i*.42f,-.15f),
                    new Vector3(.23f,.32f,.31f),moss,.08f,7+i);
            AddHit("gate",gate,new Vector3(6.8f,1.24f,.15f),1.25f);
        }

        private void BuildSecretGarden()
        {
            gardenArt=Group("Secret Garden story island");
            Material grass=Resource("Grass_Lawn",new Color(.30f,.51f,.31f));
            Material moss=Resource("Stone_Mossy",new Color(.61f,.62f,.46f));
            Material magic=Mat("Mystic leaf blue-green",new Color(.27f,.57f,.51f));
            Material glow=Mat("Golden stone",new Color(1f,.74f,.36f),.66f);
            SoftBox("Tucked-away clearing",gardenArt,new Vector3(12.15f,-.20f,-1.0f),
                new Vector3(8.7f,.37f,10.7f),grass);
            for(int i=0;i<7;i++)
            {
                float x=9.15f+(i%4)*2.1f;
                float z=-4.5f+(i/4)*4.3f;
                Column("Secret tree trunk",gardenArt,new Vector3(x,1.34f,z),
                    new Vector3(.43f,2.7f,.44f),moss);
                Organic("Willow crown",gardenArt,new Vector3(x,3.07f,z),
                    new Vector3(2.1f,2.2f,2.0f),magic,.15f,77+i);
            }
            for(int i=0;i<8;i++)
                Organic("Moonlit stepping stone",gardenArt,
                    new Vector3(8.9f+i*.78f,.05f,-1.95f+(i%2)*.29f),
                    new Vector3(.82f,.17f,.58f),moss,.06f,88+i);
            stoneArt=Organic("Golden story stone",gardenArt,new Vector3(12.4f,.95f,-1.1f),
                new Vector3(.86f,1.4f,.74f),glow,.16f,73);
            AddHit("gardenstone",gardenArt,new Vector3(12.4f,.95f,-1.1f),1f);
        }

        private void BuildCharacters()
        {
            // Original reference portraits remain the authoritative visual designs.
            // These animated low-cost meshes are visible development stand-ins only.
            Transform cast=Group("Mia and Max - animated illustrated proxies");
            Mia=TapAdventureCharacterRig.CreateMia(cast,new Vector3(-.90f,0f,1.1f));
            Max=TapAdventureCharacterRig.CreateMax(cast,new Vector3(.45f,0f,1.1f));
        }

        private void Update()
        {
            if(game==null)return;
            naturalTime+=Time.deltaTime;
            Sync(false);
            if(stoneArt!=null && stoneArt.gameObject.activeSelf)
            {
                stoneArt.localRotation=Quaternion.Euler(0,naturalTime*14f,6f*Mathf.Sin(naturalTime*.9f));
                stoneArt.localPosition=new Vector3(12.4f,.95f+.065f*Mathf.Sin(naturalTime*1.8f),-1.1f);
            }
        }

        private void Sync(bool force)
        {
            if(game==null)return;
            bool built=game.BirdhouseBuilt;
            bool unlocked=game.GardenUnlocked;
            bool blooming=game.FlowersBloomed;
            if(force||built!=lastBirdhouse)
            {
                if(birdhouseArt!=null)birdhouseArt.gameObject.SetActive(built);
                lastBirdhouse=built;
            }
            if(force||unlocked!=lastGarden)
            {
                if(gardenArt!=null)gardenArt.gameObject.SetActive(unlocked);
                lastGarden=unlocked;
            }
            if(force||blooming!=lastBloom)
            {
                foreach(Transform bloom in blooms)
                    if(bloom!=null)bloom.localScale=Vector3.one*(blooming?1.0f:.48f);
                lastBloom=blooming;
            }
            bool twigsGone=game.TwigsGathered;
            bool featherGone=game.FeatherGathered;
            if(force||twigsGone!=lastTwigs)
            {
                if(twigArt!=null)twigArt.gameObject.SetActive(!twigsGone);
                lastTwigs=twigsGone;
            }
            if(force||featherGone!=lastFeather)
            {
                if(featherArt!=null)featherArt.gameObject.SetActive(!featherGone);
                lastFeather=featherGone;
            }
        }

        public void React(string id, bool success)
        {
            if(Mia!=null)Mia.React(success);
            if(Max!=null)Max.React(success);
        }

        private void OnDestroy()
        {
            foreach(Material m in palette.Values)
                if(m!=null && m.name.StartsWith("MiaMax Art - ",StringComparison.Ordinal))
                    Destroy(m);
        }
    }

    /// <summary>A compact deterministic mesh library with UVs and normals.</summary>
    public static class ProceduralMeshes
    {
        private static Mesh rounded;
        private static Mesh column;
        private static Mesh roof;
        private static Mesh basket;
        private static readonly Dictionary<int,Mesh> blobs=new Dictionary<int, Mesh>();
        public static Mesh RoundedBox => rounded!=null?rounded:(rounded=Rounded());
        public static Mesh TaperedColumn => column!=null?column:(column=Lathe(
            new [] { new Vector2(.38f,-.5f),new Vector2(.49f,-.42f),
                new Vector2(.41f,.08f),new Vector2(.28f,.46f),new Vector2(.22f,.5f) },14));
        public static Mesh Basket => basket!=null?basket:(basket=Lathe(
            new [] {new Vector2(.30f,-.50f),new Vector2(.38f,-.38f),
                new Vector2(.46f,.18f),new Vector2(.49f,.44f),
                new Vector2(.50f,.49f),new Vector2(.41f,.49f),
                new Vector2(.38f,.30f)},18));
        public static Mesh GableRoof => roof!=null?roof:(roof=MakeRoof());

        public static Mesh Blob(float roughness,int seed)
        {
            int key=Mathf.RoundToInt(roughness*1000)*4096+(seed&4095);
            if(blobs.TryGetValue(key,out Mesh found))return found;
            Mesh mesh=SphereMesh(Mathf.Clamp(roughness,0f,.2f),seed);
            blobs[key]=mesh;
            return mesh;
        }

        private static Mesh SphereMesh(float rough,int seed)
        {
            const int slices=18, stacks=12;
            List<Vector3> v=new List<Vector3>();
            List<Vector2> uv=new List<Vector2>();
            List<int> tr=new List<int>();
            for(int j=0;j<=stacks;j++)
            {
                float latitude=(float)j/stacks*Mathf.PI;
                for(int i=0;i<=slices;i++)
                {
                    float phi=(float)i/slices*Mathf.PI*2f;
                    float wave=1f+rough*(.47f*Mathf.Sin(phi*3f+seed)*Mathf.Sin(latitude*3.4f)
                        +.43f*Mathf.Cos(phi*5f-seed*.5f)*Mathf.Sin(latitude*2.2f));
                    Vector3 p=new Vector3(Mathf.Sin(latitude)*Mathf.Cos(phi),
                        Mathf.Cos(latitude),Mathf.Sin(latitude)*Mathf.Sin(phi))*.5f*wave;
                    v.Add(p);
                    uv.Add(new Vector2((float)i/slices,(float)j/stacks));
                }
            }
            for(int j=0;j<stacks;j++)
                for(int i=0;i<slices;i++)
                {
                    int a=j*(slices+1)+i;
                    int b=a+slices+1;
                    tr.Add(a);tr.Add(a+1);tr.Add(b);
                    tr.Add(a+1);tr.Add(b+1);tr.Add(b);
                }
            return Finish("Organic sculpted mesh",v,uv,tr);
        }

        private static Mesh Rounded()
        {
            const int n=7;
            List<Vector3> v=new List<Vector3>();
            List<Vector2> uv=new List<Vector2>();
            List<int> tri=new List<int>();
            Vector3[] normals={Vector3.right,Vector3.left,Vector3.up,
                Vector3.down,Vector3.forward,Vector3.back};
            for(int f=0;f<6;f++)
            {
                Vector3 normal=normals[f];
                Vector3 u=Vector3.Cross(normal,Vector3.up);
                if(u.sqrMagnitude<.2f)u=Vector3.Cross(normal,Vector3.forward);
                u.Normalize();
                Vector3 w=Vector3.Cross(normal,u).normalized;
                int first=v.Count;
                for(int j=0;j<=n;j++)
                    for(int i=0;i<=n;i++)
                    {
                        Vector3 p=normal*.5f+u*((float)i/n-.5f)+w*((float)j/n-.5f);
                        Vector3 inner=new Vector3(Mathf.Clamp(p.x,-.38f,.38f),
                            Mathf.Clamp(p.y,-.38f,.38f),Mathf.Clamp(p.z,-.38f,.38f));
                        Vector3 delta=p-inner;
                        v.Add(inner+(delta.sqrMagnitude>1e-5f?delta.normalized*.12f:normal*.12f));
                        uv.Add(new Vector2((float)i/n,(float)j/n));
                    }
                for(int j=0;j<n;j++)
                    for(int i=0;i<n;i++)
                    {
                        int a=first+j*(n+1)+i;
                        int b=a+n+1;
                        tri.Add(a);tri.Add(a+1);tri.Add(b);
                        tri.Add(a+1);tri.Add(b+1);tri.Add(b);
                    }
            }
            return Finish("Bevel-edged sculpted box",v,uv,tri);
        }

        private static Mesh Lathe(Vector2[] section,int sides)
        {
            List<Vector3> v=new List<Vector3>();
            List<Vector2> uv=new List<Vector2>();
            List<int> tri=new List<int>();
            for(int j=0;j<section.Length;j++)
                for(int i=0;i<=sides;i++)
                {
                    float a=(float)i/sides*Mathf.PI*2f;
                    v.Add(new Vector3(Mathf.Cos(a)*section[j].x,
                        section[j].y,Mathf.Sin(a)*section[j].x));
                    uv.Add(new Vector2((float)i/sides,(float)j/(section.Length-1)));
                }
            for(int j=0;j<section.Length-1;j++)
                for(int i=0;i<sides;i++)
                {
                    int a=j*(sides+1)+i,b=a+sides+1;
                    tri.Add(a);tri.Add(b);tri.Add(a+1);
                    tri.Add(a+1);tri.Add(b);tri.Add(b+1);
                }
            return Finish("Turned organic profile",v,uv,tri);
        }

        private static Mesh MakeRoof()
        {
            // x = width, y = height; z = depth, supplied by localScale.
            Vector3[] p={
                new Vector3(-.5f,0,-.5f),new Vector3(0,.5f,-.5f),new Vector3(.5f,0,-.5f),
                new Vector3(-.5f,0,.5f),new Vector3(0,.5f,.5f),new Vector3(.5f,0,.5f)
            };
            int[] faces={
                0,3,1,1,3,4,
                1,4,2,2,4,5,
                0,1,2,
                3,5,4
            };
            List<Vector3> v=new List<Vector3>(p);
            List<Vector2> uv=new List<Vector2>{
                Vector2.zero,new Vector2(.5f,1),Vector2.right,
                Vector2.up,new Vector2(.5f,1),Vector2.one};
            return Finish("Gabled cottage roof",v,uv,new List<int>(faces));
        }

        private static Mesh Finish(string name,List<Vector3> v,List<Vector2> uv,List<int> tr)
        {
            Mesh mesh=new Mesh { name=name };
            mesh.SetVertices(v);
            mesh.SetUVs(0,uv);
            mesh.SetTriangles(tr,0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
