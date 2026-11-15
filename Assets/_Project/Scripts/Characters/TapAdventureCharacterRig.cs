using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Two lightweight, animated, sculpted 3D proxy characters. Never moves the player.
    /// Mia and Max's reference portraits are authoritative: this code-only geometry is
    /// a visual development proxy, NOT a likeness-approved production character asset.
    /// </summary>
    public sealed class TapAdventureCharacterRig : MonoBehaviour
    {
        private bool cat;
        private Transform torso, head, leftArm, rightArm, tailRoot;
        private Transform leftEye, rightEye;
        private readonly List<Material> created = new List<Material>();
        private float moodUntil, lastBlink, blinkAt = 2f, phase;
        private bool happyReaction;
        private Vector3 bodyRest, headRest;
        private static readonly Mesh smooth = ProceduralMeshes.Blob(.022f, 4);
        private static readonly Mesh round = ProceduralMeshes.RoundedBox;

        public static TapAdventureCharacterRig CreateMia(Transform parent, Vector3 position)
        {
            TapAdventureCharacterRig rig = Spawn("Mia - illustrated moving 3D proxy",
                parent, position, false);
            rig.BuildMia();
            return rig;
        }

        public static TapAdventureCharacterRig CreateMax(Transform parent, Vector3 position)
        {
            TapAdventureCharacterRig rig = Spawn("Max - illustrated moving 3D cat proxy",
                parent, position, true);
            rig.BuildMax();
            return rig;
        }

        private static TapAdventureCharacterRig Spawn(string name, Transform parent, Vector3 pos, bool isCat)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            TapAdventureCharacterRig result = go.AddComponent<TapAdventureCharacterRig>();
            result.cat = isCat;
            result.phase = isCat ? 1.4f : 0f;
            result.blinkAt = isCat ? 3.2f : 2.5f;
            return result;
        }

        private Transform Root(string name, Transform parent, Vector3 pos)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            return go.transform;
        }

        private Material Material(string name, Color fallback, float smoothness = .22f)
        {
            Material source = Resources.Load<Material>("MiaMax/Materials/" + name);
            if (source != null) return source;
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if(shader == null) shader = Shader.Find("Standard");
            Material mat = new Material(shader) { name = "MiaMax Character - " + name, color = fallback };
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", fallback);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            created.Add(mat);
            return mat;
        }

        private Transform Part(string name, Transform parent, Vector3 position,
            Vector3 size, Material material, Mesh mesh = null, Quaternion? rotation = null)
        {
            GameObject part = new GameObject(name);
            Transform t = part.transform;
            t.SetParent(parent, false);
            t.localPosition = position;
            t.localScale = size;
            if(rotation.HasValue)t.localRotation = rotation.Value;
            part.AddComponent<MeshFilter>().sharedMesh = mesh ?? smooth;
            part.AddComponent<MeshRenderer>().sharedMaterial = material;
            return t;
        }

        private void Bone(string name, Transform parent, Vector3 from, Vector3 to,
            float radius, Material mat)
        {
            Vector3 direction = to-from;
            Part(name,parent,(from+to)*.5f,
                new Vector3(radius,direction.magnitude,radius),mat,
                ProceduralMeshes.TaperedColumn,
                Quaternion.FromToRotation(Vector3.up,direction.normalized));
        }

        private void BuildMia()
        {
            Material skin=Material("Skin_Warm",new Color(.91f,.66f,.50f));
            Material hair=Material("Hair_Blonde",new Color(.73f,.51f,.24f));
            Material hairLight=Material("Wood_Warm",new Color(.81f,.57f,.26f));
            Material denim=Material("Denim_Blue",new Color(.29f,.49f,.73f));
            Material white=Material("Cotton_White",new Color(.97f,.94f,.88f));
            Material shoe=Material("Leather_Brown",new Color(.54f,.34f,.23f));
            Material eyes=Material("Eye_Green",new Color(.24f,.47f,.38f));
            Material dark=Material("Bark_Tree",new Color(.19f,.21f,.20f));
            Material cheek=Material("Flower_Petal",new Color(.95f,.47f,.45f));
            Material gold=Material("Ceramic_Glaze",new Color(.91f,.78f,.51f));

            torso=Root("Mia body sway joint",transform,new Vector3(0,1.20f,0));
            Part("Cozy white undershirt",torso,new Vector3(0,.15f,0),
                new Vector3(.69f,.79f,.47f),white);
            Part("Blue denim pinafore",torso,new Vector3(0,-.10f,-.01f),
                new Vector3(.74f,.76f,.53f),denim);
            Part("Cloth apron pocket",torso,new Vector3(0,-.26f,-.28f),
                new Vector3(.28f,.20f,.06f),denim,round);
            for(int side=-1;side<=1;side+=2)
            {
                Part("Overall strap",torso,new Vector3(side*.24f,.31f,-.23f),
                    new Vector3(.13f,.52f,.09f),denim,round);
                Part("Gold strap button",torso,new Vector3(side*.24f,.14f,-.29f),
                    new Vector3(.10f,.10f,.05f),gold);
            }
            Part("Soft neck",torso,new Vector3(0,.60f,0),
                new Vector3(.20f,.33f,.22f),skin);
            head=Root("Mia expressive head",torso,new Vector3(0,.95f,-.01f));
            Part("Rounded cheeks and face",head,Vector3.zero,
                new Vector3(.83f,.91f,.72f),skin);
            Part("Blonde hair cap",head,new Vector3(0,.31f,.03f),
                new Vector3(.91f,.48f,.79f),hair);
            Part("Left framing lock",head,new Vector3(-.39f,.05f,-.24f),
                new Vector3(.24f,.59f,.28f),hair);
            Part("Right framing lock",head,new Vector3(.39f,.05f,-.24f),
                new Vector3(.24f,.59f,.28f),hair);
            Part("Back ponytail",head,new Vector3(.35f,.11f,.40f),
                new Vector3(.35f,.66f,.39f),hair);
            Part("Hair tie",head,new Vector3(.32f,.26f,.49f),
                new Vector3(.20f,.15f,.20f),denim);
            for(int side=-1;side<=1;side+=2)
            {
                float x=side*.19f;
                Part("Eyeball",head,new Vector3(x,.07f,-.35f),
                    new Vector3(.19f,.25f,.09f),white);
                Transform eye=Part("Green iris",head,new Vector3(x,.035f,-.412f),
                    new Vector3(.095f,.145f,.055f),eyes);
                if(side==-1)leftEye=eye;else rightEye=eye;
                Part("Eye highlight",head,new Vector3(x-.017f,.08f,-.447f),
                    new Vector3(.033f,.043f,.026f),white);
                Part("Rosy cheek",head,new Vector3(side*.29f,-.18f,-.27f),
                    new Vector3(.18f,.10f,.055f),cheek);
            }
            Part("Small button nose",head,new Vector3(0,-.12f,-.37f),
                new Vector3(.12f,.11f,.12f),skin);
            Part("Smile",head,new Vector3(0,-.30f,-.33f),
                new Vector3(.20f,.035f,.038f),dark);
            leftArm=Root("Mia left arm pivot",torso,new Vector3(-.39f,.40f,0));
            rightArm=Root("Mia right arm pivot",torso,new Vector3(.39f,.40f,0));
            BuildMiaArm(leftArm,skin,white,-1);
            BuildMiaArm(rightArm,skin,white,1);
            for(int side=-1;side<=1;side+=2)
            {
                Part("Denim trouser leg",transform,new Vector3(side*.18f,.58f,0),
                    new Vector3(.29f,.95f,.32f),denim);
                Part("Stitched ankle",transform,new Vector3(side*.18f,.20f,-.03f),
                    new Vector3(.28f,.15f,.31f),white);
                Part("Soft shoe",transform,new Vector3(side*.18f,.10f,-.18f),
                    new Vector3(.39f,.24f,.64f),shoe);
            }
            bodyRest=torso.localPosition;
            headRest=head.localPosition;
        }

        private void BuildMiaArm(Transform pivot, Material skin, Material cotton, int side)
        {
            Part("Shirt sleeve",pivot,new Vector3(side*.07f,-.14f,0),
                new Vector3(.29f,.32f,.31f),cotton);
            Part("Forearm",pivot,new Vector3(side*.18f,-.39f,0),
                new Vector3(.21f,.47f,.23f),skin);
            Part("Hand",pivot,new Vector3(side*.21f,-.63f,0),
                new Vector3(.21f,.24f,.22f),skin);
        }

        private void BuildMax()
        {
            Material orange=Material("Fur_Ginger",new Color(.90f,.49f,.23f));
            Material cream=Material("Fur_Cream",new Color(.96f,.84f,.64f));
            Material eye=Material("Eye_Green",new Color(.30f,.57f,.39f));
            Material dark=Material("Bark_Tree",new Color(.21f,.23f,.19f));
            Material pink=Material("Flower_Petal",new Color(.98f,.60f,.59f));
            Material gingerShade=Material("Wood_Warm",new Color(.63f,.33f,.16f));

            torso=Root("Max animated body",transform,new Vector3(0,.58f,0));
            Part("Plump ginger body",torso,new Vector3(0,0,.03f),
                new Vector3(.95f,.89f,1.13f),orange);
            Part("Cream chest bib",torso,new Vector3(0,-.10f,-.47f),
                new Vector3(.63f,.66f,.34f),cream);
            for(int side=-1;side<=1;side+=2)
                for(int front=-1;front<=1;front+=2)
                {
                    Part("Ginger cat paw",torso,
                        new Vector3(side*.34f,-.40f,front*.32f),
                        new Vector3(.27f,.39f,.31f),orange);
                    Part("Cream toe",torso,new Vector3(side*.34f,-.56f,front*.36f-.075f),
                        new Vector3(.26f,.14f,.32f),cream);
                }
            head=Root("Max playful head",torso,new Vector3(0,.51f,-.33f));
            Part("Cat head",head,Vector3.zero,
                new Vector3(.87f,.81f,.80f),orange);
            Part("White muzzle left",head,new Vector3(-.16f,-.21f,-.36f),
                new Vector3(.37f,.30f,.26f),cream);
            Part("White muzzle right",head,new Vector3(.16f,-.21f,-.36f),
                new Vector3(.37f,.30f,.26f),cream);
            Part("Pink nose",head,new Vector3(0,-.14f,-.52f),
                new Vector3(.14f,.11f,.11f),pink);
            for(int side=-1;side<=1;side+=2)
            {
                Transform ear=Root("Triangular ear",head,new Vector3(side*.32f,.35f,0));
                ear.localRotation=Quaternion.Euler(0,0,side*-17);
                Part("Ginger ear shell",ear,new Vector3(0,.13f,0),
                    new Vector3(.30f,.46f,.18f),orange,ProceduralMeshes.TaperedColumn);
                Part("Soft pink inner ear",ear,new Vector3(0,.17f,-.10f),
                    new Vector3(.16f,.31f,.07f),pink,ProceduralMeshes.TaperedColumn);
                Part("White eye",head,new Vector3(side*.20f,.075f,-.34f),
                    new Vector3(.25f,.30f,.13f),cream);
                Transform iris=Part("Cat bright iris",head,new Vector3(side*.20f,.045f,-.445f),
                    new Vector3(.14f,.21f,.08f),eye);
                if(side==-1)leftEye=iris;else rightEye=iris;
                Part("Vertical pupil",head,new Vector3(side*.20f,.045f,-.51f),
                    new Vector3(.052f,.17f,.045f),dark);
                for(int w=0;w<2;w++)
                {
                    Bone("Max tiny whisker",head,
                        new Vector3(side*.25f,-.23f,-.46f),
                        new Vector3(side*(.62f+w*.02f),-.18f-w*.11f,-.52f),
                        .016f,cream);
                }
                Part("Ginger brow stripe",head,new Vector3(side*.17f,.27f,-.32f),
                    new Vector3(.14f,.09f,.09f),gingerShade);
            }
            Part("Tiny smile",head,new Vector3(0,-.33f,-.48f),
                new Vector3(.16f,.035f,.04f),dark);
            tailRoot=Root("Swaying ginger tail",torso,new Vector3(0,.10f,.48f));
            for(int i=0;i<5;i++)
            {
                float k=i/4f;
                Bone("Curled tail segment",tailRoot,
                    new Vector3(.05f+k*.40f,k*.31f,k*.45f),
                    new Vector3(.11f+(k+.2f)*.4f,(k+.2f)*.35f,(k+.2f)*.50f),
                    .16f-k*.07f,orange);
            }
            Part("Tail tip",tailRoot,new Vector3(.60f,.42f,.59f),
                new Vector3(.19f,.19f,.19f),cream);
            bodyRest=torso.localPosition;
            headRest=head.localPosition;
        }

        private void Update()
        {
            if(torso==null||head==null)return;
            float time=Time.time+phase;
            bool reacting=Time.time < moodUntil;
            float bounce=reacting ? .09f*Mathf.Abs(Mathf.Sin(time*11f)) :
                .026f*Mathf.Sin(time*2.3f);
            torso.localPosition=bodyRest+Vector3.up*bounce;
            float tilt=Mathf.Sin(time*1.12f)*3f;
            head.localPosition=headRest+Vector3.up*(.018f*Mathf.Sin(time*2.0f));
            head.localRotation=Quaternion.Euler(0f,Mathf.Sin(time*.66f)*7f,
                reacting?(happyReaction?8f:-9f):tilt);
            if(!cat)
            {
                if(leftArm!=null)
                    leftArm.localRotation=Quaternion.Euler(0,0,
                        reacting?(happyReaction?-75f:20f):-8f+Mathf.Sin(time*1.4f)*7f);
                if(rightArm!=null)
                    rightArm.localRotation=Quaternion.Euler(0,0,
                        reacting?(happyReaction?75f:-20f):8f-Mathf.Sin(time*1.4f)*7f);
            }
            else if(tailRoot!=null)
                tailRoot.localRotation=Quaternion.Euler(0,
                    Mathf.Sin(time*2.8f)*(reacting?28f:15f),
                    Mathf.Sin(time*2.1f)*12f);
            // Separate eye transforms provide a natural periodic blink on both faces.
            float blinkDuration=.13f;
            if(time-lastBlink>blinkAt)
            {
                lastBlink=time;
                blinkAt=3f+(Mathf.Sin(time*12f)+1f)*1.25f;
            }
            float eyeScale = time - lastBlink < blinkDuration ? 0.12f : 1f;
            if(leftEye!=null)
            {
                Vector3 s=leftEye.localScale;
                leftEye.localScale=new Vector3(s.x,cat ? .21f * eyeScale : .145f * eyeScale,s.z);
            }
            if(rightEye!=null)
            {
                Vector3 s=rightEye.localScale;
                rightEye.localScale=new Vector3(s.x,cat ? .21f * eyeScale : .145f * eyeScale,s.z);
            }
        }

        public void React(bool happy)
        {
            happyReaction=happy;
            moodUntil=Time.time+1.45f;
        }

        private void OnDestroy()
        {
            foreach(Material m in created)if(m!=null)Destroy(m);
        }
    }
}
