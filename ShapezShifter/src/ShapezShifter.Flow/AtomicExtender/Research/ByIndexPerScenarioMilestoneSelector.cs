using System;
using System.Collections.Generic;

namespace ShapezShifter.Flow.Research
{
    public class ByIndexPerScenarioMilestoneSelector : IMilestoneSelector
    {
        private readonly Func<ScenarioId, Index> IndexFunc;

        public ByIndexPerScenarioMilestoneSelector(Func<ScenarioId, Index> indexFunc)
        {
            IndexFunc = indexFunc;
        }

        public ResearchLevel Select(ScenarioId scenarioId, IReadOnlyList<ResearchLevel> milestones)
        {
            return milestones[IndexFunc.Invoke(scenarioId)];
        }
    }
}
