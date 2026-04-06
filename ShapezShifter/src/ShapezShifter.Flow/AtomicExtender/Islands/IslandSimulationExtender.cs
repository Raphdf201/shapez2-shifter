using System.Collections.Generic;
using Core.Events;
using Core.Factory;
using Core.Logging;
using Game.Core.Simulation;
using ShapezShifter.Hijack;

namespace ShapezShifter.Flow.Atomic
{
    public class IslandSimulationExtender<TSimulation> : ISimulationSystemsRewirer, IChainableRewirer
        where TSimulation : ISimulation
    {
        private readonly IslandDefinitionId DefinitionId;
        private readonly IBuildingSimulationFactoryBuilder<TSimulation> BuildingSimulationFactoryBuilder;

        public IslandSimulationExtender(
            IslandDefinitionId definitionId,
            IBuildingSimulationFactoryBuilder<TSimulation> buildingSimulationFactoryBuilder)
        {
            DefinitionId = definitionId;
            BuildingSimulationFactoryBuilder = buildingSimulationFactoryBuilder;
        }

        public void ModifySimulationSystems(
            ICollection<ISimulationSystem> simulationSystems,
            SimulationSystemsDependencies dependencies)
        {
            var factory = BuildingSimulationFactoryBuilder.BuildFactory(dependencies);
            simulationSystems.Add(CreateAtomicSystem(factory: factory, logger: dependencies.Logger));

            _AfterExtensionApplied.Invoke();
        }

        private ISimulationSystem CreateAtomicSystem(IFactory<TSimulation> factory, ILogger logger)
        {
            return new AtomicStatelessIslandSimulationSystem<TSimulation>(
                simulationFactory: factory,
                islandDefinitionId: DefinitionId,
                logger: logger);
        }

        public IEvent AfterHijack
        {
            get { return _AfterExtensionApplied; }
        }

        private readonly MultiRegisterEvent _AfterExtensionApplied = new();
    }

    public class IslandSimulationExtender<TSimulation, TState, TConfig>
        : ISimulationSystemsRewirer, IChainableRewirer<TConfig>
        where TSimulation : Simulation<TState>
        where TState : class, ISimulationState, new()
    {
        private readonly IslandDefinitionId DefinitionId;
        private readonly IIslandSimulationFactoryBuilder<TSimulation, TState, TConfig> SimulationFactoryBuilder;

        public IslandSimulationExtender(
            IslandDefinitionId definitionId,
            IIslandSimulationFactoryBuilder<TSimulation, TState, TConfig> simulationFactoryBuilder)
        {
            DefinitionId = definitionId;
            SimulationFactoryBuilder = simulationFactoryBuilder;
        }

        public void ModifySimulationSystems(
            ICollection<ISimulationSystem> simulationSystems,
            SimulationSystemsDependencies dependencies)
        {
            var factory = SimulationFactoryBuilder.BuildFactory(dependencies: dependencies, config: out TConfig config);
            simulationSystems.Add(CreateAtomicSystem(factory: factory, logger: dependencies.Logger));

            _AfterExtensionApplied.Invoke(config);
        }

        private ISimulationSystem CreateAtomicSystem(
            IFactory<TState, IslandInstance, TSimulation> factory,
            ILogger logger)
        {
            return new AtomicStatefulIslandSimulationSystem<TSimulation, TState>(
                simulationFactory: factory,
                islandDefinitionId: DefinitionId,
                logger: logger);
        }

        public IEvent<TConfig> AfterHijack
        {
            get { return _AfterExtensionApplied; }
        }

        private readonly MultiRegisterEvent<TConfig> _AfterExtensionApplied = new();
    }
}
