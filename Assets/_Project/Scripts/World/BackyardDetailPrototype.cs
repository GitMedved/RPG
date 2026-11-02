using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>Extra landmark and prop pass for the temporary backyard greybox.</summary>
    public static class BackyardDetailPrototype
    {
        public static void Build()
        {
            Material trim = Mat(new Color(0.93f, 0.89f, 0.78f));
            Material glass = Mat(new Color(0.34f, 0.62f, 0.73f));
            Material door = Mat(new Color(0.34f, 0.20f, 0.12f));
            Material wood = Mat(new Color(0.48f, 0.31f, 0.19f));
            Material dark = Mat(new Color(0.16f, 0.15f, 0.13f));
            Material flowerA = Mat(new Color(0.96f, 0.74f, 0.25f));
            Material flowerB = Mat(new Color(0.84f, 0.38f, 0.55f));
            Material pot = Mat(new Color(0.72f, 0.36f, 0.22f));
            Material leaf = Mat(new Color(0.20f, 0.43f, 0.16f));

            // Front facade: door, two framed windows and simple steps.
            Cube("House_Front_Door_Proxy", new Vector3(0f, 1.35f, 7.43f), new Vector3(1.45f, 2.7f, 0.12f), door);
            Cube("House_Door_Glass_Proxy", new Vector3(0f, 1.95f, 7.35f), new Vector3(0.82f, 1.15f, 0.035f), glass);
            Cube("House_Door_Handle_Proxy", new Vector3(0.48f, 1.25f, 7.28f), new Vector3(0.06f, 0.18f, 0.06f), dark);
            for (int side = -1; side <= 1; side += 2)
            {
                float x = side * 3.6f;
                Cube("House_Window_Frame_Proxy", new Vector3(x, 3.15f, 7.40f), new Vector3(2.0f, 1.65f, 0.12f), trim);
                Cube("House_Window_Glass_Proxy", new Vector3(x, 3.15f, 7.31f), new Vector3(1.72f, 1.37f, 0.04f), glass);
                Cube("House_Window_Mullion_Proxy", new Vector3(x, 3.15f, 7.25f), new Vector3(0.07f, 1.4f, 0.04f), trim);
                Cube("House_Window_Sill_Proxy", new Vector3(x, 2.28f, 7.29f), new Vector3(2.15f, 0.12f, 0.22f), trim);
            }
            Cube("House_Step_Upper_Proxy", new Vector3(0f, 0.20f, 7.10f), new Vector3(2.0f, 0.28f, 0.62f), trim);
            Cube("House_Step_Lower_Proxy", new Vector3(0f, 0.08f, 6.70f), new Vector3(2.4f, 0.16f, 0.62f), trim);

            // Patio seating: one low table and two simple chairs.
            Cube("Patio_Table_Top_Proxy", new Vector3(-2.7f, 0.85f, 5.6f), new Vector3(1.5f, 0.14f, 0.9f), wood);
            for (int x = -1; x <= 1; x += 2)
            {
                Cube("Patio_Table_Leg_Proxy", new Vector3(-2.7f + x * 0.55f, 0.43f, 5.6f), new Vector3(0.10f, 0.8f, 0.10f), dark);
                Cube("Patio_Chair_Seat_Proxy", new Vector3(-4.1f + (x + 1) * 1.2f, 0.52f, 5.6f), new Vector3(0.72f, 0.12f, 0.72f), wood);
                Cube("Patio_Chair_Back_Proxy", new Vector3(-4.1f + (x + 1) * 1.2f, 1.05f, 5.92f), new Vector3(0.72f, 0.95f, 0.12f), wood);
            }
            Cube("Patio_Grill_Body_Proxy", new Vector3(4.1f, 0.72f, 5.7f), new Vector3(1.0f, 0.85f, 0.8f), dark);
            Cube("Patio_Grill_Lid_Proxy", new Vector3(4.1f, 1.25f, 5.7f), new Vector3(1.08f, 0.30f, 0.88f), dark);

            // Flower beds and potted plants make the lawn feel inhabited.
            for (int row = 0; row < 2; row++)
            {
                Cube("Flower_Bed_Edge_Proxy", new Vector3(5.8f, 0.18f, 8.5f + row * 1.4f),
                    new Vector3(5.0f, 0.32f, 0.65f), wood);
                for (int i = 0; i < 5; i++)
                {
                    float x = 3.9f + i * 0.95f;
                    float z = 8.5f + row * 1.4f;
                    Sphere("Flower_Stem_Proxy", new Vector3(x, 0.50f, z), new Vector3(0.08f, 0.5f, 0.08f), leaf);
                    Sphere("Flower_Bloom_Proxy", new Vector3(x, 0.78f, z), Vector3.one * 0.24f, (i + row) % 2 == 0 ? flowerA : flowerB);
                }
            }
            for (int i = 0; i < 3; i++)
            {
                float x = -4.8f + i * 1.1f;
                Cylinder("Patio_Pot_Proxy", new Vector3(x, 0.35f, 6.7f), new Vector3(0.42f, 0.36f, 0.42f), pot);
                Sphere("Patio_Plant_Proxy", new Vector3(x, 0.85f, 6.7f), new Vector3(0.5f, 0.7f, 0.5f), leaf);
            }
        }

        private static Material Mat(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            return new Material(shader) { color = color };
        }

        private static GameObject Cube(string name, Vector3 pos, Vector3 scale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name; go.transform.position = pos; go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = material;
            return go;
        }

        private static GameObject Sphere(string name, Vector3 pos, Vector3 scale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name; go.transform.position = pos; go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = material;
            return go;
        }

        private static GameObject Cylinder(string name, Vector3 pos, Vector3 scale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name; go.transform.position = pos; go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = material;
            return go;
        }
    }
}
