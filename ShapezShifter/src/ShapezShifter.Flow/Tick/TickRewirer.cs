using System;
using ShapezShifter.Hijack;

namespace ShapezShifter.Flow
{
    internal class TickRewirer : ITickRewirer
    {
        private readonly Action<GameSessionOrchestrator, float> Periodic;

        public TickRewirer(Action<GameSessionOrchestrator, float> action)
        {
            Periodic = action;
        }

        public Action<GameSessionOrchestrator, float> RunPeriodically()
        {
            return Periodic;
        }

        public static RewirerHandle Register(Action<GameSessionOrchestrator, float> action)
        {
            return GameRewirers.AddRewirer(new TickRewirer(action));
        }
    }
}
