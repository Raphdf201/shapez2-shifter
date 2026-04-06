using System;
using System.Collections.Generic;
using System.Linq;
using Game.Content.Features.Trains.Predictions;
using Game.Core.Rails;
using Game.Core.Trains;
using MonoMod.RuntimeDetour;
using ShapezShifter.Hijack.Predictions;
using ShapezShifter.SharpDetour;

namespace ShapezShifter.Hijack
{
    internal class PredictionSystemsInterceptor : IDisposable
    {
        private readonly IRewirerProvider RewirerProvider;
        private readonly Hook CreateSimulationSystemsHook;

        internal PredictionSystemsInterceptor(IRewirerProvider rewirerProvider)
        {
            RewirerProvider = rewirerProvider;

            CreateSimulationSystemsHook =
                DetourHelper
                   .CreatePostfixHook<BuiltinPredictionSimulationSystems, ShapeOperations, IPredictedTrainRegistry,
                        IReadOnlyRailColorRegistry, TrainWagonCargoTypeId, TrainWagonCargoTypeId,
                        IEnumerable<ISimulationSystem>>(
                        original: (
                            builtinSimulationSystems,
                            ops,
                            trainRegistry,
                            railColorRegistry,
                            shapeCargoType,
                            fluidCargoType) => builtinSimulationSystems.CreateSimulationSystems(
                            ops,
                            trainRegistry,
                            railColorRegistry,
                            shapeCargoType,
                            fluidCargoType),
                        postfix: CreateSimulationSystems);
        }

        private IEnumerable<ISimulationSystem> CreateSimulationSystems(
            BuiltinPredictionSimulationSystems builtinSimulationSystems,
            ShapeOperations shapeOperations,
            IPredictedTrainRegistry predictedTrainRegistry,
            IReadOnlyRailColorRegistry railColorRegistry,
            TrainWagonCargoTypeId shapeCargoType,
            TrainWagonCargoTypeId fluidCargoType,
            IEnumerable<ISimulationSystem> systems)
        {
            var systemsList = systems.ToList();
            var simulationSystemsRewirers = RewirerProvider.RewirersOfType<IPredictionSystemsRewirer>();
            PredictionSystemsDependencies dependencies = new(builtinSimulationSystems);
            foreach (IPredictionSystemsRewirer simulationSystemsRewirer in simulationSystemsRewirers)
            {
                simulationSystemsRewirer.ModifyPredictionSystems(
                    simulationSystems: systemsList,
                    dependencies: dependencies);
            }

            return systemsList;
        }

        public void Dispose()
        {
            CreateSimulationSystemsHook.Dispose();
        }
    }
}
