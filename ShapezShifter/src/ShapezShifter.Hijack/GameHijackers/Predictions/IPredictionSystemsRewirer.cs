using System.Collections.Generic;

namespace ShapezShifter.Hijack.Predictions
{
    public interface IPredictionSystemsRewirer : IRewirer
    {
        public void ModifyPredictionSystems(
            ICollection<ISimulationSystem> simulationSystems,
            PredictionSystemsDependencies dependencies);
    }
}
