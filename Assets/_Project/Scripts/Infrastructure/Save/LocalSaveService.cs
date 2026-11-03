using System;
using System.IO;
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
            if (data != null) return Validate(data);

            data = TryRead(backup);
            if (data != null)
            {
                data = Validate(data);
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

            if (data.Progress == null) data.Progress = new MiaMax.Domain.Progression.PlayerProgressData();
            data.Progress.Level = Math.Max(1, data.Progress.Level);
            data.Progress.Experience = Math.Max(0, data.Progress.Experience);
            data.Progress.Coins = Math.Max(0, data.Progress.Coins);
            if (data.Inventory == null) data.Inventory = new System.Collections.Generic.List<MiaMax.Domain.Inventory.ItemStackData>();
            if (data.FarmPlots == null) data.FarmPlots = new System.Collections.Generic.List<MiaMax.Domain.Farming.FarmPlotData>();
            if (data.ProductionJob == null) data.ProductionJob = new MiaMax.Domain.Production.ProductionJobData();
            if (data.CompletedOrders == null) data.CompletedOrders = new System.Collections.Generic.List<MiaMax.Domain.Orders.CompletedOrderData>();
            if (data.ClaimedQuests == null) data.ClaimedQuests = new System.Collections.Generic.List<MiaMax.Domain.Progression.QuestProgressData>();
            if (data.UnlockedWorldAreas == null) data.UnlockedWorldAreas = new System.Collections.Generic.List<string>();

            for (int i = data.Inventory.Count - 1; i >= 0; i--)
            {
                var stack = data.Inventory[i];
                if (stack == null || string.IsNullOrWhiteSpace(stack.ItemId) || stack.Quantity <= 0)
                    data.Inventory.RemoveAt(i);
            }

            for (int i = data.ClaimedQuests.Count - 1; i >= 0; i--)
            {
                var quest = data.ClaimedQuests[i];
                if (quest == null || string.IsNullOrWhiteSpace(quest.QuestId))
                    data.ClaimedQuests.RemoveAt(i);
            }

            for (int i = data.UnlockedWorldAreas.Count - 1; i >= 0; i--)
                if (string.IsNullOrWhiteSpace(data.UnlockedWorldAreas[i]))
                    data.UnlockedWorldAreas.RemoveAt(i);

            long now = UtcNow();
            if (data.CreatedAtUnix <= 0 || data.CreatedAtUnix > now) data.CreatedAtUnix = now;
            if (data.UpdatedAtUnix <= 0 || data.UpdatedAtUnix > now) data.UpdatedAtUnix = now;
            return data;
        }

        private static long UtcNow()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}
