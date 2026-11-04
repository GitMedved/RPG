using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// A lightweight visual polish layer for the greybox yard. All geometry is temporary,
    /// low-poly primitive dressing and is deliberately separate from production art assets.
    /// </summary>
    public static class BackyardVisualPolishPrototype
    {
        public static void Build()
        {
            Material stone = Mat(new Color(0.68f, 0.65f, 0.57f));
            Material warmStone = Mat(new Color(0.78f, 0.72f, 0.60f));
            Material leafLight = Mat(new Color(0.27f, 0.52f, 0.20f));
            Material leafDark = Mat(new Color(0.12f, 0.32f, 0.12f));
            Material flowerPink = Mat(new Color(0.96f, 0.48f, 0.64f));
            Material flowerYellow = Mat(new Color(0.98f, 0.78f, 0.25f));
            Material flowerWhite = Mat(new Color(0.96f, 0.91f, 0.78f));
            Material wood = Mat(new Color(0.56f, 0.36f, 0.22f));
            Material trim = Mat(new Color(0.94f, 0.91f, 0.82f));

            // A soft stepping-stone route from the patio toward the play area.
            for (int i = 0; i < 8; i++)
            {
                float z = 5.0f - i * 1.65f;
                float x = Mathf.Sin(i * 0.65f) * 0.65f;
                Ellipsoid("Path_Stone_" + i, new Vector3(x, 0.035f, z),
                    new Vector3(0.85f, 0.07f, 0.62f), i % 2 == 0 ? warmStone : stone);
            }

            // Layered canopy clusters make the existing landmark tree read more softly.
            Vector3 tree = new Vector3(-7f, 5f, 2f);
            Ellipsoid("Tree_Canopy_Left", tree + new Vector3(-1.25f, 0.10f, 0.10f),
                new Vector3(2.7f, 2.5f, 2.7f), leafDark);
            Ellipsoid("Tree_Canopy_Right", tree + new Vector3(1.15f, -0.05f, -0.05f),
                new Vector3(2.6f, 2.4f, 2.6f), leafLight);
            Ellipsoid("Tree_Canopy_Top", tree + new Vector3(0f, 1.0f, 0f),
                new Vector3(2.8f, 2.5f, 2.8f), leafLight);

            // Small flower clusters along the lawn edge; keep the central play space clear.
            Vector3[] beds =
            {
                new Vector3(-13f, 0.35f, 9.4f),
                new Vector3(13.0f, 0.35f, 8.0f),
                new Vector3(-13.0f, 0.35f, -9.5f),
                new Vector3(13.0f, 0.35f, -11.0f)
            };
            for (int bed = 0; bed < beds.Length; bed++)
            {
                for (int i = 0; i < 5; i++)
                {
                    float x = beds[bed].x + (i - 2) * 0.62f;
                    float z = beds[bed].z + Mathf.Sin(i * 1.3f + bed) * 0.28f;
                    Cylinder("Flower_Stem_Detail", new Vector3(x, beds[bed].y, z),
                        new Vector3(0.045f, 0.24f, 0.045f), leafDark);
                    Ellipsoid("Flower_Leaf_Detail", new Vector3(x + 0.12f, beds[bed].y + 0.12f, z),
                        new Vector3(0.20f, 0.07f, 0.10f), leafLight);
                    Ellipsoid("Flower_Bloom_Detail", new Vector3(x, beds[bed].y + 0.30f, z),
                        new Vector3(0.20f, 0.13f, 0.20f),
                        (i + bed) % 3 == 0 ? flowerPink : (i + bed) % 3 == 1 ? flowerYellow : flowerWhite);
                }
            }

            // Decorative fence caps and house fascia give simple blockout shapes a finished silhouette.
            for (int x = -16; x <= 16; x += 2)
                Ellipsoid("Fence_Back_Post_Cap", new Vector3(x, 1.76f, 13.5f),
                    new Vector3(0.18f, 0.10f, 0.18f), trim);
            Cube("House_Front_Fascia", new Vector3(0f, 5.0f, 7.42f),
                new Vector3(12.4f, 0.16f, 0.16f), trim);
            Cube("Patio_Wood_Edge", new Vector3(0f, 0.11f, 4.82f),
                new Vector3(9.2f, 0.10f, 0.12f), wood);
        }

        private static Material Mat(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            Material material = new Material(shader) { color = color };
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", 0.12f);
            return material;
        }

        private static GameObject Cube(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = scale;
            Apply(go, material);
            return go;
        }

        private static GameObject Ellipsoid(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = scale;
            Apply(go, material);
            return go;
        }

        private static GameObject Cylinder(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = scale;
            Apply(go, material);
            return go;
        }

        private static void Apply(GameObject go, Material material)
        {
            Renderer renderer = go.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            Collider collider = go.GetComponent<Collider>();
            if (collider != null) Object.Destroy(collider);
        }
    }
}
