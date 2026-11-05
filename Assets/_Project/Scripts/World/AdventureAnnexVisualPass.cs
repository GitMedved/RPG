using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Adds lightweight, collider-free visual dressing to the two story annexes.
    /// All objects are children of the existing gated content roots.
    /// </summary>
    public static class AdventureAnnexVisualPass
    {
        public static void BuildFlowerGarden(Transform parent)
        {
            if (parent == null) return;
            Material soil = Mat(new Color(0.31f, 0.22f, 0.13f));
            Material stone = Mat(new Color(0.69f, 0.67f, 0.58f));
            Material paleStone = Mat(new Color(0.82f, 0.79f, 0.68f));
            Material leaf = Mat(new Color(0.18f, 0.43f, 0.18f));
            Material leafLight = Mat(new Color(0.31f, 0.57f, 0.23f));
            Material pink = Mat(new Color(0.96f, 0.42f, 0.63f));
            Material yellow = Mat(new Color(0.99f, 0.77f, 0.18f));
            Material purple = Mat(new Color(0.61f, 0.42f, 0.87f));
            Material white = Mat(new Color(0.96f, 0.91f, 0.83f));
            Material water = Mat(new Color(0.36f, 0.68f, 0.78f));

            // Raised planting beds frame the path without covering the three resource nodes.
            Bed(parent, "Flower_Bed_Left", new Vector3(9.5f, 0.12f, 9.15f), new Vector3(1.25f, 0.20f, 1.15f), soil, stone);
            Bed(parent, "Flower_Bed_Right", new Vector3(13.1f, 0.12f, 9.15f), new Vector3(1.25f, 0.20f, 1.15f), soil, stone);
            Bed(parent, "Flower_Bed_Back", new Vector3(11.2f, 0.12f, 11.55f), new Vector3(2.5f, 0.18f, 0.62f), soil, paleStone);

            FlowerCluster(parent, new Vector3(9.1f, 0.32f, 8.15f), pink, leaf, 3, 0.0f);
            FlowerCluster(parent, new Vector3(13.5f, 0.32f, 8.25f), yellow, leafLight, 3, 0.7f);
            FlowerCluster(parent, new Vector3(10.8f, 0.32f, 11.55f), purple, leaf, 4, 1.4f);
            FlowerCluster(parent, new Vector3(13.8f, 0.32f, 11.25f), white, leafLight, 3, 2.1f);

            // Small decorative water bowl and stepping stones make the annex feel like a garden room.
            Part(parent, PrimitiveType.Cylinder, "Garden_Bird_Bath_Bowl", new Vector3(11.2f, 0.20f, 8.0f),
                new Vector3(0.75f, 0.12f, 0.75f), stone);
            Part(parent, PrimitiveType.Cylinder, "Garden_Bird_Bath_Water", new Vector3(11.2f, 0.30f, 8.0f),
                new Vector3(0.57f, 0.025f, 0.57f), water);
            for (int i = 0; i < 4; i++)
            {
                float z = 8.4f + i * 0.72f;
                Part(parent, PrimitiveType.Cylinder, "Garden_Path_Stepping_Stone",
                    new Vector3(11.2f, 0.10f, z), new Vector3(0.34f, 0.07f, 0.34f), i % 2 == 0 ? paleStone : stone);
            }
            for (int i = 0; i < 5; i++)
            {
                float x = 9.2f + i * 1.0f;
                Part(parent, PrimitiveType.Sphere, "Garden_Fence_Base_Shrub",
                    new Vector3(x, 0.38f, 11.95f), new Vector3(0.50f, 0.38f, 0.42f), i % 2 == 0 ? leaf : leafLight);
            }
            parent.gameObject.AddComponent<AdventureAnnexAtmospherePrototype>().Configure(false);
        }

        public static void BuildWoodlandTrail(Transform parent)
        {
            if (parent == null) return;
            Material moss = Mat(new Color(0.20f, 0.36f, 0.15f));
            Material mossLight = Mat(new Color(0.31f, 0.46f, 0.19f));
            Material stone = Mat(new Color(0.48f, 0.48f, 0.40f));
            Material stoneLight = Mat(new Color(0.63f, 0.60f, 0.50f));
            Material bark = Mat(new Color(0.29f, 0.17f, 0.10f));
            Material mushroomCap = Mat(new Color(0.75f, 0.28f, 0.20f));
            Material mushroomStem = Mat(new Color(0.91f, 0.83f, 0.66f));
            Material fern = Mat(new Color(0.19f, 0.47f, 0.22f));
            Material berry = Mat(new Color(0.68f, 0.12f, 0.22f));

            // Layered canopies and mossy boulders add depth while leaving the trail corridor open.
            Vector3[] canopyPositions =
            {
                new Vector3(-33.2f, 4.2f, -8.2f), new Vector3(-22.4f, 4.1f, -8.0f),
                new Vector3(-33.0f, 4.1f, 1.2f), new Vector3(-23.0f, 4.2f, 1.5f)
            };
            for (int i = 0; i < canopyPositions.Length; i++)
            {
                Vector3 p = canopyPositions[i];
                Part(parent, PrimitiveType.Sphere, "Woodland_Canopy_Upper",
                    p + new Vector3(0.55f, 0.25f, 0.35f), new Vector3(2.0f, 1.9f, 2.0f), i % 2 == 0 ? mossLight : moss);
                Part(parent, PrimitiveType.Sphere, "Woodland_Canopy_Side",
                    p + new Vector3(-0.85f, -0.15f, 0.1f), new Vector3(1.65f, 1.5f, 1.65f), moss);
            }

            for (int i = 0; i < 8; i++)
            {
                float x = -33.0f + (i % 4) * 2.1f;
                float z = -7.2f + (i / 4) * 7.0f;
                Part(parent, PrimitiveType.Sphere, "Woodland_Mossy_Rock",
                    new Vector3(x, 0.27f, z), new Vector3(0.85f, 0.48f, 0.68f), i % 2 == 0 ? stone : stoneLight);
                Part(parent, PrimitiveType.Sphere, "Woodland_Rock_Moss",
                    new Vector3(x - 0.08f, 0.49f, z - 0.08f), new Vector3(0.48f, 0.10f, 0.34f), moss);
            }

            // Fern-like foliage sits at the forest edge, not on the central walking line.
            Vector3[] fernPositions =
            {
                new Vector3(-32.0f, 0.18f, -4.0f), new Vector3(-23.8f, 0.18f, -4.7f),
                new Vector3(-32.1f, 0.18f, -0.5f), new Vector3(-23.6f, 0.18f, -0.2f)
            };
            for (int i = 0; i < fernPositions.Length; i++)
            {
                Vector3 p = fernPositions[i];
                for (int leafIndex = 0; leafIndex < 5; leafIndex++)
                {
                    float angle = leafIndex * 72f;
                    GameObject frond = Part(parent, PrimitiveType.Sphere, "Woodland_Fern_Frond",
                        p + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * 0.30f, 0.18f,
                            Mathf.Sin(angle * Mathf.Deg2Rad) * 0.30f),
                        new Vector3(0.22f, 0.08f, 0.52f), fern);
                    frond.transform.rotation = Quaternion.Euler(0f, angle, -24f);
                }
            }

            // A fallen branch and tiny berry clusters provide readable woodland landmarks.
            GameObject log = Part(parent, PrimitiveType.Cylinder, "Woodland_Fallen_Log",
                new Vector3(-27.8f, 0.26f, 1.1f), new Vector3(0.28f, 1.35f, 0.28f), bark);
            log.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            for (int i = 0; i < 3; i++)
            {
                Vector3 p = new Vector3(-31.6f + i * 0.45f, 0.38f, -1.5f);
                Part(parent, PrimitiveType.Sphere, "Woodland_Berry_Cluster", p,
                    new Vector3(0.38f, 0.30f, 0.36f), berry);
            }
            Mushroom(parent, new Vector3(-32.1f, 0.12f, -5.4f), mushroomStem, mushroomCap);
            Mushroom(parent, new Vector3(-24.0f, 0.12f, 0.2f), mushroomStem, mushroomCap);
            parent.gameObject.AddComponent<AdventureAnnexAtmospherePrototype>().Configure(true);
        }

        private static void Bed(Transform parent, string name, Vector3 p, Vector3 size, Material soil, Material edge)
        {
            Part(parent, PrimitiveType.Cube, name + "_Soil", p, size, soil);
            float y = p.y + size.y * 0.5f + 0.07f;
            Part(parent, PrimitiveType.Cube, name + "_Front_Edge",
                new Vector3(p.x, y, p.z - size.z * 0.5f), new Vector3(size.x + 0.12f, 0.14f, 0.10f), edge);
            Part(parent, PrimitiveType.Cube, name + "_Back_Edge",
                new Vector3(p.x, y, p.z + size.z * 0.5f), new Vector3(size.x + 0.12f, 0.14f, 0.10f), edge);
            Part(parent, PrimitiveType.Cube, name + "_Left_Edge",
                new Vector3(p.x - size.x * 0.5f, y, p.z), new Vector3(0.10f, 0.14f, size.z), edge);
            Part(parent, PrimitiveType.Cube, name + "_Right_Edge",
                new Vector3(p.x + size.x * 0.5f, y, p.z), new Vector3(0.10f, 0.14f, size.z), edge);
        }

        private static void FlowerCluster(Transform parent, Vector3 origin, Material petals, Material leaves, int count, float phase)
        {
            for (int i = 0; i < count; i++)
            {
                float angle = phase + i * (Mathf.PI * 2f / count);
                Vector3 offset = new Vector3(Mathf.Cos(angle) * 0.36f, 0f, Mathf.Sin(angle) * 0.30f);
                Vector3 p = origin + offset;
                Part(parent, PrimitiveType.Cylinder, "Garden_Flower_Stem", p + Vector3.up * 0.22f,
                    new Vector3(0.045f, 0.22f, 0.045f), leaves);
                Part(parent, PrimitiveType.Sphere, "Garden_Flower_Leaf", p + new Vector3(0.12f, 0.18f, 0f),
                    new Vector3(0.22f, 0.07f, 0.12f), leaves);
                Part(parent, PrimitiveType.Sphere, "Garden_Flower_Bloom", p + Vector3.up * 0.47f,
                    new Vector3(0.28f, 0.18f, 0.28f), petals);
                Part(parent, PrimitiveType.Sphere, "Garden_Flower_Center", p + Vector3.up * 0.50f,
                    Vector3.one * 0.09f, Mat(new Color(0.98f, 0.82f, 0.30f)));
            }
        }

        private static void Mushroom(Transform parent, Vector3 p, Material stem, Material cap)
        {
            Part(parent, PrimitiveType.Cylinder, "Woodland_Mushroom_Stem", p + Vector3.up * 0.20f,
                new Vector3(0.13f, 0.20f, 0.13f), stem);
            Part(parent, PrimitiveType.Sphere, "Woodland_Mushroom_Cap", p + Vector3.up * 0.42f,
                new Vector3(0.42f, 0.20f, 0.42f), cap);
        }

        private static GameObject Part(Transform parent, PrimitiveType type, string name,
            Vector3 position, Vector3 scale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = scale;
            go.transform.SetParent(parent, true);
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = material;
            Collider collider = go.GetComponent<Collider>();
            if (collider != null) Object.Destroy(collider);
            return go;
        }

        private static Material Mat(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            return new Material(shader) { color = color };
        }
    }
}