using MiaMax.Domain;
using MiaMax.Domain.Collections;
using MiaMax.Domain.Progression;
using MiaMax.Infrastructure.Save;
using MiaMax.Infrastructure.Settings;
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
        private bool settingsOpen;
        private bool mapOpen;
        private Vector2 journalScroll;
        private GameSession boundSession;
        private QuestService questService;
        private DailyTaskService dailyTaskService;
        private DailyVisitService dailyVisitService;
        private AchievementService achievementService;
        private CollectionService collectionService;
        private ProductionService productionService;
        private OrderBoardService orderService;
        private TutorialService tutorialService;
        private UpgradeService upgradeService;
        private GameSettingsService settingsService;
        private PlayerInteraction playerInteraction;
        private GUIStyle panelStyle;
        private GUIStyle headingStyle;
        private GUIStyle bodyStyle;
        private GUIStyle buttonStyle;
        private string journalNotice = string.Empty;
        private string interactionFeedback = string.Empty;
        private float interactionFeedbackUntil;

        private void OnEnable()
        {
            BackyardInteractable.InteractionFeedback += HandleInteractionFeedback;
        }

        private void OnDisable()
        {
            BackyardInteractable.InteractionFeedback -= HandleInteractionFeedback;
        }

        private void HandleInteractionFeedback(string message)
        {
            settingsService?.Vibrate();
            interactionFeedback = message;
            interactionFeedbackUntil = Time.unscaledTime + 2.6f;
        }

        private void OnGUI()
        {
            GameSession session = GameSessionBootstrap.Current;
            if (session == null || session.Data == null) return;

            BindSession(session);

            Rect safe = Screen.safeArea;
            if (GetComponent<MobileHudCanvas>() != null)
            {
                DrawLegacyPanelsOnly(session, safe);
                return;
            }

            float top = Screen.height - safe.yMax;
            float scale = Mathf.Clamp(Mathf.Min(Screen.width / 900f, Screen.height / 600f), 0.72f, 1.25f) * session.Data.Settings.UiScale;
            float margin = 12f * scale;
            float panelWidth = Mathf.Min(270f * scale, safe.width - margin * 2f);
            float line = 30f * scale;

            EnsureStyles(scale);
            GUI.BeginGroup(new Rect(safe.xMin + margin, top + margin, safe.width - margin * 2f, safe.height - margin * 2f));
            Color previousColor = GUI.color;
            GUI.color = new Color(0.10f, 0.15f, 0.18f, 0.88f);
            GUI.Box(new Rect(0f, 0f, panelWidth, line * 4.1f), GUIContent.none, panelStyle);
            GUI.color = previousColor;
            GUI.Label(new Rect(10f * scale, 3f * scale, panelWidth - 20f * scale, line), 
                "LEVEL " + session.Data.Progress.Level + "    " + session.Data.Progress.Experience + " XP", headingStyle);
            GUI.Label(new Rect(10f * scale, line, panelWidth - 20f * scale, line),
                "Coins: " + session.Data.Progress.Coins, bodyStyle);
            GUI.Label(new Rect(10f * scale, line * 2f, panelWidth - 20f * scale, line),
                "Visit streak: " + (dailyVisitService != null ? dailyVisitService.CurrentStreak : 0) + " day(s)", bodyStyle);
            GUI.enabled = dailyVisitService != null && dailyVisitService.CanClaimToday;
            GUI.color = new Color(0.72f, 0.48f, 0.20f, 1f);
            if (GUI.Button(new Rect(8f * scale, line * 3f, panelWidth - 16f * scale, line * 0.9f),
                dailyVisitService != null ? dailyVisitService.GetButtonLabel() : "DAILY GIFT", buttonStyle))
            {
                if (dailyVisitService.TryClaimReward())
                {
                    interactionFeedback = "Daily gift claimed: +" + dailyVisitService.CoinsToday +
                        " coins, +" + dailyVisitService.ExperienceToday + " XP!";
                    interactionFeedbackUntil = Time.unscaledTime + 3f;
                }
            }
            GUI.enabled = true;
            GUI.color = previousColor;

            // Compact top-right journal shortcut.
            float journalButtonWidth = Mathf.Min(190f * scale, safe.width - margin * 2f);
            float journalButtonX = safe.width - margin * 2f - journalButtonWidth;
            GUI.color = new Color(0.16f, 0.28f, 0.27f, 0.98f);
            if (GUI.Button(new Rect(journalButtonX, 0f, journalButtonWidth, line * 1.35f),
                journalOpen ? "×  CLOSE JOURNAL" : "☰  ADVENTURE JOURNAL", buttonStyle))
            {
                journalOpen = !journalOpen;
                if (journalOpen) { inventoryOpen = false; settingsOpen = false; mapOpen = false; }
            }
            GUI.color = previousColor;
            GUI.color = new Color(0.22f, 0.30f, 0.34f, 0.98f);
            if (GUI.Button(new Rect(journalButtonX, line * 1.5f, journalButtonWidth, line), settingsOpen ? "CLOSE SETTINGS" : "SETTINGS"))
            {
                settingsOpen = !settingsOpen;
                if (settingsOpen) { inventoryOpen = false; journalOpen = false; mapOpen = false; }
            }
            GUI.color = previousColor;
            GUI.color = new Color(0.20f, 0.34f, 0.30f, 0.98f);
            if (GUI.Button(new Rect(journalButtonX, line * 2.7f, journalButtonWidth, line),
                mapOpen ? "CLOSE WORLD MAP" : "WORLD MAP"))
            {
                mapOpen = !mapOpen;
                if (mapOpen) { inventoryOpen = false; journalOpen = false; settingsOpen = false; }
            }
            GUI.color = previousColor;

            // Bottom action dock: bag on the left, contextual interaction on the right.
            float dockY = safe.height - line * 2.0f - margin;
            float dockHeight = line * 1.65f;
            GUI.color = new Color(0.08f, 0.13f, 0.16f, 0.88f);
            GUI.Box(new Rect(safe.width * 0.5f - 160f * scale, dockY, 320f * scale, dockHeight), GUIContent.none, panelStyle);
            GUI.color = previousColor;

            float dockButtonWidth = 142f * scale;
            float dockButtonY = dockY + 4f * scale;
            GUI.color = inventoryOpen
                ? new Color(0.32f, 0.49f, 0.36f, 1f)
                : new Color(0.18f, 0.29f, 0.32f, 1f);
            if (GUI.Button(new Rect(safe.width * 0.5f - 154f * scale, dockButtonY, dockButtonWidth, line * 1.35f),
                inventoryOpen ? "×  CLOSE BAG" : "▣  BACKPACK", buttonStyle))
            {
                inventoryOpen = !inventoryOpen;
                if (inventoryOpen) { journalOpen = false; settingsOpen = false; mapOpen = false; }
            }

            if (playerInteraction == null)
                playerInteraction = FindFirstObjectByType<PlayerInteraction>();
            bool canInteract = playerInteraction != null && playerInteraction.HasNearbyTarget;
            if (canInteract)
            {
                GUI.color = new Color(0.95f, 0.91f, 0.78f, 1f);
                GUI.Label(new Rect(safe.width * 0.5f - 150f * scale, dockY - line * 0.9f,
                    300f * scale, line * 0.8f), playerInteraction.NearbyPrompt, headingStyle);
            }
            GUI.color = canInteract
                ? new Color(0.72f, 0.48f, 0.20f, 1f)
                : new Color(0.22f, 0.25f, 0.27f, 0.82f);
            GUI.enabled = canInteract;
            if (GUI.Button(new Rect(safe.width * 0.5f + 12f * scale, dockButtonY, dockButtonWidth, line * 1.35f),
                canInteract ? "✦  INTERACT" : "✦  EXPLORE", buttonStyle) && canInteract)
                playerInteraction.TryInteract();
            GUI.enabled = true;
            GUI.color = previousColor;

            if (inventoryOpen)
                DrawInventory(session, scale, line, panelWidth);
            if (settingsOpen) DrawSettings(scale, line, panelWidth, safe.height);
            if (mapOpen)
            {
                float mapPanelX = safe.width - margin * 2f - panelWidth;
                GUI.BeginGroup(new Rect(mapPanelX, 0f, panelWidth, safe.height - margin * 2f));
                DrawWorldMap(scale, line, panelWidth, safe.height - margin * 2f);
                GUI.EndGroup();
            }
            if (journalOpen)
            {
                float journalPanelX = safe.width - margin * 2f - panelWidth;
                GUI.BeginGroup(new Rect(journalPanelX, 0f, panelWidth, safe.height - margin * 2f));
                DrawJournal(scale, line, panelWidth, safe.height - margin * 2f);
                GUI.EndGroup();
            }

            if (!string.IsNullOrEmpty(interactionFeedback) && Time.unscaledTime < interactionFeedbackUntil)
            {
                float toastWidth = Mathf.Min(420f * scale, safe.width - margin * 2f);
                float toastHeight = line * 1.25f;
                float toastX = (safe.width - toastWidth) * 0.5f;
                float toastY = line * 2.8f;
                GUI.color = new Color(0.12f, 0.20f, 0.18f, 0.94f);
                GUI.Box(new Rect(toastX, toastY, toastWidth, toastHeight), GUIContent.none, panelStyle);
                GUI.color = new Color(1f, 0.96f, 0.82f, 1f);
                GUI.Label(new Rect(toastX + 10f * scale, toastY + 2f * scale,
                    toastWidth - 20f * scale, toastHeight - 4f * scale), interactionFeedback, bodyStyle);
                GUI.color = previousColor;
            }

            if (tutorialService != null && !tutorialService.IsCompleted)
            {
                float hintHeight = 38f * scale;
                float hintY = dockY - hintHeight - 8f * scale;
                GUI.color = new Color(0.10f, 0.15f, 0.18f, 0.92f);
                GUI.Box(new Rect(0f, hintY, safe.width - margin * 2f, hintHeight), GUIContent.none, panelStyle);
                GUI.color = previousColor;
                GUI.Label(new Rect(10f * scale, hintY + 3f * scale,
                    safe.width - margin * 2f - 20f * scale, hintHeight - 6f * scale), tutorialService.CurrentHint, bodyStyle);
            }

            GUI.EndGroup();
        }

        public string SettingsSummary
        {
            get
            {
                if (settingsService == null) return "Loading settings...";
                var d = settingsService.Data;
                return "Audio levels\\nMaster: " + Mathf.RoundToInt(d.MasterVolume * 100f) + "%" +
                    "\\nEffects: " + Mathf.RoundToInt(d.EffectsVolume * 100f) + "%" +
                    "\\nMusic: " + Mathf.RoundToInt(d.MusicVolume * 100f) + "%" +
                    "\\nVoice: " + Mathf.RoundToInt(d.VoiceVolume * 100f) + "%" +
                    "\\n\\nInterface\\nUI scale: " + Mathf.RoundToInt(d.UiScale * 100f) + "%" +
                    "\\nVibration: " + (d.VibrationEnabled ? "ON" : "OFF") +
                    "\\nReduced motion: " + (d.ReducedMotion ? "ON" : "OFF") +
                    "\\n\\nChanges save automatically. Language: English.";
            }
        }
        public bool VibrationEnabled => settingsService != null && settingsService.Data.VibrationEnabled;
        public bool ReducedMotionEnabled => settingsService != null && settingsService.Data.ReducedMotion;
        public void AdjustSettingFromCanvas(int setting, float delta) { if (settingsService != null) AdjustSetting(setting, delta); }
        public void ToggleVibrationFromCanvas() { settingsService?.ToggleVibration(); }
        public void ToggleReducedMotionFromCanvas() { settingsService?.ToggleReducedMotion(); }

        public string FeedbackText => Time.unscaledTime < interactionFeedbackUntil ? interactionFeedback : string.Empty;

        public string InventorySummary
        {
            get
            {
                GameSession s = GameSessionBootstrap.Current;
                if (s == null || s.Data == null) return "Loading backpack...";
                if (s.Data.Inventory == null || s.Data.Inventory.Count == 0)
                    return "Your backpack is empty.\\n\\nExplore the backyard to find seeds, crops, flowers and keepsakes.";
                System.Text.StringBuilder b = new System.Text.StringBuilder();
                b.AppendLine("BACKPACK  ·  " + s.Data.Inventory.Count + " STACKS");
                b.AppendLine();
                for (int i = 0; i < s.Data.Inventory.Count; i++)
                {
                    var stack = s.Data.Inventory[i];
                    if (stack == null) continue;
                    b.AppendLine(FriendlyName(stack.ItemId) + "     x" + stack.Quantity);
                }
                return b.ToString();
            }
        }
        public string WorldMapSummary
        {
            get
            {
                GameSession s = GameSessionBootstrap.Current;
                if (s == null || s.Data == null) return "Loading world map...";
                bool garden = questService != null && questService.IsAreaUnlocked("flower_garden");
                bool trail = questService != null && questService.IsAreaUnlocked("woodland_trail");
                int flowers = s.Data.CollectedGardenResources != null ? s.Data.CollectedGardenResources.Count : 0;
                int trailItems = s.Data.CollectedTrailResources != null ? s.Data.CollectedTrailResources.Count : 0;
                return "BACKYARD  ·  HOME BASE\\nFarming, workshop, arcade and keepsakes\\n\\n" +
                    "↓  FLOWER GARDEN  ·  " + (garden ? "UNLOCKED" : "LOCKED") +
                    "\\nDiscoveries: " + flowers + "/3" +
                    (garden ? "\\nFind all flowers to open the woodland path." : "\\nClaim the Busy Little Backyard story reward to unlock.") +
                    "\\n\\n↓  WOODLAND TRAIL  ·  " + (trail ? "UNLOCKED" : "LOCKED") +
                    "\\nDiscoveries: " + trailItems + "/3" +
                    (trail ? "\\nFind the acorn, mushroom and wild berries." : "\\nCollect all 3 garden flowers and claim the next story reward.") +
                    "\\n\\nNew areas appear here as your story progresses.";
            }
        }

        public string StatusText
        {
            get
            {
                GameSession s = GameSessionBootstrap.Current;
                if (s == null || s.Data == null) return "Preparing your backyard...";
                int streak = dailyVisitService != null ? dailyVisitService.CurrentStreak : 0;
                return "LEVEL " + s.Data.Progress.Level + "   ·   " + s.Data.Progress.Experience +
                    " XP\n" + s.Data.Progress.Coins + " COINS   ·   " + streak + " DAY STREAK";
            }
        }
        public string DailyGiftLabel => dailyVisitService == null ? "DAILY GIFT" : dailyVisitService.GetButtonLabel();
        public bool CanClaimDailyGift => dailyVisitService != null && dailyVisitService.CanClaimToday;
        public string InteractionPrompt => playerInteraction != null && playerInteraction.HasNearbyTarget
            ? playerInteraction.NearbyPrompt : "Explore the backyard";
        public bool CanInteract => playerInteraction != null && playerInteraction.HasNearbyTarget;
        public string TutorialHint => tutorialService != null && !tutorialService.IsCompleted
            ? tutorialService.CurrentHint : string.Empty;
        public void TryInteractFromHud() { playerInteraction?.TryInteract(); }

        public bool InventoryOpen => inventoryOpen;
        public bool JournalOpen => journalOpen;
        public bool SettingsOpen => settingsOpen;
        public bool MapOpen => mapOpen;

        public void CloseCanvasDetails()
        {
            inventoryOpen = false;
            mapOpen = false;
        }

        public void ToggleInventory()
        {
            inventoryOpen = !inventoryOpen;
            if (inventoryOpen) { journalOpen = false; settingsOpen = false; mapOpen = false; }
        }

        public void ToggleJournal()
        {
            journalOpen = !journalOpen;
            if (journalOpen) { inventoryOpen = false; settingsOpen = false; mapOpen = false; }
        }

        public void ToggleSettings()
        {
            settingsOpen = !settingsOpen;
            if (settingsOpen) { inventoryOpen = false; journalOpen = false; mapOpen = false; }
        }

        public void ToggleMap()
        {
            mapOpen = !mapOpen;
            if (mapOpen) { inventoryOpen = false; journalOpen = false; settingsOpen = false; }
        }

        public void ClaimDailyGift()
        {
            if (dailyVisitService != null && dailyVisitService.TryClaimReward())
            {
                interactionFeedback = "Daily gift claimed: +" + dailyVisitService.CoinsToday +
                    " coins, +" + dailyVisitService.ExperienceToday + " XP!";
                interactionFeedbackUntil = Time.unscaledTime + 3f;
            }
        }

        private void DrawLegacyPanelsOnly(GameSession session, Rect safe)
        {
            float scale = Mathf.Clamp(Mathf.Min(Screen.width / 900f, Screen.height / 600f), 0.72f, 1.25f) *
                          session.Data.Settings.UiScale;
            float margin = 12f * scale;
            float line = 30f * scale;
            float panelWidth = Mathf.Min(300f * scale, safe.width - margin * 2f);
            EnsureStyles(scale);
            GUI.BeginGroup(new Rect(safe.xMin + margin, Screen.height - safe.yMax + margin,
                safe.width - margin * 2f, safe.height - margin * 2f));
            if (journalOpen)
            {
                GUI.BeginGroup(new Rect(safe.width - margin * 2f - panelWidth, 0f,
                    panelWidth, safe.height - margin * 2f));
                DrawJournal(scale, line, panelWidth, safe.height - margin * 2f);
                GUI.EndGroup();
            }
            GUI.EndGroup();
        }

        private void BindSession(GameSession session)
        {
            if (boundSession == session) return;
            boundSession = session;
            questService = new QuestService(session);
            dailyTaskService = new DailyTaskService(session);
            dailyVisitService = new DailyVisitService(session);
            achievementService = new AchievementService(session);
            collectionService = new CollectionService(session);
            productionService = new ProductionService(session);
            orderService = new OrderBoardService(session);
            tutorialService = new TutorialService(session);
            upgradeService = new UpgradeService(session);
            settingsService = new GameSettingsService(session);
            playerInteraction = FindFirstObjectByType<PlayerInteraction>();
        }

        private void DrawJournal(float scale, float line, float width, float availableHeight)
        {
            float row = 31f * scale;
            float height = row * 26.5f;
            float viewportY = line * 3.8f;
            float viewportHeight = Mathf.Max(row * 5f, availableHeight - viewportY);
            journalScroll = GUI.BeginScrollView(
                new Rect(0f, viewportY, width, viewportHeight),
                journalScroll,
                new Rect(0f, 0f, width, height),
                false,
                true);
            float y = 0f;
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

            GUI.Label(new Rect(12f * scale, y + row * 10.35f, width - 24f * scale, row),
                "BACKYARD UPGRADES", headingStyle);
            string gardenStatus = upgradeService.GardenAtMax
                ? "Garden Lv." + upgradeService.GardenLevel + " — max"
                : "Garden Lv." + upgradeService.GardenLevel + " — harvest x" + upgradeService.HarvestYield + ", " + upgradeService.GardenUpgradeCost + " coins";
            string workshopStatus = upgradeService.WorkshopAtMax
                ? "Workshop Lv." + upgradeService.WorkshopLevel + " — max"
                : "Workshop Lv." + upgradeService.WorkshopLevel + " — " + (upgradeService.WorkshopLevel * 15) + "% faster, " + upgradeService.WorkshopUpgradeCost + " coins";
            GUI.Label(new Rect(12f * scale, y + row * 11.15f, width - 24f * scale, row * 0.9f), gardenStatus, bodyStyle);
            GUI.Label(new Rect(12f * scale, y + row * 12.05f, width - 24f * scale, row * 0.9f), workshopStatus, bodyStyle);

            float upgradeGap = 5f * scale;
            float upgradeWidth = (width - 20f * scale - upgradeGap) / 2f;
            if (GUI.Button(new Rect(10f * scale, y + row * 13.0f, upgradeWidth, row * 0.9f),
                upgradeService.GardenAtMax ? "Garden MAX" : "Upgrade Garden", buttonStyle))
                journalNotice = upgradeService.TryUpgradeGarden()
                    ? "Garden upgraded! Future harvests give more crops."
                    : "Need more coins or garden is max level.";
            if (GUI.Button(new Rect(10f * scale + upgradeWidth + upgradeGap, y + row * 13.0f, upgradeWidth, row * 0.9f),
                upgradeService.WorkshopAtMax ? "Workshop MAX" : "Upgrade Workshop", buttonStyle))
                journalNotice = upgradeService.TryUpgradeWorkshop()
                    ? "Workshop upgraded! New crafts finish faster."
                    : "Need more coins or workshop is max level.";

            GUI.Label(new Rect(12f * scale, y + row * 14.0f, width - 24f * scale, row * 2.15f),
                dailyTaskService != null ? dailyTaskService.GetSummary() : "Daily goals unavailable.", bodyStyle);
            if (GUI.Button(new Rect(10f * scale, y + row * 16.35f, width - 20f * scale, row * 0.9f),
                dailyTaskService != null && dailyTaskService.IsRewardClaimed ? "Daily reward claimed" : "Claim daily reward", buttonStyle))
            {
                journalNotice = dailyTaskService != null && dailyTaskService.TryClaimReward()
                    ? "Daily reward claimed: +20 coins, +15 XP!"
                    : "Complete all daily goals before claiming.";
            }

            GUI.Label(new Rect(12f * scale, y + row * 17.3f, width - 24f * scale, row),
                achievementService != null ? achievementService.GetSummary() : "Achievements unavailable.", bodyStyle);
            GUI.Label(new Rect(12f * scale, y + row * 18.15f, width - 24f * scale, row * 4.0f),
                achievementService != null ? achievementService.GetDetails(8) : string.Empty, bodyStyle);
            if (GUI.Button(new Rect(10f * scale, y + row * 22.3f, width - 20f * scale, row * 0.9f),
                "Claim next achievement reward", buttonStyle))
            {
                string title;
                int coins, experience;
                journalNotice = achievementService != null &&
                    achievementService.TryClaimNext(out title, out coins, out experience)
                    ? title + " claimed! +" + coins + " coins, +" + experience + " XP."
                    : "No achievement reward is ready yet.";
            }
            string trailStatus = questService != null && questService.IsAreaUnlocked("woodland_trail")
                ? "Woodland Trail discoveries: " +
                    (boundSession.Data.CollectedTrailResources != null ? boundSession.Data.CollectedTrailResources.Count : 0) + "/3"
                : "Woodland Trail: complete the Flower Garden story quest to unlock.";
            GUI.Label(new Rect(12f * scale, y + row * 23.3f, width - 24f * scale, row * 0.8f),
                trailStatus, bodyStyle);
            if (!string.IsNullOrEmpty(journalNotice))
                GUI.Label(new Rect(12f * scale, y + row * 24.3f, width - 24f * scale, row * 0.8f),
                    journalNotice, bodyStyle);
            GUI.EndScrollView();
        }

        private void DrawWorldMap(float scale, float line, float width, float availableHeight)
        {
            float row = 38f * scale;
            float y = line * 2.7f;
            float height = row * 8.6f;
            Color previousColor = GUI.color;
            GUI.color = new Color(0.10f, 0.15f, 0.18f, 0.96f);
            GUI.Box(new Rect(0f, y, width, Mathf.Min(height, availableHeight - y - 10f * scale)), GUIContent.none, panelStyle);
            GUI.color = previousColor;

            GUI.Label(new Rect(10f * scale, y + 4f * scale, width - 20f * scale, row),
                "WORLD MAP", headingStyle);
            GUI.Label(new Rect(12f * scale, y + row, width - 24f * scale, row * 1.25f),
                "BACKYARD  ·  HOME BASE\nFarming, workshop, arcade and keepsakes", bodyStyle);
            GUI.Label(new Rect(12f * scale, y + row * 2.35f, width - 24f * scale, row * 0.7f),
                "↓  Follow the garden path", bodyStyle);

            bool gardenUnlocked = questService != null && questService.IsAreaUnlocked("flower_garden");
            int gardenFound = boundSession.Data.CollectedGardenResources != null
                ? boundSession.Data.CollectedGardenResources.Count : 0;
            GUI.Label(new Rect(12f * scale, y + row * 3.0f, width - 24f * scale, row * 1.35f),
                "FLOWER GARDEN  ·  " + (gardenUnlocked ? "UNLOCKED" : "LOCKED") +
                "\nDiscoveries: " + gardenFound + "/3" +
                (gardenUnlocked ? "\nCollect all flowers to open the woodland path." :
                    "\nClaim the 'A Busy Little Backyard' story reward to unlock."), bodyStyle);

            bool trailUnlocked = questService != null && questService.IsAreaUnlocked("woodland_trail");
            int trailFound = boundSession.Data.CollectedTrailResources != null
                ? boundSession.Data.CollectedTrailResources.Count : 0;
            GUI.Label(new Rect(12f * scale, y + row * 4.75f, width - 24f * scale, row * 1.55f),
                "↓  WOODLAND TRAIL  ·  " + (trailUnlocked ? "UNLOCKED" : "LOCKED") +
                "\nDiscoveries: " + trailFound + "/3" +
                (trailUnlocked ? "\nFind the acorn, mushroom and wild berries." :
                    "\nCollect all 3 garden flowers and claim the next story reward."), bodyStyle);

            GUI.Label(new Rect(12f * scale, y + row * 6.65f, width - 24f * scale, row),
                "New areas appear here as your story progresses.", bodyStyle);
        }

        private void DrawSettings(float scale, float line, float width, float availableHeight)
        {
            float y = line * 2.7f;
            float row = 34f * scale;
            float height = row * 9.2f;
            Color previousColor = GUI.color;
            GUI.color = new Color(0.10f, 0.15f, 0.18f, 0.96f);
            GUI.Box(new Rect(0f, y, width, Mathf.Min(height, availableHeight - y - 12f * scale)), GUIContent.none, panelStyle);
            GUI.color = previousColor;
            GUI.Label(new Rect(10f * scale, y + 3f * scale, width - 20f * scale, row), "SETTINGS", headingStyle);
            DrawSettingRow("Master", settingsService.Data.MasterVolume, 0, y + row, scale, row, width);
            DrawSettingRow("Effects", settingsService.Data.EffectsVolume, 1, y + row * 2f, scale, row, width);
            DrawSettingRow("Music", settingsService.Data.MusicVolume, 2, y + row * 3f, scale, row, width);
            DrawSettingRow("Voice", settingsService.Data.VoiceVolume, 3, y + row * 4f, scale, row, width);
            DrawSettingRow("UI size", (settingsService.Data.UiScale - 0.85f) / 0.30f, 4, y + row * 5f, scale, row, width);
            if (GUI.Button(new Rect(10f * scale, y + row * 6.1f, width - 20f * scale, row * 0.9f), "Vibration: " + (settingsService.Data.VibrationEnabled ? "ON" : "OFF"), buttonStyle)) settingsService.ToggleVibration();
            if (GUI.Button(new Rect(10f * scale, y + row * 7.1f, width - 20f * scale, row * 0.9f), "Reduced motion: " + (settingsService.Data.ReducedMotion ? "ON" : "OFF"), buttonStyle)) settingsService.ToggleReducedMotion();
            GUI.Label(new Rect(12f * scale, y + row * 8.15f, width - 24f * scale, row), "Settings save automatically. English is the current language.", bodyStyle);
        }

        private void DrawSettingRow(string label, float value, int setting, float y, float scale, float row, float width)
        {
            float buttonWidth = 34f * scale;
            string valueLabel = setting == 4 ? Mathf.RoundToInt(85f + value * 30f) + "%" : Mathf.RoundToInt(value * 100f) + "%";
            GUI.Label(new Rect(12f * scale, y, width * 0.55f, row), label + "  " + valueLabel, bodyStyle);
            if (GUI.Button(new Rect(width - 86f * scale, y + 2f * scale, buttonWidth, row - 4f * scale), "−", buttonStyle)) AdjustSetting(setting, -0.05f);
            if (GUI.Button(new Rect(width - 46f * scale, y + 2f * scale, buttonWidth, row - 4f * scale), "+", buttonStyle)) AdjustSetting(setting, 0.05f);
        }

        private void AdjustSetting(int setting, float delta)
        {
            switch (setting)
            {
                case 0: settingsService.SetMaster(settingsService.Data.MasterVolume + delta); break;
                case 1: settingsService.SetEffects(settingsService.Data.EffectsVolume + delta); break;
                case 2: settingsService.SetMusic(settingsService.Data.MusicVolume + delta); break;
                case 3: settingsService.SetVoice(settingsService.Data.VoiceVolume + delta); break;
                case 4: settingsService.SetUiScale(settingsService.Data.UiScale + delta * 0.3f); break;
            }
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
            buttonStyle.fontSize = Mathf.RoundToInt(14f * scale);
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.hover.textColor = Color.white;
            buttonStyle.active.textColor = Color.white;
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
                case "flower_pink": return "Pink flower";
                case "flower_yellow": return "Yellow flower";
                case "flower_purple": return "Purple flower";
                case "acorn": return "Forest acorn";
                case "wild_mushroom": return "Wild mushroom";
                case "wild_berries": return "Wild berries";
                default: return id.Replace("_", " ");
            }
        }
    }
}