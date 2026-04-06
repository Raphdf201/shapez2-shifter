#nullable enable
using System;
using ShapezShifter.Hijack;

namespace ShapezShifter.Flow
{
    /// <summary>
    /// Extension methods to make mod development easier
    /// </summary>
    public static class GameFlowExtensions
    {
        public static RewirerHandle OnTick(this IMod mod, Action<float> action)
        {
            return GameRewirers.AddRewirer(new ActionTickRewirer(action));
        }
    }
}
