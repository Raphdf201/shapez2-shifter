using Game.Core.Coordinates;
using UnityEngine;

namespace ShapezShifter.Kit
{
    public class BoundingBoxHelper
    {
        /// <remarks>
        /// Consider setting colliders manually for more complicated shapes or more accuracy
        /// </remarks>
        public static CollisionBox[] CreateBasicCollider(Mesh mesh)
        {
            return new[]
            {
                new CollisionBox(
                    new SerializedCollisionBox
                    {
                        Center_L = new LocalVector(mesh.bounds.center),
                        Dimensions_L = mesh.bounds.size
                    })
            };
        }
    }
}
