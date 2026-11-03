using MiaMax.Domain.Progression;
using MiaMax.Infrastructure.Save;
using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>Reveals the annex only after its story-area unlock has been claimed.</summary>
    public sealed class FlowerGardenZonePrototype : MonoBehaviour
    {
        private GameObject gardenContent;
        private bool revealed;

        public void Configure(GameObject contentRoot)
        {
            gardenContent = contentRoot;
            Refresh();
        }

        private void Update()
        {
            if (!revealed) Refresh();
        }

        private void Refresh()
        {
            if (gardenContent == null || GameSessionBootstrap.Current == null) return;
            QuestService quests = new QuestService(GameSessionBootstrap.Current);
            revealed = quests.IsAreaUnlocked("flower_garden");
            gardenContent.SetActive(revealed);
        }
    }
}
