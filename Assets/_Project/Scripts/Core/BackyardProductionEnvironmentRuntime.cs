using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Adds reviewed production environment visuals to the generated backyard blockout.
    /// The gameplay scene, interaction objects and their colliders remain untouched.
    /// Prefabs are loaded from Resources/Environment and remain visual-only.
    /// </summary>
    public static class BackyardProductionEnvironmentRuntime
    {
        private const string ResourceFolder = "Environment/";

        public static void Apply()
        {
            if (GameObject.Find("Backyard_Production_Environment_Visuals") != null)
                return;

            GameObject root = new GameObject("Backyard_Production_Environment_Visuals");

            // Replace only the visible central-tree primitives. Their original objects
            // remain in place so existing interaction/collision behavior is preserved.
            HideRenderers("Main_Tree_Trunk");
            HideRenderers("Main_Tree_Canopy");
            Place(root.transform, "ENV_TreeBlocks", "TreeBlocks_Main", new Vector3(-7f, 0f, 2f));

            // Reviewed seating models enrich the patio without replacing gameplay objects.
            Place(root.transform, "ENV_Bench", "Patio_Bench", new Vector3(-2.7f, 0f, 6.0f),
                Quaternion.Euler(0f, 90f, 0f));
            Place(root.transform, "ENV_ChairRounded", "Patio_Chair_Left", new Vector3(1.8f, 0f, 6.0f),
                Quaternion.Euler(0f, -90f, 0f));
            Place(root.transform, "ENV_ChairCushion", "Patio_Chair_Right", new Vector3(3.1f, 0f, 6.0f),
                Quaternion.Euler(0f, 90f, 0f));

            // Small plants and flower variants add restrained color around the lawn edge.
            Place(root.transform, "ENV_PlantSmall1", "Garden_Plant_01", new Vector3(-12.2f, 0f, 10.8f));
            Place(root.transform, "ENV_PlantSmall2", "Garden_Plant_02", new Vector3(-10.8f, 0f, 10.8f));
            Place(root.transform, "ENV_PlantSmall3", "Garden_Plant_03", new Vector3(-9.4f, 0f, 10.8f));
            Place(root.transform, "ENV_FlowerPurpleA", "Garden_Flower_01", new Vector3(8.2f, 0f, 2.4f));
            Place(root.transform, "ENV_FlowerPurpleB", "Garden_Flower_02", new Vector3(9.2f, 0f, 2.9f));
            Place(root.transform, "ENV_FlowerPurpleC", "Garden_Flower_03", new Vector3(10.2f, 0f, 2.4f));

            Debug.Log("[Mia & Max] Production environment visuals applied. Gameplay placeholders and interaction colliders were preserved.");
        }

        private static void Place(Transform parent, string resourceName, string instanceName,
            Vector3 position, Quaternion rotation = default)
        {
            GameObject prefab = Resources.Load<GameObject>(ResourceFolder + resourceName);
            if (prefab == null)
            {
                Debug.LogWarning("[Mia & Max] Production prefab not found in Resources: " + resourceName +
                    ". Run Tools > Mia & Max > Assets > Build Production Environment Prefabs.");
                return;
            }

            GameObject instance = Object.Instantiate(prefab, position,
                rotation == default ? Quaternion.identity : rotation, parent);
            instance.name = instanceName;
        }

        private static void HideRenderers(string objectName)
        {
            GameObject target = GameObject.Find(objectName);
            if (target == null) return;
            foreach (Renderer renderer in target.GetComponentsInChildren<Renderer>(true))
                renderer.enabled = false;
        }
    }
}