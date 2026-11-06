using UnityEngine;
using UnityEngine.UI;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Shared visual tokens and interaction states for the runtime mobile Canvas UI.
    /// Keep this palette intentionally asset-free until approved production UI art is available.
    /// </summary>
    public static class MobileUiTheme
    {
        public static readonly Color Ink = new Color(0.16f, 0.20f, 0.18f, 1f);
        public static readonly Color Cream = new Color(0.98f, 0.94f, 0.82f, 1f);
        public static readonly Color Forest = new Color(0.22f, 0.36f, 0.29f, 0.98f);
        public static readonly Color Leaf = new Color(0.29f, 0.43f, 0.27f, 1f);
        public static readonly Color Honey = new Color(0.82f, 0.57f, 0.25f, 1f);
        public static readonly Color DeepPanel = new Color(0.12f, 0.19f, 0.17f, 0.98f);
        public static readonly Color SoftPanel = new Color(0.94f, 0.88f, 0.72f, 0.98f);
        public static readonly Color Muted = new Color(0.43f, 0.46f, 0.39f, 0.9f);

        public const int MinimumButtonFontSize = 14;
        public const int ButtonFontShrink = 5;

        public static void ApplyButton(Button button, Image image, Color baseColor)
        {
            if (button == null || image == null) return;

            image.color = baseColor;
            button.targetGraphic = image;
            button.transition = Selectable.Transition.ColorTint;

            ColorBlock colors = button.colors;
            colors.normalColor = baseColor;
            colors.highlightedColor = Color.Lerp(baseColor, Color.white, 0.12f);
            colors.pressedColor = Color.Lerp(baseColor, Ink, 0.18f);
            colors.selectedColor = Color.Lerp(baseColor, Color.white, 0.08f);
            colors.disabledColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0.55f);
            button.colors = colors;
            MobileUiComponents.ApplyButtonTypography(button, MinimumButtonFontSize, ButtonFontShrink);
        }

        public static Color GetReadableButtonText(Color background)
        {
            return ContrastRatio(Cream, background) >= ContrastRatio(Ink, background) ? Cream : Ink;
        }

        public static bool HasAccessibleButtonTextContrast(Color background, float minimumRatio = 4.5f)
        {
            return Mathf.Max(ContrastRatio(Cream, background), ContrastRatio(Ink, background)) >= minimumRatio;
        }

        private static float ContrastRatio(Color first, Color second)
        {
            float a = RelativeLuminance(first);
            float b = RelativeLuminance(second);
            float lighter = Mathf.Max(a, b);
            float darker = Mathf.Min(a, b);
            return (lighter + 0.05f) / (darker + 0.05f);
        }

        private static float RelativeLuminance(Color color)
        {
            float red = LinearChannel(color.r);
            float green = LinearChannel(color.g);
            float blue = LinearChannel(color.b);
            return 0.2126f * red + 0.7152f * green + 0.0722f * blue;
        }

        private static float LinearChannel(float channel)
        {
            return channel <= 0.04045f
                ? channel / 12.92f
                : Mathf.Pow((channel + 0.055f) / 1.055f, 2.4f);
        }

        public static void ApplyAvailability(Button button, bool available, Color availableColor)
        {
            if (button == null) return;
            button.interactable = available;
            if (button.targetGraphic != null)
                button.targetGraphic.color = available ? availableColor : Muted;
        }
    }
}
