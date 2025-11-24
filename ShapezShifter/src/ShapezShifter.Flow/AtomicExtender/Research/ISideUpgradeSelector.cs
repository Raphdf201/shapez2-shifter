using System.Collections.Generic;

namespace ShapezShifter.Flow.Atomic
{
    public interface ISideUpgradeSelector
    {
        public ResearchSideUpgrade Select(string scenarioId, ResearchProgression progression);
    }

    public class CustomSideUpgradeSelector : ISideUpgradeSelector
    {
        private readonly IPresentableUnlockableSideUpgradeBuilder SideUpgrade;

        public CustomSideUpgradeSelector(IPresentableUnlockableSideUpgradeBuilder researchSideUpgradeBuilder)
        {
            SideUpgrade = researchSideUpgradeBuilder;
        }

        public ResearchSideUpgrade Select(string scenarioId, ResearchProgression progression)
        {
            return SideUpgrade.Build(scenarioId, progression);
        }
    }
}