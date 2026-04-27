using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Mesh = UnityEngine.Mesh;

namespace ShapezShifter.Kit
{
    /// <summary>
    /// Minimal GLB (binary glTF 2.0) parser that converts meshes to Unity format.
    /// Only handles static triangle meshes — no skins, animations, or sparse accessors.
    /// </summary>
    public static class GlbMeshLoader
    {
        // glTF accessor component types
        private const int ComponentType_UByte  = 5121;
        private const int ComponentType_UShort = 5123;
        private const int ComponentType_UInt   = 5125;
        private const int ComponentType_Float  = 5126;

        public static Mesh LoadSingleMesh(string path)
        {
            (JObject root, byte[] bin) = ParseGlb(path);
            JArray meshes = (JArray)root["meshes"];
            if (meshes == null || meshes.Count == 0)
                throw new InvalidDataException("GLB contains no meshes.");
            return ConvertMesh(meshes[0], root, bin);
        }

        public static Mesh[] LoadAllMeshes(string path)
        {
            (JObject root, byte[] bin) = ParseGlb(path);
            JArray meshes = (JArray)root["meshes"];
            if (meshes == null) return Array.Empty<Mesh>();

            var result = new Mesh[meshes.Count];
            for (int i = 0; i < meshes.Count; i++)
                result[i] = ConvertMesh(meshes[i], root, bin);
            return result;
        }

        private static (JObject root, byte[] bin) ParseGlb(string path)
        {
            using var reader = new BinaryReader(File.OpenRead(path), Encoding.UTF8, leaveOpen: false);

            // 12-byte file header
            uint magic   = reader.ReadUInt32();
            uint version = reader.ReadUInt32();
            /*uint fileLen =*/ reader.ReadUInt32();

            if (magic != 0x46546C67u) throw new InvalidDataException("Not a GLB file.");
            if (version != 2)         throw new InvalidDataException($"Unsupported GLB version {version}.");

            // Chunk 0: JSON
            uint jsonChunkLen  = reader.ReadUInt32();
            uint jsonChunkType = reader.ReadUInt32(); // 0x4E4F534A
            if (jsonChunkType != 0x4E4F534Au) throw new InvalidDataException("Expected JSON chunk first.");
            string json = Encoding.UTF8.GetString(reader.ReadBytes((int)jsonChunkLen));
            var root = JObject.Parse(json);

            // Chunk 1: BIN (optional — some GLBs have no binary data)
            byte[] bin = Array.Empty<byte>();
            if (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                uint binChunkLen  = reader.ReadUInt32();
                uint binChunkType = reader.ReadUInt32(); // 0x004E4942
                if (binChunkType == 0x004E4942u)
                    bin = reader.ReadBytes((int)binChunkLen);
            }

            return (root, bin);
        }

        private static Mesh ConvertMesh(JToken meshNode, JObject root, byte[] bin)
        {
            string name = meshNode["name"]?.Value<string>() ?? "GltfMesh";
            var primitives = (JArray)meshNode["primitives"];

            var mesh = new Mesh { name = name };
            mesh.indexFormat = IndexFormat.UInt32;

            // Accumulate across all primitives
            var allPositions = new List<float3>();
            var allNormals   = new List<float3>();
            // UV channels: up to 4 (TEXCOORD_0..3)
            var allUvs       = new List<float2>[4];
            for (int i = 0; i < 4; i++) allUvs[i] = new List<float2>();

            var allIndices   = new List<int>();
            var subMeshes    = new List<SubMeshDescriptor>();
            bool hasNormals  = false;

            foreach (JToken prim in primitives)
            {
                var attrs = (JObject)prim["attributes"];
                int vertBase   = allPositions.Count;
                int indexStart = allIndices.Count;

                // Positions (required)
                ReadVec3Accessor(attrs, "POSITION", root, bin, allPositions, flipX: true);

                // Normals (optional)
                if (attrs["NORMAL"] != null)
                {
                    ReadVec3Accessor(attrs, "NORMAL", root, bin, allNormals, flipX: true);
                    hasNormals = true;
                }

                // UVs
                for (int ch = 0; ch < 4; ch++)
                {
                    string key = $"TEXCOORD_{ch}";
                    if (attrs[key] != null)
                        ReadVec2Accessor(attrs, key, root, bin, allUvs[ch], flipY: true);
                }

                // Indices
                ReadIndices(prim, root, bin, allIndices, vertBase);

                subMeshes.Add(new SubMeshDescriptor(indexStart, allIndices.Count - indexStart));
            }

            // Upload
            using var nativeVerts = new NativeArray<float3>(allPositions.ToArray(), Allocator.Temp);
            mesh.SetVertices(nativeVerts);

            if (hasNormals)
            {
                using var nativeNormals = new NativeArray<float3>(allNormals.ToArray(), Allocator.Temp);
                mesh.SetNormals(nativeNormals);
            }

            using var nativeIndices = new NativeArray<int>(allIndices.ToArray(), Allocator.Temp);
            mesh.SetIndices(nativeIndices, MeshTopology.Triangles, submesh: 0);

            mesh.subMeshCount = subMeshes.Count;
            for (int i = 0; i < subMeshes.Count; i++)
                mesh.SetSubMesh(i, subMeshes[i], MeshUpdateFlags.DontRecalculateBounds);

            for (int ch = 0; ch < 4; ch++)
            {
                if (allUvs[ch].Count > 0)
                {
                    using var nativeUvs = new NativeArray<float2>(allUvs[ch].ToArray(), Allocator.Temp);
                    mesh.SetUVs(ch, nativeUvs);
                }
            }

            mesh.RecalculateBounds();
            if (!hasNormals) mesh.RecalculateNormals();

            return mesh;
        }

