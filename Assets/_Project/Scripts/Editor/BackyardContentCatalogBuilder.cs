#if UNITY_EDITOR
using MiaMax.Domain;
using MiaMax.Domain.Farming;
using MiaMax.Domain.Progression;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Creates a reviewable first-release catalog from IDs and reward values already
    /// defined by the current domain services. Existing authored entries are never overwritten.
    /// </summary>
    public static class BackyardContentCatalogBuilder
    {
        private const string CatalogPath = "Assets/_Project/ScriptableObjects/BackyardContentCatalog.asset";

        [MenuItem("Tools/Mia & Max/Create or Update Starter Content Catalog")]
        public static void CreateOrUpdate()
        {
            BackyardContentCatalog catalog = AssetDatabase.LoadAssetAtPath<BackyardContentCatalog>(CatalogPath);
            bool created = catalog == null;
            if (created)
            {
                catalog = ScriptableObject.CreateInstance<BackyardContentCatalog>();
                AssetDatabase.CreateAsset(catalog, CatalogPath);
            }

            int added = 0;
            added += Add(catalog, new BackyardContentEntry
            {
                Id = FarmingService.Carrot, Kind = BackyardContentKind.Crop, DisplayName = "Carrot",
                AreaId = "backyard", RequiredItemId = "seed_carrot", RequiredQuantity = 1,
                RewardItemId = FarmingService.Carrot, RewardQuantity = 1
            });
            added += Add(catalog, new BackyardContentEntry
            {
                Id = FarmingService.Tomato, Kind = BackyardContentKind.Crop, DisplayName = "Tomato",
                AreaId = "backyard", RequiredItemId = "seed_tomato", RequiredQuantity = 1,
                RewardItemId = FarmingService.Tomato, RewardQuantity = 1
            });
            added += Add(catalog, new BackyardContentEntry
            {
                Id = FarmingService.Strawberry, Kind = BackyardContentKind.Crop, DisplayName = "Strawberry",
                AreaId = "backyard", RequiredItemId = "seed_strawberry", RequiredQuantity = 1,
                RewardItemId = FarmingService.Strawberry, RewardQuantity = 1
            });

            AddRecipe(catalog, ProductionService.CarrotSoup, "Carrot Soup", "crop_carrot", 3,
                "product_carrot_soup", ref added);
            AddRecipe(catalog, ProductionService.StrawberryJuice, "Strawberry Juice", "crop_strawberry", 3,
                "product_strawberry_juice", ref added);
            AddRecipe(catalog, ProductionService.WoodenToy, "Wooden Toy", "wood", 2,
                "product_wooden_toy", ref added);

            AddOrder(catalog, "order_soup_01", "Soup Delivery", "product_carrot_soup", 12, 10, ref added);
            AddOrder(catalog, "order_juice_01", "Juice Delivery", "product_strawberry_juice", 16, 12, ref added);
            AddOrder(catalog, "order_toy_01", "Toy Delivery", "product_wooden_toy", 14, 11, ref added);

            added += Add(catalog, new BackyardContentEntry
            {
                Id = "quest_first_steps", Kind = BackyardContentKind.StoryGoal, DisplayName = "A New Adventure",
                AreaId = "backyard", RewardCoins = 10, RewardExperience = 15
            });
            added += Add(catalog, new BackyardContentEntry
            {
                Id = "quest_first_order", Kind = BackyardContentKind.StoryGoal, DisplayName = "Helping the Neighbours",
                AreaId = "backyard", RewardCoins = 15, RewardExperience = 20
            });
            added += Add(catalog, new BackyardContentEntry
            {
                Id = "quest_order_master", Kind = BackyardContentKind.StoryGoal, DisplayName = "A Busy Little Backyard",
                AreaId = "backyard", UnlockAreaId = "flower_garden", RewardCoins = 30, RewardExperience = 35
            });
            added += Add(catalog, new BackyardContentEntry
            {
                Id = "quest_flower_friend", Kind = BackyardContentKind.StoryGoal, DisplayName = "A Path Beyond the Garden",
                AreaId = "flower_garden", UnlockAreaId = "woodland_trail", RewardCoins = 40, RewardExperience = 45
            });

            AddMiniGame(catalog, "match3", "Garden Match", ref added);
            AddMiniGame(catalog, "fishing", "Pond Fishing", ref added);
            AddMiniGame(catalog, "max_run", "Max Run", ref added);

            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var issues = catalog.Validate();
            string status = issues.Count == 0 ? "Catalog structure is valid." :
                "Catalog has " + issues.Count + " validation issue(s).";
            Debug.Log("[Mia & Max] Starter content catalog " +
                (created ? "created" : "updated") + " at " + CatalogPath +
                ". Added " + added + " new entries; total: " + catalog.Entries.Count +
                ". " + status);
            for (int i = 0; i < issues.Count; i++)
                Debug.LogWarning("[Mia & Max Content Catalog] " + issues[i], catalog);

            EditorUtility.DisplayDialog("Mia & Max — Starter Content Catalog",
                (created ? "Created" : "Updated") + " catalog.\n\nNew entries: " + added +
                "\nTotal entries: " + catalog.Entries.Count + "\n\n" + status +
                "\n\nReview and tune all content in the Inspector before release.", "OK");
            Selection.activeObject = catalog;
        }

        private static int AddRecipe(BackyardContentCatalog catalog, string id, string title,
            string input, int quantity, string output, ref int added)
        {
            int result = Add(catalog, new BackyardContentEntry
            {
                Id = id, Kind = BackyardContentKind.Recipe, DisplayName = title,
                AreaId = "backyard", RequiredItemId = input, RequiredQuantity = quantity,
                RewardItemId = output, RewardQuantity = 1, RewardExperience = 5
            });
            added += result;
            return result;
        }

        private static int AddOrder(BackyardContentCatalog catalog, string id, string title,
            string requiredItem, int coins, int experience, ref int added)
        {
            int result = Add(catalog, new BackyardContentEntry
            {
                Id = id, Kind = BackyardContentKind.Order, DisplayName = title,
                AreaId = "backyard", RequiredItemId = requiredItem, RequiredQuantity = 1,
                RewardCoins = coins, RewardExperience = experience
            });
            added += result;
            return result;
        }

        private static int AddMiniGame(BackyardContentCatalog catalog, string id, string title, ref int added)
        {
            int result = Add(catalog, new BackyardContentEntry
            {
                Id = id, Kind = BackyardContentKind.MiniGame, DisplayName = title,
                AreaId = "backyard"
            });
            added += result;
            return result;
        }

        private static int Add(BackyardContentCatalog catalog, BackyardContentEntry entry)
        {
            if (catalog.TryGet(entry.Id, out _)) return 0;
            return catalog.TryAdd(entry) ? 1 : 0;
        }
    }
}
#endif
