using UnityEngine;
using UnityEngine.UI;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Touch joystick with a Canvas-rendered visual. Touch ownership remains independent
    /// of the EventSystem so the movement control can coexist with the HUD buttons.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypeMobileJoystick : MonoBehaviour
    {
        public static PrototypeMobileJoystick Instance { get; private set; }
        public Vector2 Value { get; private set; }

        [SerializeField] private float radius = 82f;
        [SerializeField] private float deadZone = 12f;
        [SerializeField, Range(0, 100)] private int canvasSortingOrder = 20;

        private int activeFingerId = -1;
        private Vector2 startPosition;
        private Texture2D circleTexture;
        private Sprite circleSprite;
        private RectTransform visualRoot;
        private RectTransform knobRect;
        private Canvas visualCanvas;
        private int lastScreenWidth;
        private int lastScreenHeight;
        private Rect lastSafeArea;

        private float EffectiveRadius =>
            Mathf.Clamp(radius * Mathf.Clamp(Screen.width / 900f, 0.72f, 1.12f), 60f, 104f);

        private const float SafeAreaMargin = 18f;

        private Rect JoystickRect
        {
            get
            {
                Rect safe = Screen.safeArea;
                float r = EffectiveRadius;
                float marginX = Mathf.Min(SafeAreaMargin, Mathf.Max(0f, (safe.width - r * 2f) * 0.5f));
                float marginY = Mathf.Min(SafeAreaMargin, Mathf.Max(0f, (safe.height - r * 2f) * 0.5f));
                return new Rect(safe.xMin + marginX, safe.yMin + marginY, r * 2f, r * 2f);
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            circleTexture = CreateCircleTexture(96);
            circleSprite = Sprite.Create(circleTexture, new Rect(0f, 0f, 96f, 96f),
                new Vector2(0.5f, 0.5f), 96f);
            circleSprite.name = "MiaMax Joystick Circle";
            BuildCanvasVisual();
            UpdateCanvasLayout(true);
        }

        private void OnEnable()
        {
            UpdateCanvasLayout(true);
        }

        private void OnDisable()
        {
            ResetInput();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus) ResetInput();
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused) ResetInput();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (circleSprite != null) Destroy(circleSprite);
            if (circleTexture != null) Destroy(circleTexture);
        }

        private void ResetInput()
        {
            activeFingerId = -1;
            Value = Vector2.zero;
            UpdateKnobVisual();
        }

        private void Update()
        {
            UpdateCanvasLayout(false);

            if (Input.touchCount == 0)
            {
                if (activeFingerId >= 0) ResetInput();
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
                }

                if (touch.fingerId != activeFingerId) continue;
                foundFinger = true;

                Vector2 delta = touch.position - startPosition;
                Value = delta.magnitude < deadZone
                    ? Vector2.zero
                    : Vector2.ClampMagnitude(delta / EffectiveRadius, 1f);

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    activeFingerId = -1;
                    Value = Vector2.zero;
                }
                break;
            }

            if (!foundFinger && activeFingerId >= 0)
                ResetInput();

            UpdateKnobVisual();
        }

        private void BuildCanvasVisual()
        {
            GameObject canvasObject = new GameObject("MiaMax Joystick Canvas",
                typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            canvasObject.transform.SetParent(transform, false);

            visualCanvas = canvasObject.GetComponent<Canvas>();
            visualCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            visualCanvas.sortingOrder = canvasSortingOrder;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            GameObject baseObject = new GameObject("Joystick Base",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            baseObject.transform.SetParent(canvasObject.transform, false);
            visualRoot = (RectTransform)baseObject.transform;
            Image baseImage = baseObject.GetComponent<Image>();
            baseImage.sprite = circleSprite;
            baseImage.color = new Color(1f, 1f, 1f, 0.22f);
            baseImage.raycastTarget = false;

            GameObject knobObject = new GameObject("Joystick Knob",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            knobObject.transform.SetParent(baseObject.transform, false);
            knobRect = (RectTransform)knobObject.transform;
            knobRect.anchorMin = new Vector2(0.5f, 0.5f);
            knobRect.anchorMax = new Vector2(0.5f, 0.5f);
            knobRect.pivot = new Vector2(0.5f, 0.5f);
            Image knobImage = knobObject.GetComponent<Image>();
            knobImage.sprite = circleSprite;
            knobImage.color = new Color(1f, 1f, 1f, 0.55f);
            knobImage.raycastTarget = false;
        }

        private void UpdateCanvasLayout(bool force)
        {
            if (visualRoot == null || visualCanvas == null) return;

            Rect safe = Screen.safeArea;
            if (!force && lastScreenWidth == Screen.width &&
                lastScreenHeight == Screen.height && lastSafeArea == safe)
                return;

            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            lastSafeArea = safe;

            Rect touchRect = JoystickRect;
            float scale = Mathf.Max(0.01f, visualCanvas.scaleFactor);
            Vector2 center = touchRect.center;

            visualRoot.anchorMin = new Vector2(center.x / Screen.width, center.y / Screen.height);
            visualRoot.anchorMax = visualRoot.anchorMin;
            visualRoot.pivot = new Vector2(0.5f, 0.5f);
            visualRoot.anchoredPosition = Vector2.zero;
            visualRoot.sizeDelta = new Vector2(touchRect.width / scale, touchRect.height / scale);

            float knobDiameter = EffectiveRadius * 0.76f / scale;
            knobRect.sizeDelta = new Vector2(knobDiameter, knobDiameter);
            UpdateKnobVisual();
        }

        private void UpdateKnobVisual()
        {
            if (knobRect == null || visualCanvas == null) return;
            float scale = Mathf.Max(0.01f, visualCanvas.scaleFactor);
            knobRect.anchoredPosition = Value * (EffectiveRadius * 0.72f / scale);
        }

        private static Texture2D CreateCircleTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = "MiaMax Joystick Circle Texture";
            texture.hideFlags = HideFlags.HideAndDontSave;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;

            Color[] pixels = new Color[size * size];
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float edge = size * 0.48f;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Clamp01(edge + 1f - distance);
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }

            texture.SetPixels(pixels);
            texture.Apply(false, true);
            return texture;
        }
    }
}