using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core.Research;

namespace ShapezShifter.Flow.Research
{
    public class ByIdPerScenarioMilestoneSelector : IMilestoneSelector
    {
        private readonly Func<ScenarioId, ResearchUpgradeId> MilestoneIdPerScenario;

        public ByIdPerScenarioMilestoneSelector(Func<ScenarioId, ResearchUpgradeId> milestoneIdPerScenario)
        {
            MilestoneIdPerScenario = milestoneIdPerScenario;
        }

        public ResearchLevel Select(ScenarioId scenarioId, IReadOnlyList<ResearchLevel> milestones)
        {
            ResearchUpgradeId id = MilestoneIdPerScenario.Invoke(scenarioId);
            try
            {
                ResearchLevel milestone = milestones.SingleOrDefault(x => x.Id == id);
                return milestone
                       ?? throw new Exception($"Could not find a milestone with id {id} for scenario {scenarioId}");
            }
            catch (InvalidOperationException)
            {
                throw new Exception($"More than one element match milestone id {id} for scenario {scenarioId}");
            }
        }
    }
}
