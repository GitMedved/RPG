using System;
using System.IO;
using MiaMax.Domain.Progression;
using UnityEngine;

namespace MiaMax.Infrastructure.Save
{
    /// <summary>
    /// Small offline-first JSON save store. Writes a temporary file before replacing
    /// the primary save and retains one known-good backup.
    /// </summary>
    public sealed class LocalSaveService
    {
        private const string SaveFileName = "mia_max_save.json";
        private const string TempFileName = "mia_max_save.tmp";
        private const string BackupFileName = "mia_max_save.backup";

        private readonly string directory;

        public LocalSaveService(string directoryOverride = null)
        {
            directory = string.IsNullOrWhiteSpace(directoryOverride)
                ? Application.persistentDataPath
                : directoryOverride;
        }

        public PlayerSaveData LoadOrCreate()
        {
            Directory.CreateDirectory(directory);
            string primary = Path.Combine(directory, SaveFileName);
            string backup = Path.Combine(directory, BackupFileName);

            PlayerSaveData data = TryRead(primary);
            if (data != null)
            {
                data = Validate(data);
                // A valid primary always wins; an orphaned temp file is from an interrupted
                // older write and must not replace a committed save.
                DeleteIfExists(Path.Combine(directory, TempFileName));
                return data;
            }

            // Save writes a complete temp file before rotating the primary. If the app was
            // interrupted after the rotation but before the final move, the primary is absent
            // and the temp file may be the newest complete copy.
            if (!File.Exists(primary))
            {
                PlayerSaveData interrupted = TryRead(Path.Combine(directory, TempFileName));
                if (interrupted != null)
                {
                    interrupted = Validate(interrupted);
                    PlayerSaveData knownGoodBackup = TryRead(backup);
                    if (knownGoodBackup == null ||
                        interrupted.UpdatedAtUnix >= knownGoodBackup.UpdatedAtUnix)
                    {
                        File.Copy(Path.Combine(directory, TempFileName), primary, true);
                        DeleteIfExists(Path.Combine(directory, TempFileName));
                        Debug.LogWarning("Recovered the most recent complete save from the temporary file.");
                        return interrupted;
                    }
                }
            }

            data = TryRead(backup);
            if (data != null)
            {
                data = Validate(data);
                // Do not let Save rotate a corrupt primary file over the known-good backup.
                if (File.Exists(primary)) File.Delete(primary);
                Save(data);
                Debug.LogWarning("Primary save was unavailable; restored the backup.");
                return data;
            }

            return new PlayerSaveData { CreatedAtUnix = UtcNow(), UpdatedAtUnix = UtcNow() };
        }

        public void Save(PlayerSaveData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            data = Validate(data);
            data.UpdatedAtUnix = UtcNow();

            Directory.CreateDirectory(directory);
            string primary = Path.Combine(directory, SaveFileName);
            string temp = Path.Combine(directory, TempFileName);
            string backup = Path.Combine(directory, BackupFileName);
            File.WriteAllText(temp, JsonUtility.ToJson(data, true));

            if (File.Exists(primary))
            {
                if (File.Exists(backup)) File.Delete(backup);
                File.Copy(primary, backup);
                File.Delete(primary);
            }

            File.Move(temp, primary);
        }

        private static void DeleteIfExists(string path)
        {
            if (File.Exists(path)) File.Delete(path);
        }

        private static PlayerSaveData TryRead(string path)
        {
            if (!File.Exists(path)) return null;
            try
            {
                PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(File.ReadAllText(path));
                if (data == null) return null;
                if (data.SchemaVersion != PlayerSaveData.CurrentSchemaVersion)
                    throw new InvalidDataException("Unsupported save schema version; source file was preserved.");
                return data;
            }
            catch (InvalidDataException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not read save file: " + exception.Message);
                return null;
            }
        }

