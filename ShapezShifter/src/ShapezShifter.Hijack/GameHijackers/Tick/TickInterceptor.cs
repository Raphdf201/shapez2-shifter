using System;
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
                original: (orchestrator, time) => orchestrator.Tick(time),
                postfix: Update);
        }

        private void Update(GameSessionOrchestrator orchestrator, float time)
        {
            var tickRewirers = RewirerProvider.RewirersOfType<ITickRewirer>();

            foreach (ITickRewirer tickRewirer in tickRewirers)
            {
                tickRewirer.Tick(time);
            }
        }

        public void Dispose()
        {
            TickHook.Dispose();
        }
    }
}
