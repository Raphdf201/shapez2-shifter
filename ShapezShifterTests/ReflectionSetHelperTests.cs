using MonoMod.RuntimeDetour;
using ShapezShifter.SharpDetour;

namespace ShapezShifterTests
{
    public class ReflectionSetHelperTests
    {
        [Test]
        public void AssertPublicClassFieldCanBeTypeSafelySetToArbitraryValue()
        {
            PublicMembersTestClass testClass = new(0);

            using Hook? hook = testClass.Set(expr: x => x.Field, value: 42);

            Assert.That(actual: testClass.Field, expression: Is.EqualTo(42));
        }

        [Test]
        public void AssertPublicStructFieldCanBeTypeSafelySetToArbitraryValue()
        {
            PublicMembersTestStructure testStructure = new(0);

            using Hook? hook = testStructure.Set(expr: x => x.Field, value: 42);

            Assert.That(actual: testStructure.Field, expression: Is.EqualTo(42));
        }

        [Test]
        public void AssertPublicClassPropertyCanBeTypeSafelyReplaceWithArbitraryGetter()
        {
            PublicMembersTestClass testClass = new(10);

            Assert.That(actual: testClass.Property, expression: Is.EqualTo(20));

            using Hook? hook = testClass.Set(expr: x => x.Property, value: 21);

            Assert.That(actual: testClass.Property, expression: Is.EqualTo(21));
        }

        [Test]
        public void AssertPublicStructPropertyCanBeTypeSafelyReplaceWithArbitraryGetter()
        {
            PublicMembersTestStructure testStruct = new(10);

            Assert.That(actual: testStruct.Property, expression: Is.EqualTo(20));

            using Hook? hook = testStruct.Set(expr: x => x.Property, value: 21);

            Assert.That(actual: testStruct.Property, expression: Is.EqualTo(21));
        }

        // [Test]
        // public void AssertPrivateClassFieldCanBeTypeSafelySetToArbitraryValue()
        // {
        //     PrivateMembersTestClass testClass = new(0);
        //
        //     using Hook? hook = testClass.Set(x => x.Field, 42);
        //
        //     Assert.That(testClass.Field, Is.EqualTo(42));
        // }
        //
        // [Test]
        // public void AssertPrivateStructFieldCanBeTypeSafelySetToArbitraryValue()
        // {
        //     PrivateMembersTestStructure testStructure = new(0);
        //
        //     using Hook? hook = testStructure.Set(x => x.Field, 42);
        //
        //     Assert.That(testStructure.Field, Is.EqualTo(42));
        // }
        //
        // [Test]
        // public void AssertPrivateClassPropertyCanBeTypeSafelyReplaceWithArbitraryGetter()
        // {
        //     PrivateMembersTestClass testClass = new(10);
        //
        //     Assert.That(testClass.Property, Is.EqualTo(20));
        //
        //     using Hook? hook = testClass.Set(x => x.Property, 21);
        //
        //     Assert.That(testClass.Property, Is.EqualTo(21));
        // }
        //
        // [Test]
        // public void AssertPrivateStructPropertyCanBeTypeSafelyReplaceWithArbitraryGetter()
        // {
        //     PublicMembersTestStructure testStruct = new(10);
        //
        //     Assert.That(testStruct.Property, Is.EqualTo(20));
        //
        //     using Hook? hook = testStruct.Set(x => x.Property, 21);
        //
        //     Assert.That(testStruct.Property, Is.EqualTo(21));
        // }

        private class PublicMembersTestClass(int field)
        {
            public readonly int Field = field;

            public int Property
            {
                get { return Field * 2; }
            }
        }

        private readonly struct PublicMembersTestStructure(int field)
        {
            public readonly int Field = field;

            public int Property
            {
                get { return Field * 2; }
            }
        }
    }
}
