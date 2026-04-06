namespace ShapezShifter.Flow.Atomic
{
    public interface ISideUpgradeSelector
    {
        public ResearchSideUpgrade Select(ScenarioId scenarioId, ResearchProgression progression);
    }

    public class CustomSideUpgradeSelector : ISideUpgradeSelector
    {
        private readonly IPresentableUnlockableSideUpgradeBuilder SideUpgrade;

        public CustomSideUpgradeSelector(IPresentableUnlockableSideUpgradeBuilder researchSideUpgradeBuilder)
        {
            SideUpgrade = researchSideUpgradeBuilder;
        }

        public ResearchSideUpgrade Select(ScenarioId scenarioId, ResearchProgression progression)
        {
            return SideUpgrade.Build(scenarioId: scenarioId, progression: progression);
        }
    }
}
