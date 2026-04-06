using Assimp;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Mesh = UnityEngine.Mesh;

namespace ShapezShifter.Kit
{
    public static class AssimpToUnityMeshConverter
    {
        public static Mesh ConvertStaticMesh(Assimp.Mesh source)
        {
            Mesh target = new() { name = source.Name };

            ConvertVertices(source: source, target: target);

            ConvertFaces(source: source, target: target);

            ConvertNormals(source: source, target: target);

            ConvertUVs(source: source, target: target);

            return target;
        }

        private static void ConvertVertices(Assimp.Mesh source, Mesh target)
        {
            // TODO: this can be drastically optimized using the native pointers, but careful with the X axis flipping
            var meshVertices = new NativeArray<float3>(length: source.Vertices.Count, allocator: Allocator.Temp);
            for (int i = 0; i < source.Vertices.Count; i++)
            {
                Vector3D v = source.Vertices[i];
                meshVertices[i] = new float3(x: -v.X, y: v.Y, z: v.Z);
            }

            target.SetVertices(meshVertices);
        }

        private static void ConvertFaces(Assimp.Mesh source, Mesh target)
        {
            using var indices = new NativeList<int>(Allocator.Temp);

            for (int i = 0; i < source.Faces.Count; i++)
            {
                Face f = source.Faces[i];
                if (f.IndexCount != 3)
                {
                    // Ignore anything that is not a triangle
                    continue;
                }

                indices.Add(f.Indices[2]);
                indices.Add(f.Indices[1]);
                indices.Add(f.Indices[0]);
            }

            target.SetIndices(indices: indices.AsArray(), topology: MeshTopology.Triangles, submesh: 0);
        }

        private static void ConvertNormals(Assimp.Mesh source, Mesh target)
        {
            var meshNormals = new NativeArray<float3>(length: source.Normals.Count, allocator: Allocator.Temp);
            for (int i = 0; i < source.Normals.Count; i++)
            {
                Vector3D n = source.Normals[i];
                meshNormals[i] = new float3(x: -n.X, y: n.Y, z: n.Z);
            }

            target.SetNormals(meshNormals);
        }

        private static void ConvertUVs(Assimp.Mesh source, Mesh target)
        {
            for (int channel = 0; channel < source.TextureCoordinateChannels.Length; channel++)
            {
                if (!source.HasTextureCoords(channel))
                {
                    continue;
                }

                var sourceUv = source.TextureCoordinateChannels[channel];
                var uvs = new NativeArray<float2>(length: source.Normals.Count, allocator: Allocator.Temp);

                for (int i = 0; i < source.Normals.Count; i++)
                {
                    Vector3D n = sourceUv[i];
                    uvs[i] = new float2(x: n.X, y: n.Y);
                }

                target.SetUVs(channel: channel, uvs: uvs);
            }
        }
    }
}
