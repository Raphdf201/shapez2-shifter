using System;
using System.Collections.Generic;
using System.Linq;
using Game.Orchestration;
using MonoMod.RuntimeDetour;
using ShapezShifter.SharpDetour;

namespace ShapezShifter.Hijack
{
    internal class SimulationSystemsInterceptor : IDisposable
    {
        private readonly IRewirerProvider RewirerProvider;
        private readonly Hook CreateSimulationSystemsHook;

        internal SimulationSystemsInterceptor(IRewirerProvider rewirerProvider)
        {
            RewirerProvider = rewirerProvider;

            CreateSimulationSystemsHook =
                DetourHelper.CreatePostfixHook<BuiltinSimulationSystems, IEnumerable<ISimulationSystem>>(
                    original: builtinSimulationSystems => builtinSimulationSystems.CreateSimulationSystems(),
                    postfix: CreateSimulationSystems);
        }

        private IEnumerable<ISimulationSystem> CreateSimulationSystems(
            BuiltinSimulationSystems builtinSimulationSystems,
            IEnumerable<ISimulationSystem> systems)
        {
            var systemsList = systems.ToList();
            var simulationSystemsRewirers = RewirerProvider.RewirersOfType<ISimulationSystemsRewirer>();
            SimulationSystemsDependencies dependencies = new(builtinSimulationSystems);
            foreach (ISimulationSystemsRewirer simulationSystemsRewirer in simulationSystemsRewirers)
            {
                simulationSystemsRewirer.ModifySimulationSystems(
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
