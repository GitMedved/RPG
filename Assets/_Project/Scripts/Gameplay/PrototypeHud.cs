using System.Text;
using MiaMax.Domain;
using MiaMax.Infrastructure.Save;
using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Temporary safe-area-aware HUD and inventory viewer.
    /// Uses IMGUI to keep the prototype free of external UI dependencies.
    /// </summary>
    public sealed class PrototypeHud : MonoBehaviour
    {
        private bool inventoryOpen;
        private GUIStyle panelStyle;
        private GUIStyle headingStyle;
        private GUIStyle bodyStyle;
        private GUIStyle buttonStyle;

        private void OnGUI()
        {
            GameSession session = GameSessionBootstrap.Current;
            if (session == null || session.Data == null) return;

            Rect safe = Screen.safeArea;
            float top = Screen.height - safe.yMax;
            float scale = Mathf.Clamp(Screen.width / 900f, 0.72f, 1.25f);
            float margin = 12f * scale;
            float panelWidth = Mathf.Min(270f * scale, safe.width - margin * 2f);
            float line = 30f * scale;

            EnsureStyles(scale);
            GUI.BeginGroup(new Rect(safe.xMin + margin, top + margin, safe.width - margin * 2f, safe.height - margin * 2f));
            Color previousColor = GUI.color;
            GUI.color = new Color(0.10f, 0.15f, 0.18f, 0.88f);
            GUI.Box(new Rect(0f, 0f, panelWidth, line * 2.1f), GUIContent.none, panelStyle);
            GUI.color = previousColor;
            GUI.Label(new Rect(10f * scale, 3f * scale, panelWidth - 20f * scale, line), 
                "LEVEL " + session.Data.Progress.Level + "    " + session.Data.Progress.Experience + " XP", headingStyle);
            GUI.Label(new Rect(10f * scale, line, panelWidth - 20f * scale, line),
                "Coins: " + session.Data.Progress.Coins, bodyStyle);

            float buttonWidth = 150f * scale;
            if (GUI.Button(new Rect(0f, line * 2.35f, buttonWidth, line * 1.2f),
                inventoryOpen ? "Close Bag" : "Inventory", buttonStyle))
                inventoryOpen = !inventoryOpen;

            if (inventoryOpen)
                DrawInventory(session, scale, line, panelWidth);

            GUI.EndGroup();
        }

        private void DrawInventory(GameSession session, float scale, float line, float width)
        {
            float y = line * 3.8f;
            float row = 27f * scale;
            int visible = Mathf.Min(session.Data.Inventory.Count, 12);
            float height = (visible + 1) * row + 16f * scale;
            Color previousColor = GUI.color;
            GUI.color = new Color(0.10f, 0.15f, 0.18f, 0.92f);
            GUI.Box(new Rect(0f, y, width, height), GUIContent.none, panelStyle);
            GUI.color = previousColor;
            GUI.Label(new Rect(10f * scale, y + 4f * scale, width - 20f * scale, row),
                "BACKPACK  (" + session.Data.Inventory.Count + " stacks)", headingStyle);

            for (int i = 0; i < visible; i++)
            {
                var stack = session.Data.Inventory[i];
                if (stack == null) continue;
                GUI.Label(new Rect(12f * scale, y + row * (i + 1), width - 24f * scale, row),
                    FriendlyName(stack.ItemId) + "   x" + stack.Quantity, bodyStyle);
            }

            if (session.Data.Inventory.Count == 0)
                GUI.Label(new Rect(12f * scale, y + row, width - 24f * scale, row),
                    "Your backpack is empty.", bodyStyle);
            else if (session.Data.Inventory.Count > visible)
                GUI.Label(new Rect(12f * scale, y + row * (visible + 1), width - 24f * scale, row),
                    "More items are stored.", bodyStyle);
        }

        private void EnsureStyles(float scale)
        {
            if (panelStyle != null) return;
            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.normal.background = Texture2D.whiteTexture;
            panelStyle.normal.textColor = Color.white;
            panelStyle.fontSize = Mathf.RoundToInt(15f * scale);
            panelStyle.alignment = TextAnchor.MiddleLeft;
            headingStyle = new GUIStyle(GUI.skin.label);
            headingStyle.fontSize = Mathf.RoundToInt(16f * scale);
            headingStyle.fontStyle = FontStyle.Bold;
            headingStyle.normal.textColor = Color.white;
            bodyStyle = new GUIStyle(GUI.skin.label);
            bodyStyle.fontSize = Mathf.RoundToInt(14f * scale);
            bodyStyle.normal.textColor = Color.white;
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = Mathf.RoundToInt(15f * scale);
        }

        private static string FriendlyName(string id)
        {
            switch (id)
            {
                case "seed_carrot": return "Carrot seeds";
                case "seed_tomato": return "Tomato seeds";
                case "seed_strawberry": return "Strawberry seeds";
                case "crop_carrot": return "Carrots";
                case "crop_tomato": return "Tomatoes";
                case "crop_strawberry": return "Strawberries";
                case "product_carrot_soup": return "Carrot soup";
                case "product_strawberry_juice": return "Strawberry juice";
                case "product_wooden_toy": return "Wooden toy";
                case "wood": return "Wood";
                default: return id.Replace("_", " ");
            }
        }
    }
}