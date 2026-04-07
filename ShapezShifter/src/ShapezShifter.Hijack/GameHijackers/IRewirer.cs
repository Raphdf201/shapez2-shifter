using System;

namespace ShapezShifter.Hijack
{
    /// <summary>
    /// An actor that alters intercepted logic or data
    /// </summary>
    public interface IRewirer : IEquatable<IRewirer>
    {
        bool IEquatable<IRewirer>.Equals(IRewirer other)
        {
            return ReferenceEquals(objA: this, objB: other);
        }
    }
}
