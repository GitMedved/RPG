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
        private Text feedbackText;
        private GameObject detailPanel;
        private RectTransform detailPanelRect;
        private RectTransform detailViewportRect;
        private RectTransform settingsControlsRect;
        private RectTransform journalControlsRect;
        private readonly RectTransform[] settingLabelRects = new RectTransform[5];
        private readonly RectTransform[] settingMinusRects = new RectTransform[5];
        private readonly RectTransform[] settingPlusRects = new RectTransform[5];
        private readonly RectTransform[] journalActionRects = new RectTransform[11];
        private Text detailTitle;
        private Text detailBody;
        private RectTransform detailContent;
        private ScrollRect detailScroll;
        private GameObject settingsControls;
        private GameObject journalControls;
        private Text vibrationLabel;
        private Text reducedMotionLabel;
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
        private bool canvasReady;

        private void Start()
        {
            hud = GetComponent<PrototypeHud>();
            BuildCanvas();
            canvasReady = HasRequiredReferences();
            if (!canvasReady)
                Debug.LogError("[Mia & Max] Mobile HUD was not fully constructed. Check the preceding Canvas setup exception; Refresh has been disabled to prevent cascading NullReferenceExceptions.", this);
            Refresh();
        }

        private void Update()
        {
            if (!canvasReady || Time.unscaledTime < nextRefresh) return;
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

            Image statusCard = MakePanel("Player Status Card", safeRoot, MobileUiTheme.Forest,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(16f, -16f),
                new Vector2(352f, 174f), new Vector2(0f, 1f));
            statusText = MakeText("Player Status", statusCard.transform, string.Empty, 27,
                TextAnchor.MiddleLeft, MobileUiTheme.Cream, new Vector2(20f, 12f), new Vector2(-20f, -12f));
            statusText.horizontalOverflow = HorizontalWrapMode.Wrap;
            statusText.verticalOverflow = VerticalWrapMode.Overflow;

            Image giftPanel = MakePanel("Daily Gift Panel", safeRoot, MobileUiTheme.SoftPanel,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(16f, -202f),
                new Vector2(352f, 74f), new Vector2(0f, 1f));
            giftButton = MakeButton("Daily Gift", giftPanel.transform, MobileUiTheme.Honey, "DAILY GIFT",
                Vector2.zero, Vector2.one, out giftButtonText, 23);
            giftButton.onClick.AddListener(() => hud?.ClaimDailyGift());

            journalButton = MakeTopButton("Journal Button", "ADVENTURE JOURNAL", safeRoot,
                new Vector2(1f, 1f), new Vector2(-16f, -16f), MobileUiTheme.Forest, out journalButtonText);
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
            backpackButton = MakeButton("Backpack Button", dock.transform, MobileUiTheme.Leaf, "BACKPACK",
                Vector2.zero, Vector2.one, out backpackButtonText, 25);
            RectTransform backpackRect = (RectTransform)backpackButton.transform;
            backpackRect.anchorMin = new Vector2(0f, 0f);
            backpackRect.anchorMax = new Vector2(0.5f, 1f);
            backpackRect.offsetMin = new Vector2(10f, 10f);
            backpackRect.offsetMax = new Vector2(-6f, -10f);
            backpackButton.onClick.AddListener(() => hud?.ToggleInventory());

            interactButton = MakeButton("Interact Button", dock.transform, MobileUiTheme.Honey, "EXPLORE",
                Vector2.zero, Vector2.one, out interactButtonText, 25);
            RectTransform interactRect = (RectTransform)interactButton.transform;
            interactRect.anchorMin = new Vector2(0.5f, 0f);
            interactRect.anchorMax = new Vector2(1f, 1f);
            interactRect.offsetMin = new Vector2(6f, 10f);
            interactRect.offsetMax = new Vector2(-10f, -10f);
            interactButton.onClick.AddListener(() => hud?.TryInteractFromHud());

            promptText = MakeText("Nearby Prompt", safeRoot, "Explore the backyard", 24,
                TextAnchor.MiddleCenter, MobileUiTheme.Cream, Vector2.zero, Vector2.zero);
            promptText.rectTransform.anchorMin = new Vector2(0.08f, 0f);
            promptText.rectTransform.anchorMax = new Vector2(0.92f, 0f);
            promptText.rectTransform.pivot = new Vector2(0.5f, 0f);
            promptText.rectTransform.anchoredPosition = new Vector2(0f, 170f);
            promptText.rectTransform.sizeDelta = new Vector2(0f, 58f);

            MakePanel("Tutorial Hint Background", safeRoot, new Color(0.96f, 0.91f, 0.78f, 0.94f),
                new Vector2(0.08f, 1f), new Vector2(0.92f, 1f), new Vector2(0f, -270f),
                new Vector2(0f, 66f), new Vector2(0.5f, 1f));
            tutorialText = MakeText("Tutorial Hint", safeRoot, string.Empty, 22,
                TextAnchor.MiddleCenter, MobileUiTheme.Ink, Vector2.zero, Vector2.zero);
            tutorialText.rectTransform.anchorMin = new Vector2(0.08f, 1f);
            tutorialText.rectTransform.anchorMax = new Vector2(0.92f, 1f);
            tutorialText.rectTransform.pivot = new Vector2(0.5f, 1f);
            tutorialText.rectTransform.anchoredPosition = new Vector2(0f, -270f);
            tutorialText.rectTransform.sizeDelta = new Vector2(0f, 66f);

            MakePanel("Gameplay Feedback Background", safeRoot, new Color(0.12f, 0.19f, 0.17f, 0.96f),
                new Vector2(0.12f, 1f), new Vector2(0.88f, 1f), new Vector2(0f, -350f),
                new Vector2(0f, 76f), new Vector2(0.5f, 1f));
            feedbackText = MakeText("Gameplay Feedback", safeRoot, string.Empty, 25,
                TextAnchor.MiddleCenter, MobileUiTheme.Cream, Vector2.zero, Vector2.zero);
            feedbackText.rectTransform.anchorMin = new Vector2(0.12f, 1f);
            feedbackText.rectTransform.anchorMax = new Vector2(0.88f, 1f);
            feedbackText.rectTransform.pivot = new Vector2(0.5f, 1f);
            feedbackText.rectTransform.anchoredPosition = new Vector2(0f, -350f);
            feedbackText.rectTransform.sizeDelta = new Vector2(0f, 76f);

            BuildDetailPanel(safeRoot);
        }

        private void BuildDetailPanel(Transform parent)
        {
            Image panel = MakePanel("Canvas Detail Panel", parent, MobileUiTheme.DeepPanel,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero,
                new Vector2(620f, 1040f), new Vector2(0.5f, 0.5f));
            detailPanel = panel.gameObject;
            detailPanelRect = (RectTransform)panel.transform;
            detailTitle = MakeText("Detail Panel Title", panel.transform, "BACKPACK", 34,
                TextAnchor.MiddleLeft, MobileUiTheme.Cream, new Vector2(28f, -92f), new Vector2(-100f, -18f));
            detailTitle.rectTransform.anchorMin = new Vector2(0f, 1f);
            detailTitle.rectTransform.anchorMax = new Vector2(1f, 1f);
            detailTitle.rectTransform.pivot = new Vector2(0.5f, 1f);
            Button close = MakeButton("Detail Panel Close", panel.transform, MobileUiTheme.Honey, "CLOSE",
                Vector2.zero, Vector2.one, out _, 20);
            RectTransform closeRect = (RectTransform)close.transform;
            closeRect.anchorMin = new Vector2(1f, 1f);
            closeRect.anchorMax = new Vector2(1f, 1f);
            closeRect.pivot = new Vector2(1f, 1f);
            closeRect.anchoredPosition = new Vector2(-18f, -18f);
            closeRect.sizeDelta = new Vector2(92f, 58f);
            close.onClick.AddListener(() => hud?.CloseCanvasDetails());

            GameObject viewportObject = new GameObject("Detail Scroll Viewport",
                typeof(RectTransform), typeof(Image), typeof(Mask));
            viewportObject.transform.SetParent(panel.transform, false);
            RectTransform viewport = (RectTransform)viewportObject.transform;
            detailViewportRect = viewport;
            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.offsetMin = new Vector2(24f, 300f);
            viewport.offsetMax = new Vector2(-24f, -118f);
            Image viewportImage = viewportObject.GetComponent<Image>();
            viewportImage.color = new Color(1f, 1f, 1f, 0.015f);
            viewportImage.raycastTarget = true;
            viewportObject.GetComponent<Mask>().showMaskGraphic = false;

            GameObject contentObject = new GameObject("Detail Scroll Content",
                typeof(RectTransform));
            contentObject.transform.SetParent(viewport, false);
            detailContent = (RectTransform)contentObject.transform;
            detailContent.anchorMin = new Vector2(0f, 1f);
            detailContent.anchorMax = new Vector2(1f, 1f);
            detailContent.pivot = new Vector2(0.5f, 1f);
            detailContent.anchoredPosition = Vector2.zero;
            detailContent.sizeDelta = new Vector2(0f, 600f);

            detailBody = MakeText("Detail Panel Body", detailContent, string.Empty, 29,
                TextAnchor.UpperLeft, MobileUiTheme.Cream, new Vector2(8f, 8f), new Vector2(-8f, -8f));
            detailBody.horizontalOverflow = HorizontalWrapMode.Wrap;
            detailBody.verticalOverflow = VerticalWrapMode.Overflow;

            detailScroll = viewportObject.AddComponent<ScrollRect>();
            detailScroll.viewport = viewport;
            detailScroll.content = detailContent;
            detailScroll.horizontal = false;
            detailScroll.vertical = true;
            detailScroll.movementType = ScrollRect.MovementType.Clamped;
            detailScroll.scrollSensitivity = 36f;
            detailScroll.inertia = true;

            settingsControls = new GameObject("Settings Canvas Controls", typeof(RectTransform));
            settingsControls.transform.SetParent(panel.transform, false);
            RectTransform controlsRect = (RectTransform)settingsControls.transform;
            settingsControlsRect = controlsRect;
            controlsRect.anchorMin = new Vector2(0f, 0f);
            controlsRect.anchorMax = new Vector2(1f, 0f);
            controlsRect.pivot = new Vector2(0.5f, 0f);
            controlsRect.anchoredPosition = new Vector2(0f, 20f);
            controlsRect.sizeDelta = new Vector2(-48f, 260f);

            string[] settingNames = { "MASTER", "EFFECTS", "MUSIC", "VOICE", "UI SCALE" };
            for (int i = 0; i < settingNames.Length; i++)
            {
                int setting = i;
                float y = 216f - i * 42f;
                Text label = MakeText("Setting " + settingNames[i], controlsRect, settingNames[i], 22,
                    TextAnchor.MiddleLeft, MobileUiTheme.Cream, Vector2.zero, Vector2.zero);
                settingLabelRects[i] = label.rectTransform;
                label.rectTransform.anchorMin = new Vector2(0f, 0f);
                label.rectTransform.anchorMax = new Vector2(0.52f, 0f);
                label.rectTransform.pivot = new Vector2(0f, 0f);
                label.rectTransform.anchoredPosition = new Vector2(8f, y);
                label.rectTransform.sizeDelta = new Vector2(0f, 38f);

                Button minus = MakeButton("Decrease " + settingNames[i], controlsRect, MobileUiTheme.Leaf, "−",
                    Vector2.zero, Vector2.one, out _, 24);
                settingMinusRects[i] = (RectTransform)minus.transform;
                SetControlRect(minus, new Vector2(0.62f, 0f), new Vector2(0.74f, 0f),
                    new Vector2(0f, y), new Vector2(0f, 38f));
                minus.onClick.AddListener(() => hud?.AdjustSettingFromCanvas(setting, -0.05f));

                Button plus = MakeButton("Increase " + settingNames[i], controlsRect, MobileUiTheme.Honey, "+",
                    Vector2.zero, Vector2.one, out _, 24);
                settingPlusRects[i] = (RectTransform)plus.transform;
                SetControlRect(plus, new Vector2(0.80f, 0f), new Vector2(0.92f, 0f),
                    new Vector2(0f, y), new Vector2(0f, 38f));
                plus.onClick.AddListener(() => hud?.AdjustSettingFromCanvas(setting, 0.05f));
            }

            Button vibration = MakeButton("Vibration Toggle", controlsRect, MobileUiTheme.Leaf, "VIBRATION",
                Vector2.zero, Vector2.one, out vibrationLabel, 20);
            SetControlRect(vibration, new Vector2(0f, 0f), new Vector2(0.48f, 0f),
                new Vector2(0f, 8f), new Vector2(0f, 38f));
            vibration.onClick.AddListener(() => hud?.ToggleVibrationFromCanvas());

            Button reduced = MakeButton("Reduced Motion Toggle", controlsRect, MobileUiTheme.Leaf, "REDUCED MOTION",
                Vector2.zero, Vector2.one, out reducedMotionLabel, 18);
            SetControlRect(reduced, new Vector2(0.52f, 0f), new Vector2(1f, 0f),
                new Vector2(0f, 8f), new Vector2(0f, 38f));
            reduced.onClick.AddListener(() => hud?.ToggleReducedMotionFromCanvas());
            settingsControls.SetActive(false);

            journalControls = new GameObject("Adventure Journal Canvas Actions", typeof(RectTransform));
            journalControls.transform.SetParent(panel.transform, false);
            RectTransform journalRect = (RectTransform)journalControls.transform;
            journalControlsRect = journalRect;
            journalRect.anchorMin = new Vector2(0f, 0f);
            journalRect.anchorMax = new Vector2(1f, 0f);
            journalRect.pivot = new Vector2(0.5f, 0f);
            journalRect.anchoredPosition = new Vector2(0f, 20f);
            journalRect.sizeDelta = new Vector2(-48f, 260f);
            string[] journalActions =
            {
                "CLAIM QUEST", "CLAIM KEEPSAKES",
                "MAKE SOUP", "MAKE JUICE",
                "MAKE WOODEN TOY", "COLLECT PRODUCT",
                "DELIVER ORDER", "UPGRADE GARDEN",
                "UPGRADE WORKSHOP", "CLAIM DAILY",
                "CLAIM ACHIEVEMENT"
            };
            for (int i = 0; i < journalActions.Length; i++)
            {
                int action = i;
                int row = i / 2;
                int column = i % 2;
                Button actionButton = MakeButton("Journal Action " + action, journalRect,
                    action % 3 == 0 ? MobileUiTheme.Honey : MobileUiTheme.Leaf, journalActions[i],
                    Vector2.zero, Vector2.one, out _, 17);
                journalActionRects[i] = (RectTransform)actionButton.transform;
                SetControlRect(actionButton,
                    new Vector2(column * 0.5f, 0f), new Vector2((column + 1) * 0.5f, 0f),
                    new Vector2(column == 0 ? 0f : 4f, 216f - row * 42f),
                    new Vector2(column == 0 ? -6f : -0f, 38f));
                actionButton.onClick.AddListener(() => hud?.PerformJournalAction(action));
            }
            journalControls.SetActive(false);
        }

        private static void SetControlRect(Button button, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 position, Vector2 size)
        {
            RectTransform rect = (RectTransform)button.transform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        private void ApplySafeArea()
        {
            Rect safe = Screen.safeArea;
            safeRoot.anchorMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
            safeRoot.anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
            safeRoot.offsetMin = Vector2.zero;
            safeRoot.offsetMax = Vector2.zero;
        }

        private void ApplyResponsiveLayout()
        {
            if (safeRoot == null || detailPanelRect == null) return;

            // Keep the detail sheet within the safe-area viewport in both portrait
            // and landscape. CanvasScaler supplies the device-specific scale factor.
            float availableWidth = Mathf.Max(1f, safeRoot.rect.width - 32f);
            float availableHeight = Mathf.Max(1f, safeRoot.rect.height - 32f);
            float panelWidth = Mathf.Min(620f, availableWidth);
            float panelHeight = Mathf.Min(1040f, availableHeight);
            detailPanelRect.sizeDelta = new Vector2(panelWidth, panelHeight);

            // Reserve a compact, predictable action zone at the bottom. The scroll
            // viewport receives all remaining space instead of being squeezed to a
            // near-zero height on short landscape displays.
            float controlsHeight = Mathf.Min(260f, Mathf.Max(136f, panelHeight * 0.43f));
            if (settingsControlsRect != null)
                settingsControlsRect.sizeDelta = new Vector2(-48f, controlsHeight);
            if (journalControlsRect != null)
                journalControlsRect.sizeDelta = new Vector2(-48f, controlsHeight);
            if (detailViewportRect != null)
            {
                detailViewportRect.offsetMin = new Vector2(24f, controlsHeight + 28f);
                detailViewportRect.offsetMax = new Vector2(-24f, -118f);
            }

            float compact = Mathf.Clamp01((720f - panelHeight) / 420f);
            float rowHeight = Mathf.Lerp(38f, 20f, compact);
            float rowGap = Mathf.Lerp(4f, 2f, compact);
            float settingStep = rowHeight + rowGap;
            for (int i = 0; i < settingLabelRects.Length; i++)
            {
                float y = rowHeight + 6f + (4 - i) * settingStep;
                SetResponsiveRow(settingLabelRects[i], y, rowHeight);
                SetResponsiveRow(settingMinusRects[i], y, rowHeight);
                SetResponsiveRow(settingPlusRects[i], y, rowHeight);
            }

            SetResponsiveRow((RectTransform)vibrationLabel.transform.parent, 0f, rowHeight);
            SetResponsiveRow((RectTransform)reducedMotionLabel.transform.parent, 0f, rowHeight);

            for (int i = 0; i < journalActionRects.Length; i++)
            {
                RectTransform rect = journalActionRects[i];
                if (rect == null) continue;
                int row = i / 2;
                int column = i % 2;
                rect.anchorMin = new Vector2(column * 0.5f, 0f);
                rect.anchorMax = new Vector2((column + 1) * 0.5f, 0f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.anchoredPosition = new Vector2(column == 0 ? 0f : 4f,
                    controlsHeight - rowHeight - 4f - row * (rowHeight + rowGap));
                rect.sizeDelta = new Vector2(column == 0 ? -6f : 0f, rowHeight);
            }
        }

        private static RectTransform MakeRect(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rect = (RectTransform)go.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
            return rect;
        }

        private static void SetResponsiveRow(RectTransform rect, float y, float height)
        {
            if (rect == null) return;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, y);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
        }

        private bool HasRequiredReferences()
        {
            return hud != null && statusText != null && detailPanel != null &&
                settingsControls != null && journalControls != null &&
                detailTitle != null && detailBody != null && detailContent != null && detailScroll != null &&
                vibrationLabel != null && reducedMotionLabel != null &&
                giftButton != null && giftButtonText != null &&
                journalButtonText != null && settingsButtonText != null && mapButtonText != null &&
                backpackButtonText != null && interactButton != null && interactButtonText != null &&
                promptText != null && feedbackText != null && tutorialText != null;
        }

        private void Refresh()
        {
            if (!canvasReady || !HasRequiredReferences()) return;

            ApplySafeArea();
            ApplyResponsiveLayout();
            statusText.text = hud.StatusText;
            bool showDetails = hud.InventoryOpen || hud.MapOpen || hud.SettingsOpen || hud.JournalOpen;
            detailPanel.SetActive(showDetails);
            settingsControls.SetActive(hud.SettingsOpen);
            journalControls.SetActive(hud.JournalOpen);
            if (showDetails)
            {
                detailTitle.text = hud.InventoryOpen ? "BACKPACK" : hud.MapOpen ? "WORLD MAP" :
                    hud.SettingsOpen ? "SETTINGS" : "ADVENTURE JOURNAL";
                detailBody.text = hud.InventoryOpen ? hud.InventorySummary :
                    hud.MapOpen ? hud.WorldMapSummary : hud.SettingsOpen ? hud.SettingsSummary : hud.JournalSummary;
                float preferred = detailBody.preferredHeight + 24f;
                detailContent.sizeDelta = new Vector2(0f, Mathf.Max(560f, preferred));
                detailScroll.verticalNormalizedPosition = Mathf.Clamp01(detailScroll.verticalNormalizedPosition);
                vibrationLabel.text = hud.VibrationEnabled ? "VIBRATION: ON" : "VIBRATION: OFF";
                reducedMotionLabel.text = hud.ReducedMotionEnabled ? "REDUCED MOTION: ON" : "REDUCED MOTION: OFF";
            }
            giftButtonText.text = hud.DailyGiftLabel;
            MobileUiTheme.ApplyAvailability(giftButton, hud.CanClaimDailyGift, MobileUiTheme.Honey);

            journalButtonText.text = hud.JournalOpen ? "CLOSE JOURNAL" : "ADVENTURE JOURNAL";
            settingsButtonText.text = hud.SettingsOpen ? "CLOSE SETTINGS" : "SETTINGS";
            mapButtonText.text = hud.MapOpen ? "CLOSE MAP" : "WORLD MAP";
            backpackButtonText.text = hud.InventoryOpen ? "CLOSE BACKPACK" : "BACKPACK";

            bool canInteract = hud.CanInteract;
            interactButton.interactable = canInteract;
            interactButtonText.text = canInteract ? "INTERACT" : "EXPLORE";
            interactButton.targetGraphic.color = canInteract ? MobileUiTheme.Honey : new Color(0.43f, 0.46f, 0.39f, 0.9f);
            promptText.text = hud.InteractionPrompt;
            feedbackText.text = hud.FeedbackText;
            feedbackText.gameObject.SetActive(!string.IsNullOrEmpty(hud.FeedbackText));

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
            MobileUiTheme.ApplyButton(button, image, color);
            labelText = MakeText(name + " Label", go.transform, label, fontSize,
                TextAnchor.MiddleCenter, MobileUiTheme.Cream, new Vector2(8f, 4f), new Vector2(-8f, -4f));
            labelText.fontStyle = FontStyle.Bold;
            labelText.color = MobileUiTheme.GetReadableButtonText(color);
            labelText.resizeTextForBestFit = true;
            labelText.resizeTextMinSize = Mathf.Max(MobileUiTheme.MinimumButtonFontSize, fontSize - MobileUiTheme.ButtonFontShrink);
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
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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
