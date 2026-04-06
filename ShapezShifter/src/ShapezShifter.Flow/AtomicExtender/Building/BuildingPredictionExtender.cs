using System.Collections.Generic;
using Core.Events;
using Core.Factory;
using Core.Logging;
using Game.Content.Features.Predictions;
using ShapezShifter.Hijack;
using ShapezShifter.Hijack.Predictions;

namespace ShapezShifter.Flow.Atomic
{
    public class BuildingPredictionExtender<TSimulation> : IPredictionSystemsRewirer, IChainableRewirer
        where TSimulation : IItemPredictionSimulation
    {
        private readonly BuildingDefinitionId DefinitionId;
        private readonly IBuildingPredictionFactoryBuilder<TSimulation> BuildingSimulationFactoryBuilder;
        private readonly ILogger Logger;

        public BuildingPredictionExtender(
            BuildingDefinitionId definitionId,
            IBuildingPredictionFactoryBuilder<TSimulation> buildingSimulationFactoryBuilder,
            ILogger logger)
        {
            DefinitionId = definitionId;
            BuildingSimulationFactoryBuilder = buildingSimulationFactoryBuilder;
            Logger = logger;
        }

        private ISimulationSystem CreateAtomicSystem(IFactory<TSimulation> factory)
        {
            return new AtomicBuildingPredictionSimulationSystem<TSimulation>(
                simulationFactory: factory,
                buildingDefinitionId: DefinitionId,
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
            var factory = BuildingSimulationFactoryBuilder.BuildFactory(dependencies);
            simulationSystems.Add(CreateAtomicSystem(factory));

            _AfterExtensionApplied.Invoke();
        }
    }

    public class BuildingPredictionExtender<TSimulation, TConfig>
        : IPredictionSystemsRewirer, IChainableRewirer<TConfig>
        where TSimulation : IItemPredictionSimulation
    {
        private readonly BuildingDefinitionId DefinitionId;
        private readonly IBuildingPredictionFactoryBuilder<TSimulation, TConfig> BuildingSimulationFactoryBuilder;
        private readonly ILogger Logger;

        public BuildingPredictionExtender(
            BuildingDefinitionId definitionId,
            IBuildingPredictionFactoryBuilder<TSimulation, TConfig> buildingSimulationFactoryBuilder,
            ILogger logger)
        {
            DefinitionId = definitionId;
            BuildingSimulationFactoryBuilder = buildingSimulationFactoryBuilder;
            Logger = logger;
        }

        private ISimulationSystem CreateAtomicSystem(IFactory<TSimulation> factory)
        {
            return new AtomicBuildingPredictionSimulationSystem<TSimulation>(
                simulationFactory: factory,
                buildingDefinitionId: DefinitionId,
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
