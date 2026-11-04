using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Responsive uGUI shell for the mobile HUD. Detailed legacy information panels
    /// are temporarily rendered by PrototypeHud until their individual Canvas migration.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MobileHudCanvas : MonoBehaviour
    {
        private PrototypeHud hud;
        private RectTransform safeRoot;
        private Text statusText;
        private Text promptText;
        private Text tutorialText;
        private Button giftButton;
        private Text giftButtonText;
        private Button journalButton;
        private Text journalButtonText;
        private Button settingsButton;
        private Text settingsButtonText;
        private Button mapButton;
        private Text mapButtonText;
        private Button backpackButton;
        private Text backpackButtonText;
        private Button interactButton;
        private Text interactButtonText;
        private float nextRefresh;

        private static readonly Color Ink = new Color(0.16f, 0.20f, 0.18f, 1f);
        private static readonly Color Cream = new Color(0.98f, 0.94f, 0.82f, 1f);
        private static readonly Color Forest = new Color(0.22f, 0.36f, 0.29f, 0.98f);
        private static readonly Color Leaf = new Color(0.38f, 0.53f, 0.35f, 1f);
        private static readonly Color Honey = new Color(0.82f, 0.57f, 0.25f, 1f);

        private void Start()
        {
            hud = GetComponent<PrototypeHud>();
            BuildCanvas();
            Refresh();
        }

        private void Update()
        {
            if (Time.unscaledTime < nextRefresh) return;
            nextRefresh = Time.unscaledTime + 0.2f;
            Refresh();
        }

        private void BuildCanvas()
        {
            Canvas canvas = new GameObject("MiaMax Mobile HUD Canvas").AddComponent<Canvas>();
            canvas.transform.SetParent(transform, false);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 30;

            CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            canvas.gameObject.AddComponent<GraphicRaycaster>();

            EnsureEventSystem();

            safeRoot = MakeRect("Safe Area", canvas.transform);
            ApplySafeArea();

            Image statusCard = MakePanel("Player Status Card", safeRoot, Forest,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(16f, -16f),
                new Vector2(352f, 174f), new Vector2(0f, 1f));
            statusText = MakeText("Player Status", statusCard.transform, string.Empty, 27,
                TextAnchor.MiddleLeft, Cream, new Vector2(20f, 12f), new Vector2(-20f, -12f));
            statusText.horizontalOverflow = HorizontalWrapMode.Wrap;
            statusText.verticalOverflow = VerticalWrapMode.Overflow;

            Image giftPanel = MakePanel("Daily Gift Panel", safeRoot, new Color(0.94f, 0.88f, 0.72f, 0.98f),
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(16f, -202f),
                new Vector2(352f, 74f), new Vector2(0f, 1f));
            giftButton = MakeButton("Daily Gift", giftPanel.transform, Honey, "DAILY GIFT",
                new Vector2(8f, 8f), new Vector2(-8f, -8f), out giftButtonText, 23);
            giftButton.onClick.AddListener(() => hud?.ClaimDailyGift());

            journalButton = MakeTopButton("Journal Button", "ADVENTURE JOURNAL", safeRoot,
                new Vector2(1f, 1f), new Vector2(-16f, -16f), Forest, out journalButtonText);
            journalButton.onClick.AddListener(() => hud?.ToggleJournal());
            settingsButton = MakeTopButton("Settings Button", "SETTINGS", safeRoot,
                new Vector2(1f, 1f), new Vector2(-16f, -94f), new Color(0.30f, 0.39f, 0.39f, 0.98f), out settingsButtonText);
            settingsButton.onClick.AddListener(() => hud?.ToggleSettings());
            mapButton = MakeTopButton("World Map Button", "WORLD MAP", safeRoot,
                new Vector2(1f, 1f), new Vector2(-16f, -172f), new Color(0.34f, 0.46f, 0.34f, 0.98f), out mapButtonText);
            mapButton.onClick.AddListener(() => hud?.ToggleMap());

            Image dock = MakePanel("Bottom Action Dock", safeRoot, new Color(0.13f, 0.20f, 0.18f, 0.96f),
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 18f),
                new Vector2(680f, 142f), new Vector2(0.5f, 0f));
            backpackButton = MakeButton("Backpack Button", dock.transform, Leaf, "BACKPACK",
                new Vector2(12f, 12f), new Vector2(0.5f, -12f), out backpackButtonText, 25);
            RectTransform backpackRect = (RectTransform)backpackButton.transform;
            backpackRect.anchorMin = new Vector2(0f, 0f);
            backpackRect.anchorMax = new Vector2(0.5f, 1f);
            backpackRect.offsetMin = new Vector2(10f, 10f);
            backpackRect.offsetMax = new Vector2(-6f, -10f);
            backpackButton.onClick.AddListener(() => hud?.ToggleInventory());

            interactButton = MakeButton("Interact Button", dock.transform, Honey, "EXPLORE",
                new Vector2(0.5f, 0f), new Vector2(-10f, 1f), out interactButtonText, 25);
            RectTransform interactRect = (RectTransform)interactButton.transform;
            interactRect.anchorMin = new Vector2(0.5f, 0f);
            interactRect.anchorMax = new Vector2(1f, 1f);
            interactRect.offsetMin = new Vector2(6f, 10f);
            interactRect.offsetMax = new Vector2(-10f, -10f);
            interactButton.onClick.AddListener(() => hud?.TryInteractFromHud());

            promptText = MakeText("Nearby Prompt", safeRoot, "Explore the backyard", 24,
                TextAnchor.MiddleCenter, Cream, new Vector2(0f, 170f), new Vector2(0f, 58f));
            promptText.rectTransform.anchorMin = new Vector2(0.08f, 0f);
            promptText.rectTransform.anchorMax = new Vector2(0.92f, 0f);
            promptText.rectTransform.pivot = new Vector2(0.5f, 0f);

            tutorialText = MakeText("Tutorial Hint", safeRoot, string.Empty, 22,
                TextAnchor.MiddleCenter, Ink, new Vector2(0f, 1f), new Vector2(0f, -270f));
            tutorialText.rectTransform.anchorMin = new Vector2(0.08f, 1f);
            tutorialText.rectTransform.anchorMax = new Vector2(0.92f, 1f);
            tutorialText.rectTransform.pivot = new Vector2(0.5f, 1f);
            tutorialText.rectTransform.anchoredPosition = new Vector2(0f, -270f);
            tutorialText.rectTransform.sizeDelta = new Vector2(0f, 66f);
            tutorialText.color = Cream;
        }

        private void ApplySafeArea()
        {
            Rect safe = Screen.safeArea;
            safeRoot.anchorMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
            safeRoot.anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
            safeRoot.offsetMin = Vector2.zero;
            safeRoot.offsetMax = Vector2.zero;
        }

        private void Refresh()
        {
            if (hud == null) return;
            ApplySafeArea();
            statusText.text = hud.StatusText;
            giftButtonText.text = hud.DailyGiftLabel;
            giftButton.interactable = hud.CanClaimDailyGift;
            giftButton.targetGraphic.color = hud.CanClaimDailyGift ? Honey : new Color(0.43f, 0.46f, 0.39f, 0.9f);

            journalButtonText.text = hud.JournalOpen ? "CLOSE JOURNAL" : "ADVENTURE JOURNAL";
            settingsButtonText.text = hud.SettingsOpen ? "CLOSE SETTINGS" : "SETTINGS";
            mapButtonText.text = hud.MapOpen ? "CLOSE MAP" : "WORLD MAP";
            backpackButtonText.text = hud.InventoryOpen ? "CLOSE BACKPACK" : "BACKPACK";

            bool canInteract = hud.CanInteract;
            interactButton.interactable = canInteract;
            interactButtonText.text = canInteract ? "INTERACT" : "EXPLORE";
            interactButton.targetGraphic.color = canInteract ? Honey : new Color(0.43f, 0.46f, 0.39f, 0.9f);
            promptText.text = hud.InteractionPrompt;

            string hint = hud.TutorialHint;
            tutorialText.text = hint;
            tutorialText.gameObject.SetActive(!string.IsNullOrEmpty(hint));
        }

        private Button MakeTopButton(string name, string label, Transform parent, Vector2 anchor,
            Vector2 position, Color color, out Text labelText)
        {
            Button button = MakeButton(name, parent, color, label,
                new Vector2(0f, 1f), new Vector2(0f, 1f), out labelText, 21);
            RectTransform rect = (RectTransform)button.transform;
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(280f, 66f);
            return button;
        }

        private static Button MakeButton(string name, Transform parent, Color color, string label,
            Vector2 anchorMin, Vector2 anchorMax, out Text labelText, int fontSize)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            RectTransform rect = (RectTransform)go.transform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            Image image = go.GetComponent<Image>();
            image.color = color;
            Button button = go.GetComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.ColorTint;
            ColorBlock colors = button.colors;
            colors.normalColor = color;
            colors.highlightedColor = Color.Lerp(color, Color.white, 0.12f);
            colors.pressedColor = Color.Lerp(color, Ink, 0.18f);
            colors.disabledColor = new Color(color.r, color.g, color.b, 0.55f);
            button.colors = colors;
            labelText = MakeText(name + " Label", go.transform, label, fontSize,
                TextAnchor.MiddleCenter, Cream, new Vector2(8f, 4f), new Vector2(-8f, -4f));
            labelText.fontStyle = FontStyle.Bold;
            labelText.resizeTextForBestFit = true;
            labelText.resizeTextMinSize = Mathf.Max(14, fontSize - 5);
            labelText.resizeTextMaxSize = fontSize;
            labelText.raycastTarget = false;
            return button;
        }

        private static Image MakePanel(string name, Transform parent, Color color,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size, Vector2 pivot)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            RectTransform rect = (RectTransform)go.transform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            Image image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static Text MakeText(string name, Transform parent, string value, int size,
            TextAnchor alignment, Color color, Vector2 offsetMin, Vector2 offsetMax)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            RectTransform rect = (RectTransform)go.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            Text text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.alignment = alignment;
            text.color = color;
            text.text = value;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            return text;
        }

        private void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null) return;
            GameObject eventSystem = new GameObject("MiaMax EventSystem",
                typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.transform.SetParent(transform, false);
        }
    }
}
