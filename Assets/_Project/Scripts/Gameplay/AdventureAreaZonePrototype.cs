using MiaMax.Domain.Progression;
using MiaMax.Infrastructure.Save;
using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>Hides an annex until its corresponding story area has been unlocked.</summary>
    public sealed class AdventureAreaZonePrototype : MonoBehaviour
    {
        private GameObject contentRoot;
        private string areaId;
        private bool revealed;

        public void Configure(string unlockAreaId, GameObject content)
        {
            areaId = unlockAreaId;
            contentRoot = content;
            if (contentRoot != null) contentRoot.SetActive(false);
            Refresh();
        }

        private void Update()
        {
            if (!revealed) Refresh();
        }

        private void Refresh()
        {
            if (contentRoot == null || GameSessionBootstrap.Current == null) return;
            revealed = new QuestService(GameSessionBootstrap.Current).IsAreaUnlocked(areaId);
            contentRoot.SetActive(revealed);
        }
    }
}
