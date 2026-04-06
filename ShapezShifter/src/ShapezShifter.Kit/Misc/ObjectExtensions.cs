using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using ShapezShifter.Kit.ArrayExtensions;

// Written by Burtsev-Alexey
// MIT License
// https://github.com/Burtsev-Alexey/net-object-deep-copy

namespace ShapezShifter.Kit
{
    [PublicAPI]
    public static class ObjectExtensions
    {
        private static readonly MethodInfo CloneMethod = typeof(object).GetMethod(
            name: "MemberwiseClone",
            bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(string))
            {
                return true;
            }

            return type.IsValueType & type.IsPrimitive;
        }

        public static object DeepCopy(this object originalObject)
        {
            return InternalCopy(
                originalObject: originalObject,
                visited: new Dictionary<object, object>(new ReferenceEqualityComparer()));
        }

        private static object InternalCopy(object originalObject, IDictionary<object, object> visited)
        {
            if (originalObject == null)
            {
                return null;
            }

            Type typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect))
            {
                return originalObject;
            }

            if (visited.TryGetValue(key: originalObject, value: out object copy))
            {
                return copy;
            }

            if (typeof(Delegate).IsAssignableFrom(typeToReflect))
            {
                return null;
            }

            object cloneObject = CloneMethod.Invoke(obj: originalObject, parameters: null);
            if (typeToReflect.IsArray)
            {
                Type arrayType = typeToReflect.GetElementType();
                if (!IsPrimitive(arrayType))
                {
                    var clonedArray = (Array)cloneObject;
                    clonedArray.ForEach(
                        (array, indices) => array.SetValue(
                            value: InternalCopy(originalObject: clonedArray.GetValue(indices), visited: visited),
                            indices: indices));
                }
            }

            visited.Add(key: originalObject, value: cloneObject);
            CopyFields(
                originalObject: originalObject,
                visited: visited,
                cloneObject: cloneObject,
                typeToReflect: typeToReflect);
            RecursiveCopyBaseTypePrivateFields(
                originalObject: originalObject,
                visited: visited,
                cloneObject: cloneObject,
                typeToReflect: typeToReflect);
            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(
            object originalObject,
            IDictionary<object, object> visited,
            object cloneObject,
            Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(
                    originalObject: originalObject,
                    visited: visited,
                    cloneObject: cloneObject,
                    typeToReflect: typeToReflect.BaseType);
                CopyFields(
                    originalObject: originalObject,
                    visited: visited,
                    cloneObject: cloneObject,
                    typeToReflect: typeToReflect.BaseType,
                    bindingFlags: BindingFlags.Instance | BindingFlags.NonPublic,
                    filter: info => info.IsPrivate);
            }
        }

        private static void CopyFields(
            object originalObject,
            IDictionary<object, object> visited,
            object cloneObject,
            Type typeToReflect,
            BindingFlags bindingFlags =
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy,
            Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && !filter(fieldInfo))
                {
                    continue;
                }

                if (IsPrimitive(fieldInfo.FieldType))
                {
                    continue;
                }

                object originalFieldValue = fieldInfo.GetValue(originalObject);
                object clonedFieldValue = InternalCopy(originalObject: originalFieldValue, visited: visited);
                fieldInfo.SetValue(obj: cloneObject, value: clonedFieldValue);
            }
        }

        public static T DeepCopy<T>(this T original)
        {
            return (T)DeepCopy((object)original);
        }
    }

    public class ReferenceEqualityComparer : EqualityComparer<object>
    {
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(objA: x, objB: y);
        }

        public override int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }
    }

    namespace ArrayExtensions
    {
        public static class ArrayExtensions
        {
            public static void ForEach(this Array array, Action<Array, int[]> action)
            {
                if (array.LongLength == 0)
                {
                    return;
                }

                ArrayTraverse walker = new(array);
                do
                {
                    action(arg1: array, arg2: walker.Position);
                }
                while (walker.Step());
            }
        }

        internal class ArrayTraverse
        {
            private readonly int[] MaxLengths;
            public readonly int[] Position;

            public ArrayTraverse(Array array)
            {
                MaxLengths = new int[array.Rank];
                for (int i = 0; i < array.Rank; ++i)
                {
                    MaxLengths[i] = array.GetLength(i) - 1;
                }

                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (int i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < MaxLengths[i])
                    {
                        Position[i]++;
                        for (int j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }

                        return true;
                    }
                }

                return false;
            }
        }
    }
}
