using Core.Logging;
using Game.Content.Features.Trains.Predictions;

namespace ShapezShifter.Hijack.Predictions
{
    public class PredictionSystemsDependencies
    {
        public readonly GameMode Mode;
        public readonly IGameResourcesMap ResourcesMap;
        public readonly IShapeRegistry ShapeRegistry;
        public readonly IShapeIdManager ShapeIdManager;
        public readonly IResearchUnlockManager ResearchUnlockManager;
        public readonly ILogger Logger;
        public readonly ITrainHashCalculatorHeuristic TrainHashHeuristic;

        public PredictionSystemsDependencies(BuiltinPredictionSimulationSystems builtinSimulationSystems)
        {
            Mode = builtinSimulationSystems.Mode;
            ResourcesMap = builtinSimulationSystems.ResourcesMap;
            ShapeRegistry = builtinSimulationSystems.ShapeRegistry;
            ShapeIdManager = builtinSimulationSystems.ShapeIdManager;
            ResearchUnlockManager = builtinSimulationSystems.ResearchUnlockManager;
            Logger = builtinSimulationSystems.Logger;
            TrainHashHeuristic = builtinSimulationSystems.TrainHashHeuristic;
        }
    }
}
