using System;
using ShapezShifter.Hijack;

namespace ShapezShifter.Flow
{
    internal class ActionTickRewirer : ITickRewirer
    {
        private readonly Action<float> OnTick;

        public ActionTickRewirer(Action<float> action)
        {
            OnTick = action;
        }

        public void Tick(float deltaTime)
        {
            OnTick.Invoke(deltaTime);
        }
    }
}
