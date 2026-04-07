using Core.Localization;
using Game.Core.Research;

namespace ShapezShifter.Flow.Research
{
    public class SideUpgradePresentationData
    {
        public readonly ResearchUpgradeId Id;
        public readonly GameImageId PreviewImageId;
        public readonly GameVideoId VideoId;
        public readonly IText Title;
        public readonly IText Description;
        public readonly bool Hidden;
        public readonly string Category;

        public SideUpgradePresentationData(
            ResearchUpgradeId id,
            GameImageId previewImageId,
            GameVideoId videoId,
            IText title,
            IText description,
            bool hidden,
            string category)
        {
            Id = id;
            PreviewImageId = previewImageId;
            VideoId = videoId;
            Title = title;
            Description = description;
            Hidden = hidden;
            Category = category;
        }
    }
}
