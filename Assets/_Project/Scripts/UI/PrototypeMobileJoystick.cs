using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Lightweight touch joystick for the greybox. It has no prefab or external UI dependency.
    /// Replace with the production UI system once the visual design is approved.
    /// </summary>
    public sealed class PrototypeMobileJoystick : MonoBehaviour
    {
        public static PrototypeMobileJoystick Instance { get; private set; }
        public Vector2 Value { get; private set; }

        [SerializeField] private float radius = 82f;
        [SerializeField] private float deadZone = 12f;
        private int activeFingerId = -1;
        private Vector2 startPosition;
        private Vector2 currentPosition;
        private Texture2D circleTexture;

        private Rect JoystickRect => new Rect(24f, 24f, radius * 2f, radius * 2f);

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            circleTexture = CreateCircleTexture(64);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (circleTexture != null) Destroy(circleTexture);
        }

        private void Update()
        {
            if (Input.touchCount == 0)
            {
                activeFingerId = -1;
                Value = Vector2.zero;
                return;
            }

            bool foundFinger = false;
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (activeFingerId < 0 && touch.phase == TouchPhase.Began &&
                    JoystickRect.Contains(touch.position))
                {
                    activeFingerId = touch.fingerId;
                    startPosition = touch.position;
                    currentPosition = touch.position;
                }

                if (touch.fingerId != activeFingerId) continue;
                foundFinger = true;
                currentPosition = touch.position;

                Vector2 delta = currentPosition - startPosition;
                Value = delta.magnitude < deadZone ? Vector2.zero : Vector2.ClampMagnitude(delta / radius, 1f);

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    activeFingerId = -1;
                    Value = Vector2.zero;
                }
                break;
            }

            if (!foundFinger && activeFingerId >= 0)
            {
                activeFingerId = -1;
                Value = Vector2.zero;
            }
        }

        private void OnGUI()
        {
            if (circleTexture == null) return;
            Vector2 center = new Vector2(24f + radius, Screen.height - 24f - radius);
            GUI.color = new Color(1f, 1f, 1f, 0.22f);
            GUI.DrawTexture(new Rect(center.x - radius, center.y - radius, radius * 2f, radius * 2f), circleTexture);
            GUI.color = new Color(1f, 1f, 1f, 0.55f);
            Vector2 knob = center + Value * radius * 0.72f;
            float knobRadius = radius * 0.38f;
            GUI.DrawTexture(new Rect(knob.x - knobRadius, knob.y - knobRadius, knobRadius * 2f, knobRadius * 2f), circleTexture);
            GUI.color = Color.white;
        }

        private static Texture2D CreateCircleTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.hideFlags = HideFlags.HideAndDontSave;
            Color[] pixels = new Color[size * size];
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float r = size * 0.48f;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Clamp01(r + 1f - distance);
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
    }
}
