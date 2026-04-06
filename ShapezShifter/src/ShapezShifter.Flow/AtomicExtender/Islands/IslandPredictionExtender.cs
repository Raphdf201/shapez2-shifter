using System.Collections.Generic;
using Core.Events;
using Core.Factory;
using Core.Logging;
using Game.Content.Features.Predictions;
using Game.Core.Simulation;
using ShapezShifter.Hijack;
using ShapezShifter.Hijack.Predictions;

namespace ShapezShifter.Flow.Atomic
{
    public class IslandPredictionExtender<TSimulation> : IPredictionSystemsRewirer, IChainableRewirer
        where TSimulation : ISimulation
    {
        private readonly IslandDefinitionId DefinitionId;
        private readonly IIslandPredictionFactoryBuilder<TSimulation> IslandSimulationFactoryBuilder;
        private readonly ILogger Logger;

        public IslandPredictionExtender(
            IslandDefinitionId definitionId,
            IIslandPredictionFactoryBuilder<TSimulation> islandSimulationFactoryBuilder,
            ILogger logger)
        {
            DefinitionId = definitionId;
            IslandSimulationFactoryBuilder = islandSimulationFactoryBuilder;
            Logger = logger;
        }

        private ISimulationSystem CreateAtomicSystem(IFactory<TSimulation> factory)
        {
            return new AtomicIslandPredictionSimulationSystem<TSimulation>(
                simulationFactory: factory,
                islandDefinitionId: DefinitionId,
                logger: Logger);
        }

        public IEvent AfterHijack
        {
            get { return _AfterExtensionApplied; }
        }

        private readonly MultiRegisterEvent _AfterExtensionApplied = new();

        public void ModifyPredictionSystems(
            ICollection<ISimulationSystem> simulationSystems,
            PredictionSystemsDependencies dependencies)
        {
            var factory = IslandSimulationFactoryBuilder.BuildFactory(dependencies);
            simulationSystems.Add(CreateAtomicSystem(factory));

            _AfterExtensionApplied.Invoke();
        }
    }

    public class IslandPredictionExtender<TSimulation, TConfig> : IPredictionSystemsRewirer, IChainableRewirer<TConfig>
        where TSimulation : ISimulation
    {
        private readonly IslandDefinitionId DefinitionId;
        private readonly IIslandPredictionFactoryBuilder<TSimulation, TConfig> BuildingSimulationFactoryBuilder;
        private readonly ILogger Logger;

        public IslandPredictionExtender(
            IslandDefinitionId definitionId,
            IIslandPredictionFactoryBuilder<TSimulation, TConfig> buildingSimulationFactoryBuilder,
            ILogger logger)
        {
            DefinitionId = definitionId;
            BuildingSimulationFactoryBuilder = buildingSimulationFactoryBuilder;
            Logger = logger;
        }

        private ISimulationSystem CreateAtomicSystem(IFactory<TSimulation> factory)
        {
            return new AtomicIslandPredictionSimulationSystem<TSimulation>(
                simulationFactory: factory,
                islandDefinitionId: DefinitionId,
                logger: Logger);
        }

        public IEvent<TConfig> AfterHijack
        {
            get { return _AfterExtensionApplied; }
        }

        private readonly MultiRegisterEvent<TConfig> _AfterExtensionApplied = new();

        public void ModifyPredictionSystems(
            ICollection<ISimulationSystem> simulationSystems,
            PredictionSystemsDependencies dependencies)
        {
            var factory = BuildingSimulationFactoryBuilder.BuildFactory(
                dependencies: dependencies,
                config: out TConfig config);
            simulationSystems.Add(CreateAtomicSystem(factory));

            _AfterExtensionApplied.Invoke(config);
        }
    }
}
