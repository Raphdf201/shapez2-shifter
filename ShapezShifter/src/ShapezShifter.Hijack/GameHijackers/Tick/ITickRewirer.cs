using System;

namespace ShapezShifter.Hijack
{
    public interface ITickRewirer : IRewirer
    {
        public Action<GameSessionOrchestrator, float> RunPeriodically();
    }
}
