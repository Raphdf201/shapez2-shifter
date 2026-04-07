using Core.Factory;
using ShapezShifter.Hijack.Predictions;

namespace ShapezShifter.Flow.Atomic
{
    public interface IBuildingPredictionFactoryBuilder<out TSimulation>
    {
        IFactory<TSimulation> BuildFactory(PredictionSystemsDependencies dependencies);
    }

    public interface IBuildingPredictionFactoryBuilder<out TSimulation, TConfig>
    {
        IFactory<TSimulation> BuildFactory(PredictionSystemsDependencies dependencies, out TConfig config);
    }
}
