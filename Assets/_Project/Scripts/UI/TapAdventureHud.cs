using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Portrait-first accessible UGUI HUD. The UI is fully functional without any
    /// imported UI pack, prefabs, or TextMeshPro dependency.
    /// The old OnGUI renderer remains only as a fallback if HUD construction fails.
    /// </summary>
    public sealed class TapAdventureHud : MonoBehaviour
    {
        private TapAdventureWorldBootstrap game;
        private Canvas canvas;
        private RectTransform safeRoot, header, quest, card, hint, miniGame, dialog;
        private Text resourceText, questText, cardTitle, cardBody, actionLabel;
        private Text toastText, hintText, miniGameMessage, soundLabel, locationTitle, dialogTitle;
        private Button actionButton, backButton, soundButton;
        private Rect lastSafe;
        private bool muted;
        private float nextRefresh;
        private static Sprite rounded;
        private static Font uiFont;

        private static readonly Color Cream = new Color(.998f, .968f, .909f, .98f);
        private static readonly Color Paper = new Color(.98f, .94f, .85f, .985f);
        private static readonly Color Ink = new Color(.18f, .23f, .19f);
        private static readonly Color Muted = new Color(.40f, .45f, .39f);
        private static readonly Color Forest = new Color(.20f, .43f, .34f);
        private static readonly Color ForestLight = new Color(.31f, .58f, .41f);
        private static readonly Color Gold = new Color(.94f, .70f, .34f);
        private static readonly Color Coral = new Color(.89f, .47f, .37f);

        public static TapAdventureHud Install(TapAdventureWorldBootstrap owner)
        {
            GameObject root = new GameObject("Mia & Max - Mobile HUD", typeof(RectTransform));
            root.transform.SetParent(owner.transform, false);
            TapAdventureHud hud = root.AddComponent<TapAdventureHud>();
            hud.game = owner;
            hud.Build();
            return hud;
        }

        private static Sprite RoundedSprite()
        {
            if (rounded != null) return rounded;
            const int size = 64, radius = 16;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = "MiaMax Rounded Interface";
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(0f, radius - x - .5f, x - size + radius + .5f);
                    float dy = Mathf.Max(0f, radius - y - .5f, y - size + radius + .5f);
                    float distance = Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = Mathf.Clamp01(radius - distance + .5f);
                    pixels[y * size + x] = new Color(1, 1, 1, alpha);
                }
            texture.SetPixels(pixels);
            texture.Apply(false, true);
            rounded = Sprite.Create(texture, new Rect(0, 0, size, size),
                new Vector2(.5f, .5f), 100f, 0,
                SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
            rounded.name = "MiaMax Card NineSlice";
            return rounded;
        }

        private static Font Font()
        {
            if (uiFont == null)
                uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return uiFont;
        }

        private RectTransform Element(string name, Transform parent,
            Vector2 min, Vector2 max, Vector2 size, Vector2 pos)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = min;
            rt.anchorMax = max;
            rt.pivot = new Vector2(.5f, .5f);
            rt.sizeDelta = size;
            rt.anchoredPosition = pos;
            return rt;
        }

        private RectTransform Panel(string name, Transform parent,
            Vector2 min, Vector2 max, Vector2 size, Vector2 pos,
            Color color)
        {
            RectTransform rt = Element(name, parent, min, max, size, pos);
            Image image = rt.gameObject.AddComponent<Image>();
            image.sprite = RoundedSprite();
            image.type = Image.Type.Sliced;
            image.color = color;
            image.raycastTarget = true;
            return rt;
        }

        private Text Label(string name, Transform parent, string value,
            int size, Color color, FontStyle style, TextAnchor anchor,
            Vector2 min, Vector2 max, Vector2 delta, Vector2 pos)
        {
            RectTransform rt = Element(name, parent, min, max, delta, pos);
            Text text = rt.gameObject.AddComponent<Text>();
            text.font = Font();
            text.fontSize = size;
            text.color = color;
            text.fontStyle = style;
            text.alignment = anchor;
            text.supportRichText = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            text.text = value;
            return text;
        }

        private Button Press(string name, Transform parent, string value,
            Vector2 min, Vector2 max, Vector2 size, Vector2 pos,
            Color baseColor, Color foreground, Action callback)
        {
            RectTransform rt = Panel(name, parent, min, max, size, pos, baseColor);
            Button button = rt.gameObject.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(.93f, .97f, .90f);
            colors.pressedColor = new Color(.78f, .85f, .77f);
            colors.selectedColor = Color.white;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = .12f;
            button.colors = colors;
            button.onClick.AddListener(() => callback());
            Label("Button label", rt, value, 29, foreground, FontStyle.Bold,
                TextAnchor.MiddleCenter, Vector2.zero, Vector2.one,
                new Vector2(-18, -7), Vector2.zero);
            return button;
        }

        private void Build()
        {
            muted = PlayerPrefs.GetInt("MiaMax.UI.SoundMuted", 0) == 1;
            AudioListener.volume = muted ? 0f : 1f;

            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = .0f;
            gameObject.AddComponent<GraphicRaycaster>();

            if (EventSystem.current == null)
            {
                GameObject eventSystem = new GameObject("Mia & Max - UI Events");
                eventSystem.transform.SetParent(transform, false);
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            safeRoot = Element("Phone safe area", transform, Vector2.zero, Vector2.one,
                Vector2.zero, Vector2.zero);
            UpdateSafeArea();

            header = Panel("Header - ivory glass", safeRoot,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(-34, 176), new Vector2(0, -108), Cream);
            locationTitle = Label("Location title", header, "MIA & MAX", 46, Ink, FontStyle.Bold,
                TextAnchor.MiddleLeft, new Vector2(0, 1), new Vector2(.69f, 1),
                new Vector2(-10, 65), new Vector2(31, -39));
            Label("Adventure tagline", header, "LITTLE MOMENTS, BIG ADVENTURES",
                22, Muted, FontStyle.Normal, TextAnchor.MiddleLeft,
                new Vector2(0, 1), new Vector2(.69f, 1),
                new Vector2(-10, 34), new Vector2(31, -81));
            resourceText = Label("Inventory counters", header, "", 28, Ink, FontStyle.Bold,
                TextAnchor.MiddleLeft, new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(-62, 51), new Vector2(0, 36));
            // Keep the approved character reference portraits visible in the modern HUD.
            RawImage portraitMia = AddPortrait("Mia", header, "MiaMax/Art/MiaPortrait", -315f);
            RawImage portraitMax = AddPortrait("Max", header, "MiaMax/Art/MaxPortrait", -238f);
            backButton = Press("Return to yard", header, "Back", new Vector2(1, 1),
                new Vector2(1, 1), new Vector2(145, 66),
                new Vector2(-100, -44), Forest, Color.white, game.LeaveGarden);

            soundButton = Press("Sound on or off", header, "Sound", new Vector2(1, 1),
                new Vector2(1, 1), new Vector2(145, 57),
                new Vector2(-100, -117), new Color(.81f,.88f,.76f),
                Ink, ToggleAudio);
            soundLabel = soundButton.GetComponentInChildren<Text>();

            quest = Panel("Story instruction - warm paper", safeRoot,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(-34, 146), new Vector2(0, -278), Paper);
            Label("Story section", quest, "THE LITTLE BIRDHOUSE", 23, Forest, FontStyle.Bold,
                TextAnchor.MiddleLeft, new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(-58, 38), new Vector2(0, -30));
            questText = Label("Current story objective", quest, "", 35, Ink, FontStyle.Normal,
                TextAnchor.MiddleLeft, Vector2.zero, Vector2.one,
                new Vector2(-70, -59), new Vector2(0, -13));

            hint = Panel("Tap hint", safeRoot, new Vector2(0, 0),
                new Vector2(1, 0), new Vector2(-34, 103),
                new Vector2(0, 86), Cream);
            hintText = Label("Tap instruction", hint, "", 30, Ink, FontStyle.Normal,
                TextAnchor.MiddleCenter, Vector2.zero, Vector2.one,
                new Vector2(-58, -10), Vector2.zero);

            card = Panel("Interaction choice panel", safeRoot,
                new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(-34, 320), new Vector2(0, 188), Cream);
            cardTitle = Label("Selected object", card, "", 47, Ink, FontStyle.Bold,
                TextAnchor.MiddleLeft, new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(-66, 67), new Vector2(0, -51));
            cardBody = Label("Action description", card, "", 33, Muted, FontStyle.Normal,
                TextAnchor.MiddleLeft, new Vector2(0, .32f), new Vector2(1, .78f),
                new Vector2(-72, -14), Vector2.zero);
            actionButton = Press("Do action", card, "Explore", new Vector2(0, 0),
                new Vector2(.69f, 0), new Vector2(-42, 91),
                new Vector2(22, 65), Forest, Color.white, game.ExecuteSelectedAction);
            actionLabel = actionButton.GetComponentInChildren<Text>();
            Press("Close action", card, "Close", new Vector2(.69f, 0),
                new Vector2(1, 0), new Vector2(-36, 91),
                new Vector2(-22, 65), new Color(.80f,.84f,.74f), Ink,
                game.CloseSelection);

            toastText = Label("Action feedback toast", safeRoot, "", 27, Forest,
                FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0, 0),
                new Vector2(1, 0), new Vector2(-90, 114), new Vector2(0, 427));

            BuildMiniGame();
            Refresh(true);
        }

        private RawImage AddPortrait(string name, Transform parent, string resource, float x)
        {
            Texture2D tex = Resources.Load<Texture2D>(resource);
            if (tex == null) return null;
            RectTransform rt = Element(name + " - original character portrait",
                parent, Vector2.one, Vector2.one, new Vector2(66, 66), new Vector2(x, -45f));
            RawImage image = rt.gameObject.AddComponent<RawImage>();
            image.texture = tex;
            image.color = Color.white;
            image.raycastTarget = false;
            AspectRatioFitter ratio = rt.gameObject.AddComponent<AspectRatioFitter>();
            ratio.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            ratio.aspectRatio = 1f;
            return image;
        }

        private void BuildMiniGame()
        {
            miniGame = Element("Treasure sorting minigame", safeRoot,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image shade = miniGame.gameObject.AddComponent<Image>();
            shade.color = new Color(.08f, .15f, .13f, .65f);
            shade.raycastTarget = true;
            dialog = Panel("Treasure sort card", miniGame,
                new Vector2(.5f,.5f), new Vector2(.5f,.5f),
                new Vector2(928, 760), Vector2.zero, Cream);
            dialogTitle = Label("Mini-game title",dialog,"MAX'S TREASURE SORT",
                43,Ink,FontStyle.Bold,TextAnchor.MiddleCenter,
                new Vector2(0,1),new Vector2(1,1),
                new Vector2(-60,88),new Vector2(0,-75));
            miniGameMessage = Label("Mini-game steps",dialog,
                "Tap the objects in the right order.",30,Muted,FontStyle.Normal,
                TextAnchor.MiddleCenter,new Vector2(0,1),new Vector2(1,1),
                new Vector2(-80,115),new Vector2(0,-169));
            string[] labels = { "Shell", "Leaf", "Feather" };
            for (int i = 0; i < labels.Length; i++)
            {
                int selection = i;
                Press("Treasure button " + labels[i],dialog,labels[i],
                    new Vector2(0,.5f),new Vector2(1,.5f),
                    new Vector2(-130,96),new Vector2(0,60-i*122f),
                    i==0?Gold:(i==1?ForestLight:Coral),Color.white,
                    () => game.ChooseTreasure(selection));
            }
            Press("Leave treasure sort",dialog,"Leave mini-game",
                new Vector2(0,0),new Vector2(1,0),
                new Vector2(-145,80),new Vector2(0,73),
                new Color(.81f,.85f,.78f),Ink,game.LeaveTreasureGame);
            miniGame.gameObject.SetActive(false);
        }

        private void ToggleAudio()
        {
            muted=!muted;
            AudioListener.volume=muted?0f:1f;
            PlayerPrefs.SetInt("MiaMax.UI.SoundMuted",muted?1:0);
            PlayerPrefs.Save();
            Refresh(true);
        }

        private void UpdateSafeArea()
        {
            if(safeRoot==null)return;
            Rect safe=Screen.safeArea;
            if(lastSafe==safe && lastSafe.width>0) return;
            safeRoot.anchorMin=new Vector2(safe.xMin/Screen.width,safe.yMin/Screen.height);
            safeRoot.anchorMax=new Vector2(safe.xMax/Screen.width,safe.yMax/Screen.height);
            safeRoot.offsetMin=Vector2.zero;
            safeRoot.offsetMax=Vector2.zero;
            lastSafe=safe;
        }

        private void Update()
        {
            UpdateSafeArea();
            if(Time.unscaledTime<nextRefresh)return;
            nextRefresh=Time.unscaledTime+.18f;
            Refresh(false);
        }

        private void Refresh(bool force)
        {
            if(game==null||resourceText==null)return;
            resourceText.text="Stars " + game.Stars + "    Twigs " + game.Twigs
                + "    Feather " + game.Feathers + "    Flowers " + game.Flowers;
            questText.text=game.ObjectiveText;
            locationTitle.text=game.IsGarden?"SECRET GARDEN":"MIA & MAX";
            backButton.gameObject.SetActive(game.IsGarden);
            soundLabel.text=muted?"Sound off":"Sound on";
            toastText.text=game.ToastText;
            toastText.rectTransform.anchoredPosition=new Vector2(0,
                game.SelectedId.Length>0?439f:209f);
            bool hasSelection=!string.IsNullOrEmpty(game.SelectedId);
            hint.gameObject.SetActive(!hasSelection);
            card.gameObject.SetActive(hasSelection);
            hintText.text=game.IsGarden?
                "Find the golden stone and tap it.":"Tap something in the garden to explore.";
            if(hasSelection)
            {
                cardTitle.text=game.SelectedTitle;
                cardBody.text=game.SelectedDescription;
                actionLabel.text=game.SelectedActionText;
            }
            miniGame.gameObject.SetActive(game.IsMiniGameOpen);
            if(game.IsMiniGameOpen)miniGameMessage.text=game.MiniGameMessage;
        }

        private void OnDestroy()
        {
            // Shared font/rounded card sprite intentionally remain cached while the application runs.
        }
    }
}
