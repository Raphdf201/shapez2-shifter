using System;
using System.Reflection;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace ShapezShifter.SharpDetour
{
    public readonly struct FluentHook
    {
        public static HookPromiseInstance<TObject> Targeting<TObject>()
        {
            return new HookPromiseInstance<TObject>();
        }
    }

    public static class FluentHookTest
    {
        public static void Test()
        {
            MethodInfo original = typeof(Test).GetMethod(
                                      name: "MultiplyAndCeilToInt",
                                      bindingAttr: BindingFlags.Public | BindingFlags.Instance)
                                  ?? throw new InvalidOperationException();

            using Hook hook = new(
                source: original,
                target: typeof(FluentHookTest).GetMethod(nameof(Patch)) ?? throw new InvalidOperationException());

            using Hook hook2 = DetourHelper.CreatePostfixHook<Test, float, float, int>(
                original: (test, f1, f2) => test.MultiplyAndCeilToInt(f1, f2),
                postfix: AddOne);

            using Hook hook3 = FluentHook.Targeting<Test>()
                                         .WithArg<float>()
                                         .WithArg<float>()
                                         .Returning<int>()
                                         .Detour((test, f1, f2) => test.MultiplyAndCeilToInt(f1, f2))
                                         .Postfix(AddOne);

            int Patch(Func<Test, float, float, int> orig, Test self, float arg0, float arg1)
            {
                int value = orig(arg1: self, arg2: arg0, arg3: arg1);
                return AddOne(arg1: self, arg2: arg0, arg3: arg1, result: value);
            }
        }

        private static int AddOne(Test arg1, float arg2, float arg3, int result)
        {
            return result + 1;
        }
    }

    public class Test
    {
        public int MultiplyAndCeilToInt(float a, float b)
        {
            return Mathf.CeilToInt(a * b);
        }
    }
}
