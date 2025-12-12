using System;
using System.Collections.Generic;
using MonoMod.RuntimeDetour;
using ShapezShifter.SharpDetour;

namespace ShapezShifter.Hijack
{
    internal class TickInterceptor : IDisposable
    {
        private readonly IRewirerProvider RewirerProvider;
        private readonly Hook TickHook;

        public TickInterceptor(IRewirerProvider rewirerProvider)
        {
            RewirerProvider = rewirerProvider;
            TickHook = DetourHelper.CreatePostfixHook<GameSessionOrchestrator, float>(
                (orchestrator, time) => orchestrator.Tick(time),
                Update);
        }

        private void Update(GameSessionOrchestrator orchestrator, float time)
        {
            IEnumerable<ITickRewirer> tickRewirers =
                RewirerProvider.RewirersOfType<ITickRewirer>();

            foreach (ITickRewirer tickRewirer in tickRewirers)
            {
                tickRewirer.RunPeriodically()(orchestrator, time);
            }
        }

        public void Dispose()
        {
            TickHook.Dispose();
        }
    }
}