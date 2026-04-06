using System.Collections.Generic;
using Core.Events;
using Core.Factory;
using Core.Logging;
using Game.Core.Simulation;
using ShapezShifter.Hijack;

namespace ShapezShifter.Flow.Atomic
{
    public class BuildingSimulationExtender<TSimulation> : ISimulationSystemsRewirer, IChainableRewirer
        where TSimulation : ISimulation
    {
        private readonly BuildingDefinitionId DefinitionId;
        private readonly IBuildingSimulationFactoryBuilder<TSimulation> BuildingSimulationFactoryBuilder;
        private readonly ILogger Logger;

        public BuildingSimulationExtender(
            BuildingDefinitionId definitionId,
            IBuildingSimulationFactoryBuilder<TSimulation> buildingSimulationFactoryBuilder,
            ILogger logger)
        {
            DefinitionId = definitionId;
            BuildingSimulationFactoryBuilder = buildingSimulationFactoryBuilder;
            Logger = logger;
        }

        public void ModifySimulationSystems(
            ICollection<ISimulationSystem> simulationSystems,
            SimulationSystemsDependencies dependencies)
        {
            var factory = BuildingSimulationFactoryBuilder.BuildFactory(dependencies);
            simulationSystems.Add(CreateAtomicSystem(factory));

            _AfterExtensionApplied.Invoke();
        }

        private ISimulationSystem CreateAtomicSystem(IFactory<TSimulation> factory)
        {
            return new AtomicStatelessBuildingSimulationSystem<TSimulation>(
                simulationFactory: factory,
                buildingDefinitionId: DefinitionId,
                logger: Logger);
        }

        public IEvent AfterHijack
        {
            get { return _AfterExtensionApplied; }
        }

        private readonly MultiRegisterEvent _AfterExtensionApplied = new();
    }

    public class BuildingSimulationExtender<TSimulation, TState, TConfig>
        : ISimulationSystemsRewirer, IChainableRewirer<TConfig>
        where TSimulation : ISimulation
        where TState : class, ISimulationState, new()
    {
        private readonly BuildingDefinitionId DefinitionId;

        private readonly IBuildingSimulationFactoryBuilder<TSimulation, TState, TConfig>
            BuildingSimulationFactoryBuilder;

        private readonly ILogger Logger;

        public BuildingSimulationExtender(
            BuildingDefinitionId definitionId,
            IBuildingSimulationFactoryBuilder<TSimulation, TState, TConfig> buildingSimulationFactoryBuilder,
            ILogger logger)
        {
            DefinitionId = definitionId;
            BuildingSimulationFactoryBuilder = buildingSimulationFactoryBuilder;
            Logger = logger;
        }

        public void ModifySimulationSystems(
            ICollection<ISimulationSystem> simulationSystems,
            SimulationSystemsDependencies dependencies)
        {
            var factory = BuildingSimulationFactoryBuilder.BuildFactory(
                dependencies: dependencies,
                config: out TConfig config);
            simulationSystems.Add(CreateAtomicSystem(factory));

            _AfterExtensionApplied.Invoke(config);
        }

        private ISimulationSystem CreateAtomicSystem(IFactory<TState, TSimulation> factory)
        {
            return new AtomicStatefulBuildingSimulationSystem<TSimulation, TState>(
                simulationFactory: factory,
                buildingDefinitionId: DefinitionId,
                logger: Logger);
        }

        public IEvent<TConfig> AfterHijack
        {
            get { return _AfterExtensionApplied; }
        }

        private readonly MultiRegisterEvent<TConfig> _AfterExtensionApplied = new();
    }
}
