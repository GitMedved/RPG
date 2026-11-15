#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Networking;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Curated free CC0 rig import workflow. We do not silently declare random
    /// third-party figures to be Mia/Max; promotion to the production slots is explicit.
    ///
    /// Sources:
    /// Gobkit Schoolkid, CC0: https://gobkit.com/api/free
    /// KayKit Rogue, CC0: https://github.com/KayKit-Game-Assets/KayKit-Character-Pack-Adventures-1.0
    /// Vuena Toy Cat FREE sample, CC0: https://vuena.itch.io/blocky-pets-free
    /// </summary>
    public static class MiaMaxRigPackInstaller
    {
        const string CandidateDir = "Assets/Resources/MiaMax/External/Candidates";
        const string CastDir = "Assets/Resources/MiaMax/External/Cast";
        const string AnimDir = "Assets/_Project/Animations/Imported";
        static bool busy;

        [MenuItem("Mia & Max/Art Pipeline/1. Download CC0 Rigged Human Candidates")]
        private static async void InstallHumans()
        {
            if (busy || EditorApplication.isPlaying) return;
            if (!EditorUtility.DisplayDialog("CC0 Character Candidates",
                "Download two free animated models (a child-scale Schoolkid and KayKit Rogue) into the project as reference candidates? They are NOT approved Mia likenesses.",
                "Download Candidates", "Cancel")) return;

            busy = true;
            try
            {
                Directory.CreateDirectory(CandidateDir);
                string[] urls = {
                    "https://gobkit.com/freebies/human001/Schoolkid.glb",
                    "https://raw.githubusercontent.com/KayKit-Game-Assets/KayKit-Character-Pack-Adventures-1.0/main/addons/kaykit_character_pack_adventures/Characters/gltf/Rogue.glb"
                };
                string[] names = { "MiaSchoolkidCandidate.glb", "MiaRogueCandidate.glb" };
                int installed = 0;
                for (int i=0; i<urls.Length; i++)
                {
                    if (await DownloadGlb(urls[i], CandidateDir + "/" + names[i]))
                        installed++;
                }
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                Debug.Log("[Mia & Max Rig] Human candidate downloads: " + installed +
                    "/" + urls.Length + ". Inspect in Unity before approval.");
            }
            finally { busy = false; }
        }

        private static async Task<bool> DownloadGlb(string url,string path)
        {
            if (File.Exists(path) && IsValidGlb(File.ReadAllBytes(path)))
                return true;

            try
            {
                using (UnityWebRequest req = UnityWebRequest.Get(url))
                {
                    req.timeout = 90;
                    UnityWebRequestAsyncOperation op = req.SendWebRequest();
                    while (!op.isDone) await Task.Yield();
                    if (req.result != UnityWebRequest.Result.Success)
                        throw new IOException(req.error);
                    byte[] data=req.downloadHandler.data;
                    if (!IsValidGlb(data)) throw new InvalidDataException("Invalid .glb header");
                    if (data.Length > 30 * 1024 * 1024)
                        throw new InvalidDataException("Model exceeds 30 MB limit");
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.WriteAllBytes(path,data);
                    Debug.Log("[Mia & Max Rig] Downloaded verified GLB: "+path+
                        " ("+(data.Length/1024)+" KB)");
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Mia & Max Rig] Download failed for "+url+": "+e.Message);
                return false;
            }
        }

        private static bool IsValidGlb(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 1024) return false;
            if (bytes[0] != (byte)'g' || bytes[1] != (byte)'l'
                || bytes[2] != (byte)'T' || bytes[3] != (byte)'F')
                return false;
            uint size = BitConverter.ToUInt32(bytes,8);
            return size == (uint)bytes.Length && BitConverter.ToUInt32(bytes,4)==2;
        }

        [MenuItem("Mia & Max/Art Pipeline/2. Import Free Animated Cat Archive")]
        private static void ImportCat()
        {
            if (busy || EditorApplication.isPlaying) return;
            string path=EditorUtility.OpenFilePanel("Select Vuena free CC0 cat ZIP",
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "zip");
            if (string.IsNullOrEmpty(path)) return;
            try
            {
                Directory.CreateDirectory(CandidateDir);
                // Only extract a .glb with cat in the name: avoid zip-slip and
                // unrelated files. GLBs embed material data and model textures.
                using (ZipArchive archive=ZipFile.OpenRead(path))
                {
                    ZipArchiveEntry entry=archive.Entries
                        .Where(e=>e.Name.EndsWith(".glb",StringComparison.OrdinalIgnoreCase)
                            && e.Name.IndexOf("cat",StringComparison.OrdinalIgnoreCase)>=0)
                        .OrderBy(e=>e.Name.IndexOf("LOD1",StringComparison.OrdinalIgnoreCase)>=0?1:0)
                        .ThenBy(e=>e.Length)
                        .FirstOrDefault();
                    if (entry==null)throw new FileNotFoundException(
                        "Could not locate a Cat GLB in the supplied archive.");
                    if (entry.Length > 30*1024*1024)
                        throw new InvalidDataException("Cat GLB exceeds 30MB.");
                    string output=CandidateDir+"/MaxCatCandidate.glb";
                    using (Stream src=entry.Open())
                    using (FileStream dst=File.Create(output))
                        src.CopyTo(dst);
                    if (!IsValidGlb(File.ReadAllBytes(output)))
                    {
                        File.Delete(output);
                        throw new InvalidDataException("The cat file is not GLB v2.");
                    }
                    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                    Debug.Log("[Mia & Max Rig] Imported animated CC0 Cat: "+output);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Mia & Max Rig] Cat archive invalid: "+e.Message);
            }
        }

        [MenuItem("Mia & Max/Art Pipeline/3. Build Animated Max Prefab (Review Candidate)")]
        private static void BuildMax()
        {
            BuildPrefab("Max",CandidateDir+"/MaxCatCandidate.glb");
        }

        [MenuItem("Mia & Max/Art Pipeline/4. Build Animated Mia Prefab (Review Candidate)")]
        private static void BuildMia()
        {
            int decision = EditorUtility.DisplayDialogComplex("Mia Character Selection",
                "No free candidate is an exact likeness of the original Mia reference. Schoolkid offers child-like proportions; KayKit Rogue is stylized but has fantasy clothing. Choose a temporary model for testing the full rig.",
                "Schoolkid", "Cancel", "KayKit Rogue");
            if (decision==1) return;
            BuildPrefab("Mia",CandidateDir+"/"+
                (decision==0?"MiaSchoolkidCandidate.glb":"MiaRogueCandidate.glb"));
        }

        [MenuItem("Mia & Max/Art Pipeline/5. Verify Imported Rigs and Animation Clips")]
        private static void AuditRigs()
        {
            string[] names={"Mia","Max"};
            foreach(string name in names)
            {
                string path=CastDir+"/"+name+"Rig.prefab";
                GameObject prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab==null)
                {
                    Debug.LogWarning("[Mia & Max Rig] Not approved yet: "+name);
                    continue;
                }
                var renderers=prefab.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                var animators=prefab.GetComponentsInChildren<Animator>(true);
                var animations=prefab.GetComponentsInChildren<Animation>(true);
                int clips=0;
                foreach(var animator in animators)
                    if(animator.runtimeAnimatorController!=null)
                        clips+=animator.runtimeAnimatorController.animationClips.Length;
                foreach(var a in animations)clips+=a.GetClipCount();
                if(renderers.Length==0 || clips==0)
                    Debug.LogError("[Mia & Max Rig] Incomplete rig: "+name+
                        " Skinned="+renderers.Length+" Clips="+clips);
                else Debug.Log("[Mia & Max Rig] Ready: "+name+
                    " Skinned="+renderers.Length+" Clips="+clips);
            }
        }

        private static void BuildPrefab(string name,string importedPath)
        {
            if (EditorApplication.isPlaying) return;
            GameObject model=AssetDatabase.LoadAssetAtPath<GameObject>(importedPath);
            if (model==null)
            {
                Debug.LogError("[Mia & Max Rig] Import the model first: "+
                    importedPath + ". Ensure glTFast imports .glb files.");
                return;
            }

            UnityEngine.Object[] subassets=AssetDatabase.LoadAllAssetsAtPath(importedPath);
            AnimationClip[] clips=subassets.OfType<AnimationClip>()
                .Where(c=>!c.name.StartsWith("__preview__",StringComparison.OrdinalIgnoreCase))
                .ToArray();
            if(clips.Length==0)
            {
                Debug.LogError("[Mia & Max Rig] No baked clips found in "+
                    importedPath+". Do not approve as an animated character.");
                return;
            }

            Directory.CreateDirectory(CastDir);
            Directory.CreateDirectory(AnimDir);
            string ctrlPath=CastDir+"/"+name+"Rig.controller";
            if(AssetDatabase.LoadAssetAtPath<AnimatorController>(ctrlPath)!=null)
                AssetDatabase.DeleteAsset(ctrlPath);
            AnimatorController controller=AnimatorController.CreateAnimatorControllerAtPath(ctrlPath);
            AnimatorStateMachine sm=controller.layers[0].stateMachine;
            var idle=PickClip(clips,"idle");
            var react=PickClip(clips,"cheer","wave","special","happy","jump","call","idle_alt");
            var talk=PickClip(clips,"talk","call","special","idle_alt","idle");
            AnimationClip idleClip=CopyClip(name,"Idle",idle);
            AnimationClip reactClip=CopyClip(name,"React",react??idle);
            AnimationClip talkClip=CopyClip(name,"Talk",talk??idle);
            AnimatorState idleState=sm.AddState("Idle");
            idleState.motion=idleClip;
            sm.defaultState=idleState;
            AnimatorState reactState=sm.AddState("React");
            reactState.motion=reactClip;
            AnimatorState talkState=sm.AddState("Talk");
            talkState.motion=talkClip;

            GameObject root=new GameObject(name+" Rig Approved");
            try
            {
                GameObject child=PrefabUtility.InstantiatePrefab(model) as GameObject;
                if(child==null)child=UnityEngine.Object.Instantiate(model);
                child.name=name+" imported skeletal mesh";
                child.transform.SetParent(root.transform,false);
                Animator animator=child.GetComponentInChildren<Animator>(true);
                if(animator==null)animator=child.AddComponent<Animator>();
                animator.runtimeAnimatorController=controller;
                animator.applyRootMotion=false;
                root.AddComponent<MiaMax.Gameplay.MiaMaxRigAnimator>();

                string prefabPath=CastDir+"/"+name+"Rig.prefab";
                PrefabUtility.SaveAsPrefabAsset(root,prefabPath);
                AssetDatabase.SaveAssets();
                Debug.Log("[Mia & Max Rig] Created "+prefabPath+
                    ", "+clips.Length+" source clips; states Idle / React / Talk. Re-enter Play Mode to preview.");
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
            finally {UnityEngine.Object.DestroyImmediate(root);}
        }

        private static AnimationClip PickClip(AnimationClip[] clips,params string[] parts)
        {
            foreach(string part in parts)
            {
                AnimationClip selected=clips.FirstOrDefault(c=>c.name.Equals(part,StringComparison.OrdinalIgnoreCase));
                if(selected!=null)return selected;
                selected=clips.FirstOrDefault(c=>c.name.IndexOf(part,StringComparison.OrdinalIgnoreCase)>=0);
                if(selected!=null)return selected;
            }
            return clips.Length>0?clips[0]:null;
        }

        private static AnimationClip CopyClip(string actor,string state,AnimationClip original)
        {
            if(original==null)throw new InvalidDataException("No animation clips.");
            string path=AnimDir+"/"+actor+"_"+state+".anim";
            if(AssetDatabase.LoadAssetAtPath<AnimationClip>(path)!=null)
                AssetDatabase.DeleteAsset(path);
            AnimationClip clip=UnityEngine.Object.Instantiate(original);
            clip.name=actor+"_"+state;
            clip.legacy=false;
            clip.wrapMode=state=="Idle"?WrapMode.Loop:WrapMode.Once;
            AssetDatabase.CreateAsset(clip,path);
            return clip;
        }

        [MenuItem("Mia & Max/Art Pipeline/1. Download CC0 Rigged Human Candidates",true)]
        [MenuItem("Mia & Max/Art Pipeline/2. Import Free Animated Cat Archive",true)]
        private static bool CanImport()=> !EditorApplication.isPlaying && !busy;
    }
}
#endif
