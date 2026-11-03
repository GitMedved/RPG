using System.Text;
using MiaMax.Domain;
using MiaMax.Domain.Collections;
using MiaMax.Domain.Progression;
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
        private bool journalOpen;
        private GameSession boundSession;
        private QuestService questService;
        private CollectionService collectionService;
        private ProductionService productionService;
        private OrderBoardService orderService;
        private TutorialService tutorialService;
        private GUIStyle panelStyle;
        private GUIStyle headingStyle;
        private GUIStyle bodyStyle;
        private GUIStyle buttonStyle;
        private string journalNotice = string.Empty;

        private void OnGUI()
        {
            GameSession session = GameSessionBootstrap.Current;
            if (session == null || session.Data == null) return;

            BindSession(session);

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

            float buttonWidth = 126f * scale;
            if (GUI.Button(new Rect(0f, line * 2.35f, buttonWidth, line * 1.2f),
                inventoryOpen ? "Close Bag" : "Inventory", buttonStyle))
            {
                inventoryOpen = !inventoryOpen;
                if (inventoryOpen) journalOpen = false;
            }
            if (GUI.Button(new Rect(buttonWidth + 8f * scale, line * 2.35f, buttonWidth, line * 1.2f),
                journalOpen ? "Close Journal" : "Journal", buttonStyle))
            {
                journalOpen = !journalOpen;
                if (journalOpen) inventoryOpen = false;
            }

            if (inventoryOpen)
                DrawInventory(session, scale, line, panelWidth);
            if (journalOpen)
                DrawJournal(scale, line, panelWidth);

            if (tutorialService != null && !tutorialService.IsCompleted)
            {
                float hintHeight = 38f * scale;
                GUI.color = new Color(0.10f, 0.15f, 0.18f, 0.92f);
                GUI.Box(new Rect(0f, safe.height - hintHeight, safe.width - margin * 2f, hintHeight), GUIContent.none, panelStyle);
                GUI.color = previousColor;
                GUI.Label(new Rect(10f * scale, safe.height - hintHeight + 3f * scale,
                    safe.width - margin * 2f - 20f * scale, hintHeight - 6f * scale), tutorialService.CurrentHint, bodyStyle);
            }

            GUI.EndGroup();
        }

        private void BindSession(GameSession session)
        {
            if (boundSession == session) return;
            boundSession = session;
            questService = new QuestService(session);
            collectionService = new CollectionService(session);
            productionService = new ProductionService(session);
            orderService = new OrderBoardService(session);
            tutorialService = new TutorialService(session);
        }

        private void DrawJournal(float scale, float line, float width)
        {
            float y = line * 3.8f;
            float row = 31f * scale;
            float height = row * 11.25f;
            Color previousColor = GUI.color;
            GUI.color = new Color(0.10f, 0.15f, 0.18f, 0.96f);
            GUI.Box(new Rect(0f, y, width, height), GUIContent.none, panelStyle);
            GUI.color = previousColor;

            GUI.Label(new Rect(10f * scale, y + 3f * scale, width - 20f * scale, row),
                "ADVENTURE JOURNAL", headingStyle);
            GUI.Label(new Rect(12f * scale, y + row, width - 24f * scale, row * 1.45f),
                questService != null ? questService.GetSummary() : "Story quests unavailable.", bodyStyle);

            float questButtonY = y + row * 2.45f;
            if (GUI.Button(new Rect(10f * scale, questButtonY, width - 20f * scale, row * 0.9f),
                "Claim next quest reward", buttonStyle))
            {
                string claimedTitle;
                journalNotice = questService != null && questService.TryClaimNext(out claimedTitle)
                    ? "Quest reward claimed: " + claimedTitle + "!"
                    : "No quest reward is ready yet.";
            }

            GUI.Label(new Rect(12f * scale, y + row * 3.45f, width - 24f * scale, row * 1.25f),
                collectionService != null ? collectionService.GetSummary() : "Collection unavailable.", bodyStyle);
            if (GUI.Button(new Rect(10f * scale, y + row * 4.65f, width - 20f * scale, row * 0.9f),
                "Claim collection reward", buttonStyle))
            {
                journalNotice = collectionService == null
                    ? "Collection unavailable."
                    : collectionService.IsCompletionRewardClaimed
                        ? "Collection reward was already claimed."
                        : collectionService.TryClaimCompletionReward()
                            ? "Collection reward claimed: +25 coins, +30 XP!"
                            : "Find all 5 keepsakes to unlock this reward.";
            }

            var job = productionService != null ? productionService.ActiveJob : null;
            string productionStatus = job == null || !job.IsActive
                ? "Workshop: choose a recipe below."
                : productionService.RemainingSeconds > 0
                    ? "Workshop: " + RecipeName(job.RecipeId) + " — " + productionService.RemainingSeconds + "s left."
                    : "Workshop: " + RecipeName(job.RecipeId) + " is ready to collect!";
            GUI.Label(new Rect(12f * scale, y + row * 5.65f, width - 24f * scale, row), productionStatus, bodyStyle);

            float recipeY = y + row * 6.55f;
            float recipeGap = 4f * scale;
            float recipeWidth = (width - 20f * scale - recipeGap * 2f) / 3f;
            if (GUI.Button(new Rect(10f * scale, recipeY, recipeWidth, row), "Soup x3", buttonStyle))
                journalNotice = StartRecipe(ProductionService.CarrotSoup);
            if (GUI.Button(new Rect(10f * scale + recipeWidth + recipeGap, recipeY, recipeWidth, row), "Juice x3", buttonStyle))
                journalNotice = StartRecipe(ProductionService.StrawberryJuice);
            if (GUI.Button(new Rect(10f * scale + (recipeWidth + recipeGap) * 2f, recipeY, recipeWidth, row), "Toy x2", buttonStyle))
                journalNotice = StartRecipe(ProductionService.WoodenToy);

            if (GUI.Button(new Rect(10f * scale, y + row * 7.55f, width - 20f * scale, row * 0.9f),
                "Collect finished product", buttonStyle))
                journalNotice = productionService != null && productionService.TryCollect()
                    ? "Product collected and +5 XP earned!"
                    : "No finished product is ready yet.";

            GUI.Label(new Rect(12f * scale, y + row * 8.55f, width - 24f * scale, row),
                orderService != null ? orderService.GetStatusSummary() : "Orders unavailable.", bodyStyle);
            if (GUI.Button(new Rect(10f * scale, y + row * 9.45f, width - 20f * scale, row * 0.9f),
                "Deliver next ready order", buttonStyle))
            {
                string orderId = orderService != null ? orderService.FirstAvailableOrder() : null;
                journalNotice = orderId != null && orderService.TryComplete(orderId)
                    ? "Order delivered! Coins and XP received."
                    : "No order is ready. Make a product first.";
            }

            if (!string.IsNullOrEmpty(journalNotice))
                GUI.Label(new Rect(12f * scale, y + row * 10.4f, width - 24f * scale, row * 0.8f),
                    journalNotice, bodyStyle);
        }

        private string StartRecipe(string recipeId)
        {
            if (productionService == null) return "Workshop unavailable.";
            var activeJob = productionService.ActiveJob;
            if (activeJob != null && activeJob.IsActive)
                return "Workshop busy. Collect the current product first.";
            if (productionService.TryStart(recipeId)) return "Started: " + RecipeName(recipeId) + ".";
            return "Not enough ingredients or workshop is busy.";
        }

        private static string RecipeName(string recipeId)
        {
            switch (recipeId)
            {
                case ProductionService.CarrotSoup: return "Carrot soup";
                case ProductionService.StrawberryJuice: return "Strawberry juice";
                case ProductionService.WoodenToy: return "Wooden toy";
                default: return "Unknown recipe";
            }
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
            bodyStyle.wordWrap = true;
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