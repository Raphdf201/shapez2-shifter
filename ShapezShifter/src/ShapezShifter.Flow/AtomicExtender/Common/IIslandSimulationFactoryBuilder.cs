using Core.Factory;
using ShapezShifter.Hijack;

namespace ShapezShifter.Flow.Atomic
{
    public interface IIslandSimulationFactoryBuilder<out TSimulation, in TState, TConfig>
    {
        IFactory<TState, IslandInstance, TSimulation> BuildFactory(
            SimulationSystemsDependencies dependencies,
            out TConfig config);
    }
}
