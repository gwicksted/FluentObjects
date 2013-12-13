using System;
using System.Collections.Generic;
using System.Diagnostics;
using FluentObjects;
using NUnit.Framework;

namespace FluentObjectsTests
{
    [TestFixture]
    public class TestFluentObject
    {
        private class TestNameClass
        {
            public string Name { get; set; }
        }

        [Test]
        public void TestImplicitIDictionaryConversion()
        {
            dynamic o = new FluentObject();

            o.test[0].Name.Is.So[12].Cool["YAY"].Oh.Yeah = "MyTest";
            o.test[0].Name.Is.So[12].Yes["HEY"] = 4;
            o.test[0].Name.Is.So[12].Yes["YOU"] = 7;

            var dict = o.test[0].Name.Is.So[12].Yes;

            IDictionary<string, int> imp = dict;

            Debug.Print(string.Join(", ", imp));
        }

        [Test]
        public void TestExplicitIDictionaryConversion()
        {
            dynamic o = new FluentObject();

            o.test[0].Name.Is.So[12].Cool["YAY"].Oh.Yeah = "MyTest";
            o.test[0].Name.Is.So[12].Yes["HEY"] = 4;
            o.test[0].Name.Is.So[12].Yes["YOU"] = 7;

            var dict = o.test[0].Name.Is.So[12].Yes;

            try
            {
                IDictionary<string, int> implicitIDictionary = (IDictionary<string, int>) dict;
                Assert.Fail("Expected to throw here - You fixed implicit IDictionary casts!");
            }
            catch (Exception)
            {
                // expected
            }

            IDictionary<string, int> semiImplicitIDictionary =
                (IDictionary<string, int>) dict.ToDictionary<string, int>();

            Debug.Print(string.Join(", ", semiImplicitIDictionary));
        }

        [Test]
        public void TestDictionaryConversion()
        {
            dynamic o = new FluentObject();

            o.test[0].Name.Is.So[12].Cool["YAY"].Oh.Yeah = "MyTest";
            o.test[0].Name.Is.So[12].Yes["HEY"] = 4;
            o.test[0].Name.Is.So[12].Yes["YOU"] = 7;

            Debug.Print(string.Join(", ", ((Dictionary<string, int>) (o.test[0].Name.Is.So[12].Yes))));
        }


        [Test]
        public void TestDynamicExpressionWithString()
        {
            dynamic o = new FluentObject();

            o.test[0].Name.Is.So[12].Cool["YAY"].Oh.Yeah = "MyTest";
            o.test[0].Name.Is.So[12].Yes["HEY"] = 4;
            o.test[0].Name.Is.So[12].Yes["YOU"] = 7;

            Assert.AreEqual((string) (o.test[0].Name.Is.So[12].Cool["YAY"].Oh.Yeah), "MyTest");
            Assert.AreEqual(o.test[0].Name.Is.So[12].Cool["YAY"].Oh.Yeah, "MyTest");
        }


        [Test]
        public void TestDynamicIteration()
        {
            dynamic o = new FluentObject();

            o.test[0].Name.Is.So[12].Cool["YAY"].Oh.Yeah = "MyTest";
            o.test[0].Name.Is.So[12].Yes["HEY"] = 4;
            o.test[0].Name.Is.So[12].Yes["YOU"] = 7;

            bool iterated = false;
            foreach (dynamic obj in o.test)
            {
                if (iterated)
                {
                    Assert.Fail("Expected only a single iteration");
                }

                iterated = true;

                Assert.AreEqual((obj.Name.Is.So[12].Cool["YAY"].Oh.Yeah), "MyTest");
            }

            if (!iterated)
            {
                Assert.Fail("Expected one iteration");
            }
        }


        [Test]
        public void TestCaseSensitivity()
        {
            dynamic o = new FluentObject();

            o.test[0].Name.Is.So[12].Cool["YAY"].Oh.Yeah = "MyTest";
            o.test[0].Name.Is.So[12].Yes["HEY"] = 4;
            o.test[0].Name.Is.So[12].Yes["YOU"] = 7;

            o.Test.Yadda = "abc";

            Assert.AreEqual("abc", o.Test.Yadda);
            Assert.AreEqual("MyTest", o.test[0].Name.Is.So[12].Cool["YAY"].Oh.Yeah);
            // TODO: implement this on arrays?
            //Assert.AreEqual(o.test.Count(), 1);
        }

        [Test]
        public void TestFunctionCallContinuation()
        {
            dynamic o = new FluentObject();

            o.test[0].Name.Is.So[12].Cool["YAY"].Oh.Yeah = "MyTest";
            o.test[0].Name.Is.So[12].Yes["HEY"] = 4;
            o.test[0].Name.Is.So[12].Yes["YOU"] = 7;

            o.Test("abc").Yadda = "abc";
            o.Test("def").Yadda = "def";

            // TODO: allow different parameters to return different values?

            Assert.AreEqual("def", o.Test().Yadda);
            Assert.AreEqual("def", o.Test("abc").Yadda);
            Assert.AreEqual("def", o.Test("def").Yadda);
        }

        [Test]
        public void TestReadBeforeWrite()
        {
            dynamic o = new FluentObject();

            o.test[0].Name.Is.So[12].Cool["YAY"].Oh.Yeah = "MyTest";
            o.test[0].Name.Is.So[12].Yes["HEY"] = 4;
            o.test[0].Name.Is.So[12].Yes["YOU"] = 7;

            Assert.AreEqual(default(string), (string) o.x);
            Assert.AreEqual("0", string.Format("{0}", (int) (o.x[4])));
        }

        [Test]
        public void TestInvalidExtensionOfNative()
        {
            dynamic o = new FluentObject();

            o.x = 5;

            // This throws since x is int
            try
            {
                o.x.y = 2;
                Assert.Fail("Expected to throw here - While this could be a feature, it's not intended.");
            }
            catch (Exception)
            {
            }
        }

        [Test]
        public void TestFromFluentObject()
        {
            dynamic o = new FluentObject();

            o.test[0].Name.Is.So[12].Cool["YAY"].Oh.Yeah = "MyTest";
            o.test[0].Name.Is.So[12].Yes["HEY"] = 4;
            o.test[0].Name.Is.So[12].Yes["YOU"] = 7;

            dynamic o2 = FluentObject.From(o);

            Assert.AreEqual("MyTest", o2.test[0].Name.Is.So[12].Cool["YAY"].Oh.Yeah);
        }

        [Test]
        public void TestFromClass()
        {
            TestNameClass test = new TestNameClass();
            test.Name = "Super";

            dynamic testFrom = FluentObject.From(test);

            Assert.AreEqual("Super", testFrom.Name);
        }

        [Test]
        public void TestFromDictionary()
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            dictionary.Add("test", 123);

            dynamic dynamicDictionary = FluentObject.From(dictionary);

            Assert.AreEqual(123, dynamicDictionary["test"]);
            Assert.AreEqual(default(int), (int) (dynamicDictionary["asdf"]));
        }

        [Test]
        public void TestFromIDictionary()
        {
            IDictionary<string, int> dictionary = new Dictionary<string, int>();
            dictionary.Add("test", 123);

            dynamic dynamicDictionary = FluentObject.From(dictionary);

            Assert.AreEqual(123, dynamicDictionary["test"]);
            Assert.AreEqual(default(int), (int) (dynamicDictionary["asdf"]));
        }
    }
}
