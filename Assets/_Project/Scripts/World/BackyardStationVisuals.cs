using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Adds readable, collider-free procedural station dressing while preserving
    /// the original root collider and gameplay components.
    /// </summary>
    public static class BackyardStationVisuals
    {
        public enum StationStyle
        {
            Workshop,
            Orders,
            Journal,
            Arcade,
            Collection
        }

        public static void Build(GameObject root, StationStyle style)
        {
            if (root == null) return;
            Renderer rootRenderer = root.GetComponent<Renderer>();
            if (rootRenderer != null) rootRenderer.enabled = false;

            Vector3 p = root.transform.position;
            switch (style)
            {
                case StationStyle.Workshop: BuildWorkshop(root.transform, p); break;
                case StationStyle.Orders: BuildBoard(root.transform, p, false); break;
                case StationStyle.Journal: BuildBoard(root.transform, p, true); break;
                case StationStyle.Arcade: BuildArcade(root.transform, p); break;
                case StationStyle.Collection: BuildCollection(root.transform, p); break;
            }
        }

        private static void BuildWorkshop(Transform parent, Vector3 p)
        {
            Material timber = Mat(new Color(0.48f, 0.27f, 0.13f));
            Material lightWood = Mat(new Color(0.69f, 0.46f, 0.25f));
            Material cream = Mat(new Color(0.94f, 0.84f, 0.64f));
            Material copper = Mat(new Color(0.72f, 0.39f, 0.19f));
            Material green = Mat(new Color(0.25f, 0.48f, 0.22f));

            Part(parent, PrimitiveType.Cube, "Workbench_Top", p + new Vector3(0f, 0.62f, 0f), new Vector3(2.15f, 0.16f, 1.35f), lightWood);
            for (int x = -1; x <= 1; x += 2)
            for (int z = -1; z <= 1; z += 2)
                Part(parent, PrimitiveType.Cube, "Workbench_Leg", p + new Vector3(x * 0.82f, 0.24f, z * 0.48f),
                    new Vector3(0.16f, 0.62f, 0.16f), timber);
            Part(parent, PrimitiveType.Cube, "Workbench_Front_Apron", p + new Vector3(0f, 0.36f, -0.58f),
                new Vector3(1.75f, 0.22f, 0.10f), timber);
            Part(parent, PrimitiveType.Cylinder, "Cooking_Pot", p + new Vector3(0.22f, 0.91f, 0.05f),
                new Vector3(0.62f, 0.28f, 0.62f), copper);
            Part(parent, PrimitiveType.Cylinder, "Cooking_Pot_Rim", p + new Vector3(0.22f, 1.20f, 0.05f),
                new Vector3(0.68f, 0.045f, 0.68f), cream);
            Part(parent, PrimitiveType.Sphere, "Herb_Bundle", p + new Vector3(-0.62f, 0.88f, 0.18f),
                new Vector3(0.24f, 0.12f, 0.38f), green);
            Part(parent, PrimitiveType.Cube, "Recipe_Book", p + new Vector3(-0.62f, 0.76f, -0.32f),
                new Vector3(0.42f, 0.07f, 0.34f), cream);
        }

        private static void BuildBoard(Transform parent, Vector3 p, bool journal)
        {
            Material frame = Mat(new Color(0.37f, 0.22f, 0.12f));
            Material panel = Mat(journal ? new Color(0.88f, 0.76f, 0.53f) : new Color(0.76f, 0.61f, 0.39f));
            Material paper = Mat(new Color(0.97f, 0.90f, 0.72f));
            Material accent = Mat(journal ? new Color(0.72f, 0.27f, 0.19f) : new Color(0.23f, 0.43f, 0.67f));
            float width = journal ? 1.12f : 1.32f;
            float height = journal ? 1.78f : 1.88f;

            Part(parent, PrimitiveType.Cube, "Board_Back_Panel", p, new Vector3(width, height, 0.12f), panel);
            for (int x = -1; x <= 1; x += 2)
                Part(parent, PrimitiveType.Cube, "Board_Frame_Side", p + new Vector3(x * (width * 0.5f + 0.035f), 0f, -0.015f),
                    new Vector3(0.10f, height + 0.16f, 0.18f), frame);
            for (int y = -1; y <= 1; y += 2)
                Part(parent, PrimitiveType.Cube, "Board_Frame_Rail", p + new Vector3(0f, y * (height * 0.5f + 0.035f), -0.015f),
                    new Vector3(width + 0.16f, 0.10f, 0.18f), frame);

            Part(parent, PrimitiveType.Cube, "Notice_Paper_Main", p + new Vector3(0f, 0.12f, -0.105f),
                new Vector3(width * 0.68f, height * 0.48f, 0.025f), paper);
            Part(parent, PrimitiveType.Cube, "Notice_Paper_Left", p + new Vector3(-width * 0.27f, -0.48f, -0.11f),
                new Vector3(width * 0.25f, height * 0.20f, 0.03f), paper);
            Part(parent, PrimitiveType.Cube, "Notice_Paper_Right", p + new Vector3(width * 0.27f, -0.48f, -0.11f),
                new Vector3(width * 0.25f, height * 0.20f, 0.03f), paper);
            Part(parent, PrimitiveType.Sphere, "Board_Header_Emblem", p + new Vector3(0f, height * 0.34f, -0.17f),
                Vector3.one * 0.22f, accent);
            for (int i = 0; i < 3; i++)
                Part(parent, PrimitiveType.Cube, "Notice_Line", p + new Vector3(0f, 0.20f - i * 0.16f, -0.13f),
                    new Vector3(width * (0.42f - i * 0.035f), 0.025f, 0.02f), accent);
            Part(parent, PrimitiveType.Cylinder, "Board_Stand", p + new Vector3(0f, -height * 0.5f - 0.32f, 0.03f),
                new Vector3(0.16f, 0.40f, 0.16f), frame);
        }

        private static void BuildArcade(Transform parent, Vector3 p)
        {
            Material cabinet = Mat(new Color(0.16f, 0.28f, 0.52f));
            Material trim = Mat(new Color(0.91f, 0.66f, 0.25f));
            Material screen = Mat(new Color(0.12f, 0.22f, 0.29f));
            Material red = Mat(new Color(0.83f, 0.20f, 0.19f));
            Material white = Mat(new Color(0.94f, 0.91f, 0.82f));

            Part(parent, PrimitiveType.Cube, "Arcade_Cabinet_Body", p + new Vector3(0f, 0.05f, 0f),
                new Vector3(1.18f, 1.92f, 0.52f), cabinet);
            Part(parent, PrimitiveType.Cube, "Arcade_Marquee", p + new Vector3(0f, 0.83f, -0.29f),
                new Vector3(0.92f, 0.34f, 0.07f), trim);
            Part(parent, PrimitiveType.Cube, "Arcade_Screen", p + new Vector3(0f, 0.25f, -0.30f),
                new Vector3(0.91f, 0.78f, 0.06f), screen);
            Part(parent, PrimitiveType.Cube, "Arcade_Control_Deck", p + new Vector3(0f, -0.32f, -0.37f),
                new Vector3(1.00f, 0.16f, 0.36f), trim);
            Part(parent, PrimitiveType.Cylinder, "Arcade_Joystick_Base", p + new Vector3(-0.25f, -0.19f, -0.42f),
                new Vector3(0.14f, 0.05f, 0.14f), white);
            Part(parent, PrimitiveType.Sphere, "Arcade_Joystick_Ball", p + new Vector3(-0.25f, -0.10f, -0.42f),
                Vector3.one * 0.12f, red);
            Part(parent, PrimitiveType.Sphere, "Arcade_Button_Red", p + new Vector3(0.20f, -0.19f, -0.42f),
                Vector3.one * 0.13f, red);
            Part(parent, PrimitiveType.Sphere, "Arcade_Button_White", p + new Vector3(0.40f, -0.19f, -0.42f),
                Vector3.one * 0.13f, white);
            Part(parent, PrimitiveType.Cube, "Arcade_Base", p + new Vector3(0f, -0.91f, 0f),
                new Vector3(1.28f, 0.16f, 0.62f), cabinet);
        }

        private static void BuildCollection(Transform parent, Vector3 p)
        {
            Material wood = Mat(new Color(0.43f, 0.25f, 0.13f));
            Material leather = Mat(new Color(0.48f, 0.22f, 0.16f));
            Material pages = Mat(new Color(0.94f, 0.84f, 0.64f));
            Material gold = Mat(new Color(0.91f, 0.68f, 0.25f));

            Part(parent, PrimitiveType.Cube, "Collection_Pedestal", p + new Vector3(0f, -0.72f, 0f),
                new Vector3(1.30f, 0.28f, 0.55f), wood);
            Part(parent, PrimitiveType.Cube, "Collection_Book_Pages", p + new Vector3(0f, 0.03f, -0.12f),
                new Vector3(0.88f, 1.08f, 0.18f), pages);
            Part(parent, PrimitiveType.Cube, "Collection_Book_Cover", p + new Vector3(0f, 0.03f, -0.23f),
                new Vector3(0.98f, 1.16f, 0.09f), leather);
            Part(parent, PrimitiveType.Cube, "Collection_Book_Spine", p + new Vector3(-0.48f, 0.03f, -0.29f),
                new Vector3(0.12f, 1.16f, 0.10f), gold);
            Part(parent, PrimitiveType.Sphere, "Collection_Book_Emblem", p + new Vector3(0.12f, 0.10f, -0.30f),
                Vector3.one * 0.26f, gold);
        }

        private static GameObject Part(Transform parent, PrimitiveType type, string name,
            Vector3 position, Vector3 scale, Material material)
        {
            GameObject part = GameObject.CreatePrimitive(type);
            part.name = name;
            part.transform.position = position;
            part.transform.localScale = scale;
            part.transform.SetParent(parent, true);
            Renderer renderer = part.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = material;
            Collider collider = part.GetComponent<Collider>();
            if (collider != null) Object.Destroy(collider);
            return part;
        }

        private static Material Mat(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            Material material = new Material(shader);
            material.color = color;
            return material;
        }
    }
}