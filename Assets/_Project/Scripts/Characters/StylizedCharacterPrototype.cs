using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Stylized, procedural greybox character art. These meshes are temporary visual proxies,
    /// not replacements for the approved production character models.
    /// </summary>
    public sealed class StylizedCharacterPrototype : MonoBehaviour
    {
        private Transform[] animatedParts;
        private Vector3[] baseRotations;
        private bool isCat;
        private Transform tail;
        private Transform torso;
        private Transform head;
        private Transform leftEye;
        private Transform rightEye;
        private Transform leftPupil;
        private Transform rightPupil;
        private Vector3 torsoBaseScale;
        private Vector3 headBasePosition;
        private Vector3 leftEyeBaseScale;
        private Vector3 rightEyeBaseScale;
        private Vector3 leftPupilBaseScale;
        private Vector3 rightPupilBaseScale;
        private float idlePhase;
        private Vector3 previousPosition;

        public static void BuildMia(Transform root)
        {
            GameObject productionPrefab = Resources.Load<GameObject>("MiaMax/Characters/Mia");
            if (productionPrefab != null)
            {
                GameObject instance = Instantiate(productionPrefab, root, false);
                instance.name = "Mia_Production_Model";
                return;
            }

            var rig = root.gameObject.AddComponent<StylizedCharacterPrototype>();
            rig.isCat = false;
            rig.BuildGirl();
        }

        public static void BuildMax(Transform root)
        {
            GameObject productionPrefab = Resources.Load<GameObject>("MiaMax/Characters/Max");
            if (productionPrefab != null)
            {
                GameObject instance = Instantiate(productionPrefab, root, false);
                instance.name = "Max_Production_Model";
                return;
            }

            var rig = root.gameObject.AddComponent<StylizedCharacterPrototype>();
            rig.isCat = true;
            rig.BuildCat();
        }

        private void BuildGirl()
        {
            Material skin = Mat(new Color(0.96f, 0.72f, 0.57f));
            Material hair = Mat(new Color(0.88f, 0.66f, 0.28f));
            Material shirt = Mat(new Color(0.94f, 0.93f, 0.86f));
            Material denim = Mat(new Color(0.12f, 0.29f, 0.55f));
            Material shoes = Mat(new Color(0.94f, 0.93f, 0.87f));
            Material eyes = Mat(new Color(0.10f, 0.36f, 0.22f));
            Material dark = Mat(new Color(0.18f, 0.12f, 0.08f));

            torso = Part("Mia_Torso_Proxy", PrimitiveType.Capsule, new Vector3(0f, 0.98f, 0f), new Vector3(0.48f, 0.58f, 0.32f), shirt);
            Part("Mia_Neck_Proxy", PrimitiveType.Cylinder, new Vector3(0f, 1.48f, 0f), new Vector3(0.12f, 0.12f, 0.12f), skin);
            head = Part("Mia_Head_Proxy", PrimitiveType.Sphere, new Vector3(0f, 1.68f, 0.02f), new Vector3(0.48f, 0.53f, 0.43f), skin);
            Part("Mia_Hair_Cap_Proxy", PrimitiveType.Sphere, new Vector3(0f, 1.91f, -0.04f), new Vector3(0.51f, 0.34f, 0.46f), hair);
            Part("Mia_Hair_Front_Proxy", PrimitiveType.Sphere, new Vector3(0f, 1.88f, 0.27f), new Vector3(0.43f, 0.20f, 0.18f), hair);
            leftEye = Part("Mia_Eye_Left_Proxy", PrimitiveType.Sphere, new Vector3(-0.16f, 1.72f, 0.405f), new Vector3(0.09f, 0.12f, 0.045f), eyes);
            rightEye = Part("Mia_Eye_Right_Proxy", PrimitiveType.Sphere, new Vector3(0.16f, 1.72f, 0.405f), new Vector3(0.09f, 0.12f, 0.045f), eyes);
            Part("Mia_Smile_Proxy", PrimitiveType.Sphere, new Vector3(0f, 1.53f, 0.414f), new Vector3(0.09f, 0.035f, 0.025f), dark);

            for (int side = -1; side <= 1; side += 2)
            {
                float x = side * 0.29f;
                Transform braid = Part("Mia_Braid_Proxy", PrimitiveType.Capsule,
                    new Vector3(x, 1.48f, -0.02f), new Vector3(0.12f, 0.34f, 0.12f), hair);
                braid.localRotation = Quaternion.Euler(0f, 0f, side * -8f);
                Part("Mia_Braid_Tip_Proxy", PrimitiveType.Sphere,
                    new Vector3(x + side * -0.025f, 1.20f, -0.01f), new Vector3(0.14f, 0.14f, 0.14f), hair);
                Transform arm = Part("Mia_Arm_Proxy", PrimitiveType.Capsule,
                    new Vector3(side * 0.34f, 0.91f, 0f), new Vector3(0.13f, 0.34f, 0.13f), shirt);
                arm.localRotation = Quaternion.Euler(0f, 0f, side * -16f);
                Transform leg = Part("Mia_Jeans_Leg_Proxy", PrimitiveType.Capsule,
                    new Vector3(side * 0.14f, 0.39f, 0f), new Vector3(0.15f, 0.36f, 0.16f), denim);
                Transform shoe = Part("Mia_Sneaker_Proxy", PrimitiveType.Capsule,
                    new Vector3(side * 0.14f, 0.10f, 0.08f), new Vector3(0.17f, 0.10f, 0.23f), shoes);
                shoe.localRotation = Quaternion.Euler(90f, 0f, 0f);
                AddAnimated(arm);
                AddAnimated(leg);
            }
            CacheAnimationParts();
            CachePresentationPose();
        }

        private void BuildCat()
        {
            Material fur = Mat(new Color(0.91f, 0.39f, 0.10f));
            Material lightFur = Mat(new Color(0.97f, 0.82f, 0.64f));
            Material innerEar = Mat(new Color(0.86f, 0.43f, 0.39f));
            Material green = Mat(new Color(0.19f, 0.53f, 0.25f));
            Material black = Mat(new Color(0.12f, 0.10f, 0.08f));
            torso = Part("Max_Body_Proxy", PrimitiveType.Sphere, new Vector3(0f, 0.55f, 0f), new Vector3(0.92f, 0.59f, 0.58f), fur);
            Part("Max_Belly_Proxy", PrimitiveType.Sphere, new Vector3(0f, 0.40f, 0.27f), new Vector3(0.58f, 0.37f, 0.34f), lightFur);
            head = Part("Max_Head_Proxy", PrimitiveType.Sphere, new Vector3(0f, 1.02f, 0.25f), new Vector3(0.64f, 0.59f, 0.55f), fur);
            Part("Max_Muzzle_Proxy", PrimitiveType.Sphere, new Vector3(0f, 0.83f, 0.72f), new Vector3(0.38f, 0.23f, 0.20f), lightFur);
            Part("Max_Nose_Proxy", PrimitiveType.Sphere, new Vector3(0f, 0.91f, 0.91f), new Vector3(0.09f, 0.065f, 0.055f), innerEar);
            leftEye = Part("Max_Eye_Left_Proxy", PrimitiveType.Sphere, new Vector3(-0.21f, 1.10f, 0.70f), new Vector3(0.12f, 0.15f, 0.07f), green);
            rightEye = Part("Max_Eye_Right_Proxy", PrimitiveType.Sphere, new Vector3(0.21f, 1.10f, 0.70f), new Vector3(0.12f, 0.15f, 0.07f), green);
            leftPupil = Part("Max_Pupil_Left_Proxy", PrimitiveType.Sphere, new Vector3(-0.21f, 1.10f, 0.765f), new Vector3(0.045f, 0.09f, 0.025f), black);
            rightPupil = Part("Max_Pupil_Right_Proxy", PrimitiveType.Sphere, new Vector3(0.21f, 1.10f, 0.765f), new Vector3(0.045f, 0.09f, 0.025f), black);

            for (int side = -1; side <= 1; side += 2)
            {
                Transform ear = Part("Max_Ear_Proxy", PrimitiveType.Cube,
                    new Vector3(side * 0.39f, 1.47f, 0.19f), new Vector3(0.22f, 0.37f, 0.14f), fur);
                ear.localRotation = Quaternion.Euler(0f, 0f, side * -18f);
                Transform inner = Part("Max_Ear_Inner_Proxy", PrimitiveType.Cube,
                    new Vector3(side * 0.39f, 1.48f, 0.27f), new Vector3(0.11f, 0.22f, 0.035f), innerEar);
                inner.localRotation = ear.localRotation;
                Transform paw = Part("Max_Front_Paw_Proxy", PrimitiveType.Sphere,
                    new Vector3(side * 0.31f, 0.20f, 0.31f), new Vector3(0.24f, 0.22f, 0.28f), fur);
                Transform backPaw = Part("Max_Back_Paw_Proxy", PrimitiveType.Sphere,
                    new Vector3(side * 0.34f, 0.19f, -0.28f), new Vector3(0.25f, 0.22f, 0.28f), fur);
                AddAnimated(paw);
                AddAnimated(backPaw);
            }

            tail = Part("Max_Tail_Proxy", PrimitiveType.Capsule,
                new Vector3(0f, 0.79f, -0.52f), new Vector3(0.16f, 0.52f, 0.16f), fur);
            tail.localRotation = Quaternion.Euler(-48f, 0f, 0f);
            for (int i = 0; i < 3; i++)
                Part("Max_Tail_Stripe_Proxy", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.55f + i * 0.16f, -0.73f - i * 0.12f),
                    new Vector3(0.17f, 0.055f, 0.17f), black);
            CacheAnimationParts();
            CachePresentationPose();
        }

        private void CachePresentationPose()
        {
            idlePhase = Mathf.Abs(transform.position.x * 0.37f + transform.position.z * 0.19f);
            if (torso != null) torsoBaseScale = torso.localScale;
            if (head != null) headBasePosition = head.localPosition;
            if (leftEye != null) leftEyeBaseScale = leftEye.localScale;
            if (rightEye != null) rightEyeBaseScale = rightEye.localScale;
            if (leftPupil != null) leftPupilBaseScale = leftPupil.localScale;
            if (rightPupil != null) rightPupilBaseScale = rightPupil.localScale;
        }

        private Transform Part(string partName, PrimitiveType type, Vector3 localPosition, Vector3 localScale, Material material)
        {
            GameObject part = GameObject.CreatePrimitive(type);
            part.name = partName;
            part.transform.SetParent(transform, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;
            Renderer renderer = part.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            Collider collider = part.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
            return part.transform;
        }

        private void AddAnimated(Transform part)
        {
            if (animatedParts == null) animatedParts = new Transform[0];
            var next = new Transform[animatedParts.Length + 1];
            animatedParts.CopyTo(next, 0);
            next[next.Length - 1] = part;
            animatedParts = next;
        }

        private void CacheAnimationParts()
        {
            if (animatedParts == null) return;
            baseRotations = new Vector3[animatedParts.Length];
            for (int i = 0; i < animatedParts.Length; i++)
                baseRotations[i] = animatedParts[i].localEulerAngles;
        }

        private void Update()
        {
            float clock = Time.time + idlePhase;
            float movement = Mathf.Clamp01(GetComponent<CharacterController>() != null
                ? GetComponent<CharacterController>().velocity.magnitude / 4f
                : 0f);
            float idleBlend = 1f - movement;

            // Gentle breathing and head motion keep the proxy characters alive when standing still.
            if (torso != null)
            {
                float breath = Mathf.Sin(clock * (isCat ? 2.0f : 1.65f)) * 0.012f * idleBlend;
                torso.localScale = new Vector3(torsoBaseScale.x, torsoBaseScale.y * (1f + breath),
                    torsoBaseScale.z * (1f - breath * 0.35f));
            }
            if (head != null)
            {
                float nod = Mathf.Sin(clock * 1.35f) * (isCat ? 0.018f : 0.012f) * idleBlend;
                head.localPosition = headBasePosition + Vector3.up * nod;
                head.localRotation = Quaternion.Euler(Mathf.Sin(clock * 0.9f) * 1.8f * idleBlend, 0f, 0f);
            }

            // Natural periodic blinks. Max's pupils follow the eye squash so they stay aligned.
            float blink = BlinkAmount(clock + (isCat ? 1.1f : 0f));
            SetEyeBlink(leftEye, leftEyeBaseScale, blink);
            SetEyeBlink(rightEye, rightEyeBaseScale, blink);
            SetEyeBlink(leftPupil, leftPupilBaseScale, blink);
            SetEyeBlink(rightPupil, rightPupilBaseScale, blink);

            if (isCat && tail != null)
                tail.localRotation = Quaternion.Euler(-48f, 0f, Mathf.Sin(clock * 2.2f) * (12f + 8f * idleBlend));

            if (animatedParts == null) return;
            float gaitFrequency = Mathf.Lerp(9f, 12f, movement);
            float gaitAmplitude = Mathf.Lerp(4f, 28f, movement);
            for (int i = 0; i < animatedParts.Length; i++)
            {
                if (animatedParts[i] == null) continue;
                float swing = Mathf.Sin(clock * gaitFrequency + i * Mathf.PI) * gaitAmplitude * movement;
                animatedParts[i].localRotation = Quaternion.Euler(baseRotations[i] + new Vector3(swing, 0f, 0f));
            }
        }

        private static float BlinkAmount(float time)
        {
            float cycle = Mathf.Repeat(time, 4.1f);
            if (cycle > 0.12f) return 0f;
            float phase = cycle / 0.12f;
            return Mathf.Sin(phase * Mathf.PI);
        }

        private static void SetEyeBlink(Transform eye, Vector3 baseScale, float blink)
        {
            if (eye == null) return;
            eye.localScale = new Vector3(baseScale.x, baseScale.y * (1f - blink * 0.92f), baseScale.z);
        }

        private static Material Mat(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            return new Material(shader) { color = color };
        }
    }
}
