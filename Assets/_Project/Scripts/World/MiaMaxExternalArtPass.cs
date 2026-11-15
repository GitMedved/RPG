using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Bridges downloaded/committed CC0 native glTF assets into the authored scene.
    /// Existing handmade scene and animated character proxies remain fallback.
    /// glTFast converts .glb files under Resources into native Unity GameObjects.
    /// </summary>
    public static class MiaMaxExternalArtPass
    {
        private const string Nature = "MiaMax/External/Gobkit/";
        private const string Cast = "MiaMax/External/Cast/";

        private struct Placement
        {
            public string Name;
            public Vector3 Position;
            public float Height;
            public float Yaw;
            public Placement(string name, Vector3 position, float height, float yaw)
            {
                Name = name; Position = position; Height = height; Yaw = yaw;
            }
        }

        private static readonly Placement[] Environment =
        {
            new Placement("TreeHigh001",new Vector3(-8.0f,0f,3.15f),4.25f,24f),
            new Placement("TreeLow003",new Vector3(8.0f,0f,4.6f),3.05f,-27f),
            new Placement("Bush001",new Vector3(-7.3f,0f,-2.6f),.84f,16f),
            new Placement("Bush002",new Vector3(7.9f,0f,-2.65f),.75f,-31f),
            new Placement("Rock001",new Vector3(-6.9f,0f,-7.1f),.53f,-20f),
            new Placement("Rock002",new Vector3(7.0f,0f,-6.7f),.47f,40f),
            new Placement("Grass001",new Vector3(-2.15f,0f,-3.8f),.30f,18f),
            new Placement("Reed001",new Vector3(-5.2f,0f,-4.2f),.74f,-18f)
        };

        public static int AddNature(Transform parent)
        {
            Transform group = new GameObject("Imported CC0 GLBs - Gobkit Nature").transform;
            group.SetParent(parent, false);
            int installed = 0;
            foreach (Placement placement in Environment)
            {
                if (Spawn(Nature + placement.Name, group, placement.Position,
                    placement.Height, placement.Yaw, true) != null) installed++;
            }
            if (installed == 0) UnityEngine.Object.Destroy(group.gameObject);
            else Debug.Log("[Mia & Max Assets] Loaded " + installed +
                " real CC0 environmental GLB meshes (Gobkit).");
            return installed;
        }

        public static int ApplyRiggedCharacterOverrides(TapAdventureCharacterRig mia,
            TapAdventureCharacterRig max)
        {
            int count = 0;
            // Only enable explicitly approved prefabs: staging GLBs are not substitutes
            // for Mia/Max. Original reference likeness remains an art acceptance gate.
            if (TryReplace(mia, Cast + "MiaRig", 2.45f)) count++;
            if (TryReplace(max, Cast + "MaxRig", 1.35f)) count++;
            return count;
        }

        private static bool TryReplace(TapAdventureCharacterRig proxy,
            string path, float targetHeight)
        {
            if (proxy == null) return false;
            GameObject resource = Resources.Load<GameObject>(path);
            if (resource == null) return false;
            // Do not hide the current character unless the replacement renders.
            GameObject real = Spawn(path, proxy.transform,
                proxy.transform.position, targetHeight, 180f, false);
            if (real == null) return false;
            real.name = proxy.name + " - Imported Approved Production Rig";
            foreach (Renderer renderer in proxy.GetComponentsInChildren<Renderer>(true))
                if (!renderer.transform.IsChildOf(real.transform)) renderer.enabled = false;
            // Animation clips are imported with glTF/FBX if supported; the Editor
            // approval step creates a native AnimatorController on the prefab.
            if (real.GetComponent<MiaMaxRigAnimator>() == null)
                real.AddComponent<MiaMaxRigAnimator>();
            return true;
        }

        private static GameObject Spawn(string resourcePath, Transform parent,
            Vector3 position, float targetHeight, float yaw, bool disableColliders)
        {
            GameObject prefab = Resources.Load<GameObject>(resourcePath);
            if (prefab == null) return null;
            GameObject obj = null;
            try
            {
                obj = UnityEngine.Object.Instantiate(prefab,position,
                    Quaternion.Euler(0f,yaw,0f),parent);
                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
                if (renderers.Length == 0)
                    throw new InvalidOperationException("Imported model has no renderers: " + resourcePath);
                Bounds bounds = renderers[0].bounds;
                for (int i=1;i<renderers.Length;i++) bounds.Encapsulate(renderers[i].bounds);
                float height = Mathf.Max(.01f,bounds.size.y);
                float factor = Mathf.Clamp(targetHeight/height,.1f,15f);
                obj.transform.localScale *= factor;
                // Bounding boxes must be re-read after applying final scale.
                renderers = obj.GetComponentsInChildren<Renderer>(true);
                bounds = renderers[0].bounds;
                for (int i=1;i<renderers.Length;i++) bounds.Encapsulate(renderers[i].bounds);
                obj.transform.position += Vector3.up * (position.y - bounds.min.y);
                if (disableColliders)
                    foreach (Collider collider in obj.GetComponentsInChildren<Collider>(true))
                        collider.enabled=false;
                return obj;
            }
            catch(Exception e)
            {
                Debug.LogWarning("[Mia & Max Assets] Could not display "+resourcePath+
                    ": "+e.Message);
                if(obj!=null)UnityEngine.Object.Destroy(obj);
                return null;
            }
        }
    }
}
