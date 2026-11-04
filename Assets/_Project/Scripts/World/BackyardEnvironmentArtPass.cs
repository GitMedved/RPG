using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// M40: adds a coherent, warm cartoon-style visual layer to the backyard blockout.
    /// All pieces are lightweight decorative primitives with no colliders; production art
    /// and the supplied reference layout remain the source of truth.
    /// </summary>
    public static class BackyardEnvironmentArtPass
    {
        public static void Build()
        {
            GameObject root = new GameObject("M40_Backyard_Environment_Dressing");

            Material cream = Mat(new Color(0.98f, 0.91f, 0.75f));
            Material frame = Mat(new Color(0.91f, 0.86f, 0.72f));
            Material glass = Mat(new Color(0.42f, 0.72f, 0.82f));
            Material glassLight = Mat(new Color(0.68f, 0.86f, 0.88f));
            Material door = Mat(new Color(0.43f, 0.25f, 0.15f));
            Material doorLight = Mat(new Color(0.58f, 0.36f, 0.21f));
            Material brass = Mat(new Color(0.88f, 0.67f, 0.28f));
            Material soil = Mat(new Color(0.30f, 0.19f, 0.12f));
            Material timber = Mat(new Color(0.64f, 0.43f, 0.25f));
            Material terracotta = Mat(new Color(0.76f, 0.34f, 0.22f));
            Material planterGreen = Mat(new Color(0.24f, 0.43f, 0.19f));
            Material flowerPink = Mat(new Color(0.96f, 0.48f, 0.64f));
            Material flowerYellow = Mat(new Color(0.98f, 0.78f, 0.25f));

            BuildHouseFacade(root.transform, cream, frame, glass, glassLight, door, doorLight, brass);
            BuildGardenDetails(root.transform, timber);
            BuildPatioPlanters(root.transform, terracotta, planterGreen, flowerPink, flowerYellow);
            BuildSandboxTrim(root.transform, timber, cream);
        }

        private static void BuildHouseFacade(Transform parent, Material cream, Material frame,
            Material glass, Material glassLight, Material door, Material doorLight, Material brass)
        {
            // The front wall faces the open yard (negative Z).
            const float wallZ = 7.46f;

            Cube("House_Door_Frame", new Vector3(0f, 1.38f, wallZ - 0.035f),
                new Vector3(1.82f, 2.82f, 0.16f), cream, parent);
            Cube("House_Front_Door", new Vector3(0f, 1.32f, wallZ - 0.13f),
                new Vector3(1.48f, 2.55f, 0.10f), door, parent);
            Cube("House_Door_Inset", new Vector3(0f, 1.48f, wallZ - 0.19f),
                new Vector3(1.12f, 1.32f, 0.035f), doorLight, parent);
            Cube("House_Door_Lower_Panel", new Vector3(0f, 0.63f, wallZ - 0.19f),
                new Vector3(1.12f, 0.55f, 0.035f), doorLight, parent);
            Ellipsoid("House_Door_Handle", new Vector3(0.52f, 1.28f, wallZ - 0.25f),
                new Vector3(0.13f, 0.13f, 0.08f), brass, parent);
            Cube("House_Doorstep", new Vector3(0f, 0.10f, 7.12f),
                new Vector3(2.15f, 0.20f, 0.72f), cream, parent);

            BuildWindow(parent, "Left", new Vector3(-3.65f, 3.05f, wallZ - 0.07f),
                frame, glass, glassLight);
            BuildWindow(parent, "Right", new Vector3(3.65f, 3.05f, wallZ - 0.07f),
                frame, glass, glassLight);

            Cube("House_Front_Eave_Trim", new Vector3(0f, 4.88f, 7.39f),
                new Vector3(12.15f, 0.20f, 0.22f), cream, parent);
            Cube("House_Front_Foundation_Trim", new Vector3(0f, 0.18f, 7.43f),
                new Vector3(11.9f, 0.20f, 0.12f), frame, parent);
            Cube("House_Porch_Lintel", new Vector3(0f, 2.92f, 7.24f),
                new Vector3(2.12f, 0.13f, 0.16f), frame, parent);
        }

        private static void BuildWindow(Transform parent, string label, Vector3 center,
            Material frame, Material glass, Material glassLight)
        {
            Cube("House_Window_" + label + "_Glass", center,
                new Vector3(1.62f, 1.35f, 0.06f), glass, parent);
            Cube("House_Window_" + label + "_Highlight", center + new Vector3(-0.34f, 0f, -0.045f),
                new Vector3(0.34f, 1.15f, 0.025f), glassLight, parent);
            Cube("House_Window_" + label + "_Frame_Top", center + new Vector3(0f, 0.78f, -0.07f),
                new Vector3(1.92f, 0.14f, 0.16f), frame, parent);
            Cube("House_Window_" + label + "_Frame_Bottom", center + new Vector3(0f, -0.78f, -0.07f),
                new Vector3(1.92f, 0.14f, 0.16f), frame, parent);
            Cube("House_Window_" + label + "_Frame_Left", center + new Vector3(-0.89f, 0f, -0.07f),
                new Vector3(0.14f, 1.42f, 0.16f), frame, parent);
            Cube("House_Window_" + label + "_Frame_Right", center + new Vector3(0.89f, 0f, -0.07f),
                new Vector3(0.14f, 1.42f, 0.16f), frame, parent);
            Cube("House_Window_" + label + "_Mullion_Vertical", center + new Vector3(0f, 0f, -0.09f),
                new Vector3(0.09f, 1.32f, 0.10f), frame, parent);
            Cube("House_Window_" + label + "_Mullion_Horizontal", center + new Vector3(0f, 0f, -0.09f),
                new Vector3(1.58f, 0.09f, 0.10f), frame, parent);
            Cube("House_Window_" + label + "_Sill", center + new Vector3(0f, -0.91f, -0.12f),
                new Vector3(2.10f, 0.13f, 0.34f), frame, parent);
        }

        private static void BuildGardenDetails(Transform parent, Material timber)
        {
            // Slim edging frames the existing interactive plots without covering their crop visuals.
            for (int i = 0; i < 3; i++)
            {
                float x = 5f + i * 2.1f;
                float z = 1.5f;
                float y = 0.285f;
                Cube("Garden_Bed_" + (i + 1) + "_Front_Edge", new Vector3(x, y, z - 0.77f),
                    new Vector3(1.82f, 0.10f, 0.10f), timber, parent);
                Cube("Garden_Bed_" + (i + 1) + "_Back_Edge", new Vector3(x, y, z + 0.77f),
                    new Vector3(1.82f, 0.10f, 0.10f), timber, parent);
                Cube("Garden_Bed_" + (i + 1) + "_Left_Edge", new Vector3(x - 0.86f, y, z),
                    new Vector3(0.10f, 0.10f, 1.44f), timber, parent);
                Cube("Garden_Bed_" + (i + 1) + "_Right_Edge", new Vector3(x + 0.86f, y, z),
                    new Vector3(0.10f, 0.10f, 1.44f), timber, parent);
            }

            // A short branch of stepping stones visually connects the patio to the growing area.
            for (int i = 0; i < 4; i++)
            {
                float z = 4.35f - i * 0.72f;
                float x = 4.15f + i * 0.22f;
                Ellipsoid("Garden_Path_Stone_" + i, new Vector3(x, 0.045f, z),
                    new Vector3(0.76f, 0.07f, 0.48f), i % 2 == 0 ? timber : soil, parent);
            }
        }

        private static void BuildPatioPlanters(Transform parent, Material pot, Material leaves,
            Material pink, Material yellow)
        {
            CreatePlanter(parent, "Left", new Vector3(-3.65f, 0.42f, 5.25f), pot, leaves, pink);
            CreatePlanter(parent, "Right", new Vector3(3.65f, 0.42f, 5.25f), pot, leaves, yellow);
        }

        private static void CreatePlanter(Transform parent, string label, Vector3 position,
            Material pot, Material leaves, Material bloom)
        {
            Cylinder("Patio_Planter_" + label + "_Pot", position,
                new Vector3(0.62f, 0.42f, 0.62f), pot, parent);
            for (int i = 0; i < 3; i++)
            {
                float angle = i * 2.094f;
                Vector3 offset = new Vector3(Mathf.Cos(angle) * 0.18f, 0.43f,
                    Mathf.Sin(angle) * 0.18f);
                Cylinder("Patio_Planter_" + label + "_Stem_" + i, position + offset + Vector3.up * 0.15f,
                    new Vector3(0.045f, 0.22f, 0.045f), leaves, parent);
                Ellipsoid("Patio_Planter_" + label + "_Leaf_" + i, position + offset + Vector3.up * 0.32f,
                    new Vector3(0.24f, 0.10f, 0.15f), leaves, parent);
                Ellipsoid("Patio_Planter_" + label + "_Bloom_" + i, position + offset + Vector3.up * 0.48f,
                    new Vector3(0.16f, 0.13f, 0.16f), bloom, parent);
            }
        }

        private static void BuildSandboxTrim(Transform parent, Material timber, Material cream)
        {
            Cube("Sandbox_Outer_Trim_Front", new Vector3(10f, 0.47f, -8.72f),
                new Vector3(4.62f, 0.12f, 0.16f), cream, parent);
            Cube("Sandbox_Outer_Trim_Back", new Vector3(10f, 0.47f, -5.28f),
                new Vector3(4.62f, 0.12f, 0.16f), cream, parent);
            Cube("Sandbox_Outer_Trim_Left", new Vector3(7.72f, 0.47f, -7f),
                new Vector3(0.16f, 0.12f, 3.30f), cream, parent);
            Cube("Sandbox_Outer_Trim_Right", new Vector3(12.28f, 0.47f, -7f),
                new Vector3(0.16f, 0.12f, 3.30f), cream, parent);
            Cube("Sandbox_Toy_Block", new Vector3(9.0f, 0.62f, -7.25f),
                new Vector3(0.38f, 0.30f, 0.38f), timber, parent);
        }

        private static Material Mat(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            Material material = new Material(shader) { color = color };
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", 0.18f);
            return material;
        }

        private static GameObject Cube(string name, Vector3 position, Vector3 scale,
            Material material, Transform parent)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, true);
            go.transform.position = position;
            go.transform.localScale = scale;
            Apply(go, material);
            return go;
        }

        private static GameObject Ellipsoid(string name, Vector3 position, Vector3 scale,
            Material material, Transform parent)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.SetParent(parent, true);
            go.transform.position = position;
            go.transform.localScale = scale;
            Apply(go, material);
            return go;
        }

        private static GameObject Cylinder(string name, Vector3 position, Vector3 scale,
            Material material, Transform parent)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            go.transform.SetParent(parent, true);
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
