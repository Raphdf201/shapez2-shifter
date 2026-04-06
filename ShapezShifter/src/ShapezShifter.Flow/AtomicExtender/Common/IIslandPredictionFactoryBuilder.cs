using Core.Factory;
using ShapezShifter.Hijack.Predictions;

namespace ShapezShifter.Flow.Atomic
{
    public interface IIslandPredictionFactoryBuilder<out TSimulation>
    {
        IFactory<TSimulation> BuildFactory(PredictionSystemsDependencies dependencies);
    }

    public interface IIslandPredictionFactoryBuilder<out TSimulation, TConfig>
    {
        IFactory<TSimulation> BuildFactory(PredictionSystemsDependencies dependencies, out TConfig config);
    }
}