        private static PlayerSaveData Validate(PlayerSaveData data)
        {
            if (data.SchemaVersion != PlayerSaveData.CurrentSchemaVersion)
                throw new InvalidDataException("Unsupported save schema version.");

            data.GardenLevel = Math.Max(0, Math.Min(2, data.GardenLevel));
            data.WorkshopLevel = Math.Max(0, Math.Min(2, data.WorkshopLevel));
            if (data.Settings == null) data.Settings = new PlayerSettingsData();
            if (data.DailyTasks == null) data.DailyTasks = new DailyTaskSaveData();
            if (data.DailyVisit == null) data.DailyVisit = new DailyVisitSaveData();
            if (data.Achievements == null) data.Achievements = new System.Collections.Generic.List<AchievementProgressData>();
            for (int i = data.Achievements.Count - 1; i >= 0; i--)
            {
                var achievement = data.Achievements[i];
                if (achievement == null || string.IsNullOrWhiteSpace(achievement.AchievementId)) data.Achievements.RemoveAt(i);
                else achievement.Progress = Math.Max(0, Math.Min(1000, achievement.Progress));
            }
            if (data.Achievements.Count > 100) data.Achievements.RemoveRange(100, data.Achievements.Count - 100);
            data.DailyTasks.HarvestCount = Math.Max(0, Math.Min(1000, data.DailyTasks.HarvestCount));
            data.DailyTasks.CraftCount = Math.Max(0, Math.Min(1000, data.DailyTasks.CraftCount));
            data.DailyTasks.DeliverCount = Math.Max(0, Math.Min(1000, data.DailyTasks.DeliverCount));
            data.DailyTasks.ExploreCount = Math.Max(0, Math.Min(1000, data.DailyTasks.ExploreCount));
            data.DailyTasks.ArcadeRewardsClaimed = Math.Max(0, Math.Min(DailyTaskService.DailyArcadeRewardLimit, data.DailyTasks.ArcadeRewardsClaimed));
            data.DailyVisit.CurrentStreak = Math.Max(0, Math.Min(36500, data.DailyVisit.CurrentStreak));
            data.DailyVisit.BestStreak = Math.Max(data.DailyVisit.CurrentStreak, Math.Max(0, Math.Min(36500, data.DailyVisit.BestStreak)));
            data.Settings.MasterVolume = Clamp01(data.Settings.MasterVolume);
            data.Settings.EffectsVolume = Clamp01(data.Settings.EffectsVolume);
            data.Settings.MusicVolume = Clamp01(data.Settings.MusicVolume);
            data.Settings.VoiceVolume = Clamp01(data.Settings.VoiceVolume);
            data.Settings.UiScale = Math.Max(0.85f, Math.Min(1.15f, data.Settings.UiScale));
            if (data.Progress == null) data.Progress = new MiaMax.Domain.Progression.PlayerProgressData();
            data.Progress.Level = Math.Max(1, Math.Min(100, data.Progress.Level));
            data.Progress.Experience = Math.Max(0, Math.Min(
                MiaMax.Domain.GameSession.ExperienceForNextLevel(data.Progress.Level) - 1,
                data.Progress.Experience));
            data.Progress.Coins = Math.Max(0, data.Progress.Coins);
            if (data.Inventory == null) data.Inventory = new System.Collections.Generic.List<MiaMax.Domain.Inventory.ItemStackData>();
            if (data.FarmPlots == null) data.FarmPlots = new System.Collections.Generic.List<MiaMax.Domain.Farming.FarmPlotData>();
            if (data.ProductionJob == null) data.ProductionJob = new MiaMax.Domain.Production.ProductionJobData();
            if (data.CompletedOrders == null) data.CompletedOrders = new System.Collections.Generic.List<MiaMax.Domain.Orders.CompletedOrderData>();
            if (data.ClaimedQuests == null) data.ClaimedQuests = new System.Collections.Generic.List<MiaMax.Domain.Progression.QuestProgressData>();
            if (data.UnlockedWorldAreas == null) data.UnlockedWorldAreas = new System.Collections.Generic.List<string>();
            if (data.CollectedGardenResources == null) data.CollectedGardenResources = new System.Collections.Generic.List<string>();
            if (data.CollectedTrailResources == null) data.CollectedTrailResources = new System.Collections.Generic.List<string>();
            data.CollectedTrailResources.RemoveAll(string.IsNullOrWhiteSpace);
            if (data.CollectedTrailResources.Count > 100) data.CollectedTrailResources.RemoveRange(100, data.CollectedTrailResources.Count - 100);
            data.CollectedGardenResources.RemoveAll(string.IsNullOrWhiteSpace);
            if (data.CollectedGardenResources.Count > 100) data.CollectedGardenResources.RemoveRange(100, data.CollectedGardenResources.Count - 100);
            if (data.MiniGameRewardClaims == null) data.MiniGameRewardClaims = new System.Collections.Generic.List<MiaMax.Domain.MiniGames.MiniGameRewardClaimData>();
            if (data.DiscoveredCollectibles == null) data.DiscoveredCollectibles = new System.Collections.Generic.List<string>();
            data.DiscoveredCollectibles.RemoveAll(string.IsNullOrWhiteSpace);
            if (data.DiscoveredCollectibles.Count > 100) data.DiscoveredCollectibles.RemoveRange(100, data.DiscoveredCollectibles.Count - 100);

            for (int i = data.MiniGameRewardClaims.Count - 1; i >= 0; i--)
            {
                var claim = data.MiniGameRewardClaims[i];
                if (claim == null || string.IsNullOrWhiteSpace(claim.RunId) ||
                    string.IsNullOrWhiteSpace(claim.GameId) || claim.ClaimedAtUnix <= 0)
                    data.MiniGameRewardClaims.RemoveAt(i);
            }
            if (data.MiniGameRewardClaims.Count > 500)
                data.MiniGameRewardClaims.RemoveRange(0, data.MiniGameRewardClaims.Count - 500);

            for (int i = data.Inventory.Count - 1; i >= 0; i--)
            {
                var stack = data.Inventory[i];
                if (stack == null || string.IsNullOrWhiteSpace(stack.ItemId) || stack.Quantity <= 0)
                    data.Inventory.RemoveAt(i);
            }
            // Normalize duplicate stacks so inventory operations always address one
            // canonical stack per item. Clamp on overflow rather than wrapping negative.
            for (int i = 0; i < data.Inventory.Count; i++)
            {
                for (int j = data.Inventory.Count - 1; j > i; j--)
                {
                    if (data.Inventory[i].ItemId != data.Inventory[j].ItemId) continue;
                    long combined = (long)data.Inventory[i].Quantity + data.Inventory[j].Quantity;
                    data.Inventory[i].Quantity = (int)Math.Min(int.MaxValue, combined);
                    data.Inventory.RemoveAt(j);
                }
            }

            var plotIds = new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            for (int i = data.FarmPlots.Count - 1; i >= 0; i--)
            {
                var plot = data.FarmPlots[i];
                if (plot == null || string.IsNullOrWhiteSpace(plot.PlotId) || !plotIds.Add(plot.PlotId))
                {
                    data.FarmPlots.RemoveAt(i);
                    continue;
                }
                if (string.IsNullOrWhiteSpace(plot.CropId) || plot.PlantedAtUnix <= 0)
                {
                    plot.CropId = null;
                    plot.PlantedAtUnix = 0;
                }
            }
            if (data.FarmPlots.Count > 100)
                data.FarmPlots.RemoveRange(100, data.FarmPlots.Count - 100);

            string recipeInput;
            int recipeQuantity;
            int recipeDuration;
            if (data.ProductionJob == null ||
                !data.ProductionJob.IsActive ||
                !MiaMax.Domain.ProductionService.Recipe(data.ProductionJob.RecipeId,
                    out recipeInput, out recipeQuantity, out recipeDuration) ||
                data.ProductionJob.DurationSeconds <= 0)
                data.ProductionJob = new MiaMax.Domain.Production.ProductionJobData();

            var orderIds = new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            for (int i = data.CompletedOrders.Count - 1; i >= 0; i--)
            {
                var order = data.CompletedOrders[i];
                if (order == null || string.IsNullOrWhiteSpace(order.OrderId) || !orderIds.Add(order.OrderId))
                    data.CompletedOrders.RemoveAt(i);
            }
            if (data.CompletedOrders.Count > 500)
                data.CompletedOrders.RemoveRange(0, data.CompletedOrders.Count - 500);

            var questIds = new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            for (int i = data.ClaimedQuests.Count - 1; i >= 0; i--)
            {
                var quest = data.ClaimedQuests[i];
                if (quest == null || string.IsNullOrWhiteSpace(quest.QuestId) || !questIds.Add(quest.QuestId))
                    data.ClaimedQuests.RemoveAt(i);
            }
            if (data.ClaimedQuests.Count > 100)
                data.ClaimedQuests.RemoveRange(0, data.ClaimedQuests.Count - 100);

            var areaIds = new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            for (int i = data.UnlockedWorldAreas.Count - 1; i >= 0; i--)
                if (string.IsNullOrWhiteSpace(data.UnlockedWorldAreas[i]) || !areaIds.Add(data.UnlockedWorldAreas[i]))
                    data.UnlockedWorldAreas.RemoveAt(i);

            long now = UtcNow();
            if (data.CreatedAtUnix <= 0 || data.CreatedAtUnix > now) data.CreatedAtUnix = now;
            if (data.UpdatedAtUnix <= 0 || data.UpdatedAtUnix > now) data.UpdatedAtUnix = now;
            return data;
        }

        private static float Clamp01(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) return 1f;
            return Math.Max(0f, Math.Min(1f, value));
        }

        private static long UtcNow()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}
