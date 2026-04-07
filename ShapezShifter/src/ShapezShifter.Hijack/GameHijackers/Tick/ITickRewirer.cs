namespace ShapezShifter.Hijack
{
    public interface ITickRewirer : IRewirer
    {
        public void Tick(float deltaTime);
    }
}
