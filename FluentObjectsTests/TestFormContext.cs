using System.Collections.Generic;
using FluentObjects.Mvc4;
using NUnit.Framework;

namespace FluentObjectsTests
{
    [TestFixture]
    public class TestFormContext
    {
        private class TestNameClass
        {
            public string Name { get; set; }
        }

        [TearDown]
        public void TearDown()
        {
            FluentFormContext.Clear();
        }

        private static IList<TestNameClass> GetTestNameList()
        {
            return new List<TestNameClass>
                {
                    new TestNameClass {Name = "Test"}
                };
        }

        [Test]
        public void TestContextPath()
        {
            using (FormContextInstance context = FluentFormContext.SpecifyArray("test", new object()))
            {
                Assert.AreEqual(context.Path, FluentFormContext.Path);
            }

            Assert.AreEqual(string.Empty, FluentFormContext.Path);
        }

        [Test]
        public void TestNestedContextPath()
        {
            using (FormContextInstance context = FluentFormContext.SpecifyArray("test", new object()))
            {
                Assert.AreEqual(context.Path, FluentFormContext.Path);
                
                using (FormContextInstance objectContext = FluentFormContext.SpecifyObject("object"))
                {
                    // TODO: context.Path to be relative to when it was created not the current path
                    Assert.AreEqual(objectContext.Path, FluentFormContext.Path);
                }

                Assert.AreEqual(context.Path, FluentFormContext.Path);
            }

            Assert.AreEqual(string.Empty, FluentFormContext.Path);
        }

        [Test]
        public void TestObjectContextPath()
        {
            Assert.AreEqual(string.Empty, FluentFormContext.Path);
            FluentFormContext.SpecifyObject("Test");
            Assert.AreEqual("Test", FluentFormContext.Path);
            FluentFormContext.Clear();
            Assert.AreEqual(string.Empty, FluentFormContext.Path);
        }

        [Test]
        public void TestForgetArrayContextPath()
        {
            Assert.AreEqual(string.Empty, FluentFormContext.Path);
            
            FluentFormContext.SpecifyArray("Test", new object());
            Assert.AreEqual("Test[0]", FluentFormContext.Path);
            
            FormContextInstance context = FluentFormContext.SpecifyArray("Test", new object(), true);
            Assert.AreEqual("Test[0].Test[0]", FluentFormContext.Path);
            
            context.Dispose();
            Assert.AreEqual("Test[0]", FluentFormContext.Path);
            
            FluentFormContext.Clear();
            Assert.AreEqual(string.Empty, FluentFormContext.Path);
        }

        [Test]
        public void TestArrayTreeModify()
        {
            using (FluentFormContext.SpecifyArray("test", new object()))
            {
                var tree = FluentFormContext.GetTree();
                tree.test[0].Name = "My test name";
                Assert.AreEqual("My test name", tree.test[0].Name);
            }
        }

        [Test]
        public void TestArrayPaths()
        {
            IList<TestNameClass> test = GetTestNameList();

            using (FluentFormContext.SpecifyArray("test", new object()))
            {
                Assert.AreEqual("test[0]", FluentFormContext.Path);
                Assert.AreEqual("test[0].Name", FluentFormContext.PathFor<TestNameClass, string>(m => m.Name));
                Assert.AreEqual("test[0].test[0].Name", FluentFormContext.PathFor<TestNameClass, string>(m => test[0].Name));
            }
        }

        [Test]
        public void TestArrayReuse()
        {
            const int iterations = 10;

            IList<TestNameClass> test = GetTestNameList();

            for (int i = 0; i < iterations; i++)
            {
                // True must be used here to ensure "test" is overwritten
                using (FormContextInstance context = FluentFormContext.SpecifyArray("test", test, true))
                {
                    Assert.AreEqual(context.Path, FluentFormContext.Path);

                    Assert.AreEqual("test[0]", FluentFormContext.Path);
                    Assert.AreEqual("test[0].Name", FluentFormContext.PathFor<TestNameClass, string>(m => m.Name));
                    Assert.AreEqual("test[0].test[0].Name", FluentFormContext.PathFor<TestNameClass, string>(m => test[0].Name));
                }
            }
        }

        [Test]
        public void TestArrayNoReuseNewObjectKeys()
        {
            const int iterations = 10;

            IList<TestNameClass> test = GetTestNameList();

            for (int i = 0; i < iterations; i++)
            {
                using (FormContextInstance context = FluentFormContext.SpecifyArray("test", new object()))
                {
                    Assert.AreEqual(context.Path, FluentFormContext.Path);

                    Assert.AreEqual(string.Format("test[{0}]", i), FluentFormContext.Path);
                    Assert.AreEqual(string.Format("test[{0}].Name", i), FluentFormContext.PathFor<TestNameClass, string>(m => m.Name));
                    Assert.AreEqual(string.Format("test[{0}].test[0].Name", i), FluentFormContext.PathFor<TestNameClass, string>(m => test[0].Name));
                }
            }
        }

        [Test]
        public void TestArrayNoReuseSameKeys()
        {
            const int iterations = 10;

            IList<TestNameClass> test = GetTestNameList();

            for (int i = 0; i < iterations; i++)
            {
                using (FormContextInstance context = FluentFormContext.SpecifyArray("test", test))
                {
                    Assert.AreEqual(context.Path, FluentFormContext.Path);

                    Assert.AreEqual("test[0]", FluentFormContext.Path);
                    Assert.AreEqual("test[0].Name", FluentFormContext.PathFor<TestNameClass, string>(m => m.Name));
                    Assert.AreEqual("test[0].test[0].Name", FluentFormContext.PathFor<TestNameClass, string>(m => test[0].Name));
                }
            }
        }
    }
}
