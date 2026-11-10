using UnityEngine;
using UnityEngine.UI;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Small, asset-free uGUI building blocks shared by mobile screens.
    /// These helpers provide consistent hierarchy and accessible empty/loading feedback;
    /// they do not own gameplay state or invent progress.
    /// </summary>
    public static class MobileUiComponents
    {
        public static RectTransform CreateCard(Transform parent, string name, Color background,
            float padding = 18f, float spacing = 10f)
        {
            GameObject card = new GameObject(name, typeof(RectTransform), typeof(Image),
                typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            card.transform.SetParent(parent, false);

            Image image = card.GetComponent<Image>();
            image.color = background;
            Sprite panelSprite = Resources.Load<Sprite>("MiaMax/UI/rounded_panel");
            if (panelSprite != null)
            {
                image.sprite = panelSprite;
                image.type = Image.Type.Sliced;
            }
            image.raycastTarget = false;

            VerticalLayoutGroup layout = card.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(padding), Mathf.RoundToInt(padding),
                Mathf.RoundToInt(padding), Mathf.RoundToInt(padding));
            layout.spacing = spacing;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            ContentSizeFitter fitter = card.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return (RectTransform)card.transform;
        }

        public static Text CreateLabel(Transform parent, string name, string value,
            int fontSize, Color color, TextAnchor alignment = TextAnchor.MiddleLeft)
        {
            GameObject labelObject = new GameObject(name, typeof(RectTransform), typeof(Text), typeof(LayoutElement));
            labelObject.transform.SetParent(parent, false);
            Text label = labelObject.GetComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            label.text = value ?? string.Empty;
            label.fontSize = Mathf.Max(14, fontSize);
            label.color = color;
            label.alignment = alignment;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;
            labelObject.GetComponent<LayoutElement>().minHeight = Mathf.Max(28f, fontSize * 1.35f);
            return label;
        }

        public static RectTransform CreateStateMessage(Transform parent, string name,
            string heading, string description, bool isLoading)
        {
            Color surface = isLoading ? MobileUiTheme.DeepPanel : MobileUiTheme.SoftPanel;
            Color foreground = isLoading ? MobileUiTheme.Cream : MobileUiTheme.Ink;
            RectTransform card = CreateCard(parent, name, surface);
            CreateLabel(card, "State Heading", heading, 22, foreground);
            CreateLabel(card, "State Description", description, 16, foreground);
            return card;
        }

        public static void ApplyButtonTypography(Button button, int minimumFontSize = 14, int shrinkBy = 5)
        {
            if (button == null) return;
            Text label = button.GetComponentInChildren<Text>(true);
            if (label == null) return;
            int maximum = Mathf.Max(minimumFontSize, label.fontSize);
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = Mathf.Max(minimumFontSize, maximum - Mathf.Max(0, shrinkBy));
            label.resizeTextMaxSize = maximum;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
        }

        public static void SetVisible(CanvasGroup group, bool visible, bool blockInput = true)
        {
            if (group == null) return;
            group.alpha = visible ? 1f : 0f;
            group.interactable = visible;
            group.blocksRaycasts = visible && blockInput;
        }
    }
}
