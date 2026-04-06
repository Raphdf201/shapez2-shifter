using System.Collections.Generic;

namespace ShapezShifter.Flow.Research
{
    public interface IMilestoneSelector
    {
        public ResearchLevel Select(ScenarioId scenarioId, IReadOnlyList<ResearchLevel> milestones);
    }
}