        private static void ReadVec3Accessor(
            JObject attrs, string semantic,
            JObject root, byte[] bin,
            List<float3> output, bool flipX)
        {
            int accessorIdx = attrs[semantic].Value<int>();
            GetAccessorInfo(root, bin, accessorIdx,
                out byte[] data, out int count, out int componentType, out int byteStride);

            int stride = byteStride > 0 ? byteStride : 12; // 3 floats × 4 bytes

            for (int i = 0; i < count; i++)
            {
                int offset = i * stride;
                float x = ReadFloat(data, offset + 0, componentType);
                float y = ReadFloat(data, offset + 4, componentType);
                float z = ReadFloat(data, offset + 8, componentType);
                output.Add(new float3(flipX ? -x : x, y, z));
            }
        }

        private static void ReadVec2Accessor(
            JObject attrs, string semantic,
            JObject root, byte[] bin,
            List<float2> output, bool flipY)
        {
            int accessorIdx = attrs[semantic].Value<int>();
            GetAccessorInfo(root, bin, accessorIdx,
                out byte[] data, out int count, out int componentType, out int byteStride);

            int stride = byteStride > 0 ? byteStride : 8; // 2 floats × 4 bytes

            for (int i = 0; i < count; i++)
            {
                int offset = i * stride;
                float u = ReadFloat(data, offset + 0, componentType);
                float v = ReadFloat(data, offset + 4, componentType);
                output.Add(new float2(u, flipY ? 1f - v : v));
            }
        }

        private static void ReadIndices(
            JToken prim, JObject root, byte[] bin,
            List<int> output, int vertexOffset)
        {
            if (prim["indices"] == null)
            {
                // Non-indexed mesh — generate sequential indices
                int vertCount = output.Count - vertexOffset; // already added verts
                // Winding swap: 0,2,1 instead of 0,1,2
                for (int i = 0; i < vertCount; i += 3)
                {
                    output.Add(vertexOffset + i + 0);
                    output.Add(vertexOffset + i + 2);
                    output.Add(vertexOffset + i + 1);
                }
                return;
            }

            int accessorIdx = prim["indices"].Value<int>();
            GetAccessorInfo(root, bin, accessorIdx,
                out byte[] data, out int count, out int componentType, out int byteStride);

            int elementSize = componentType switch
            {
                ComponentType_UByte  => 1,
                ComponentType_UShort => 2,
                ComponentType_UInt   => 4,
                _ => throw new InvalidDataException($"Unexpected index component type {componentType}")
            };
            int stride = byteStride > 0 ? byteStride : elementSize;

            // Winding swap: read 0,2,1 instead of 0,1,2 (counters the X-axis flip)
            for (int i = 0; i < count; i += 3)
            {
                output.Add(vertexOffset + ReadIndex(data, (i + 0) * stride, componentType));
                output.Add(vertexOffset + ReadIndex(data, (i + 2) * stride, componentType));
                output.Add(vertexOffset + ReadIndex(data, (i + 1) * stride, componentType));
            }
        }

        private static void GetAccessorInfo(
            JObject root, byte[] bin, int accessorIdx,
            out byte[] data, out int count, out int componentType, out int byteStride)
        {
            JToken accessor   = root["accessors"][accessorIdx];
            JToken bufferView = root["bufferViews"][accessor["bufferView"].Value<int>()];

            count         = accessor["count"].Value<int>();
            componentType = accessor["componentType"].Value<int>();
            byteStride    = bufferView["byteStride"]?.Value<int>() ?? 0;

            int viewOffset     = bufferView["byteOffset"]?.Value<int>() ?? 0;
            int viewLength     = bufferView["byteLength"].Value<int>();
            int accessorOffset = accessor["byteOffset"]?.Value<int>() ?? 0;

            if (accessor["bufferView"] == null)
                throw new NotSupportedException("Sparse accessors are not supported.");
            if (bufferView["buffer"].Value<int>() != 0)
                throw new NotSupportedException("Only embedded GLB buffers (buffer index 0) are supported.");

            // slice the full bufferView, then the accessor offset is handled
            // by offsetting into `data` during reads — OR slice from the accessor start
            // and read from offset 0. We do the latter for simplicity, but the slice
            // length must be viewLength (not viewLength - accessorOffset).
            int start = viewOffset + accessorOffset;

            // The slice length is viewLength minus how far the accessor is into the view.
            // For non-interleaved buffers this equals the accessor's own byte size.
            // For interleaved buffers we need the full view so stride-based reads work.
            int sliceLength = viewLength - accessorOffset;
            data = new byte[sliceLength];
            Buffer.BlockCopy(bin, start, data, 0, sliceLength);
        }

        // Primitive value readers
        private static float ReadFloat(byte[] data, int offset, int componentType)
        {
            // glTF vec2/vec3 attributes are always float in practice,
            // but handle the component type defensively
            return componentType == ComponentType_Float
                ? BitConverter.ToSingle(data, offset)
                : throw new NotSupportedException($"Non-float attribute component type {componentType}");
        }

        private static int ReadIndex(byte[] data, int offset, int componentType) =>
            componentType switch
            {
                ComponentType_UByte  => data[offset],
                ComponentType_UShort => BitConverter.ToUInt16(data, offset),
                ComponentType_UInt   => (int)BitConverter.ToUInt32(data, offset),
                _ => throw new InvalidDataException($"Unknown index component type {componentType}")
            };
    }
}
