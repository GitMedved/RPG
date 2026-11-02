using System;
namespace MiaMax.Domain.Farming
{
    [Serializable]
    public sealed class FarmPlotData
    {
        public string PlotId;
        public string CropId;
        public long PlantedAtUnix;
        public bool IsPlanted => !string.IsNullOrEmpty(CropId) && PlantedAtUnix > 0;
    }
}