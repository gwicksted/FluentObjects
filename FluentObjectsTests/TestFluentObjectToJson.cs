using System.Dynamic;
using FluentObjects;
using NUnit.Framework;
using Newtonsoft.Json;

namespace FluentObjectsTests
{
    [TestFixture]
    public class TestFluentObjectToJson
    {
        [Test]
        public void TestConvertToJson()
        {
            dynamic o = new FluentObject();
            o.Name = "hello!";

            string json = JsonConvert.SerializeObject((ExpandoObject)o);

            Assert.AreEqual("{\"Name\":\"hello!\"}", json);
        }

        [Test]
        public void TestConvertComplexToJson()
        {
            dynamic o = new FluentObject();

            o.test[0].Name.Is.So[12].Cool["YAY"].Oh.Yeah = "MyTest";
            o.test[0].Name.Is.So[12].Yes["HEY"] = 4;
            o.test[0].Name.Is.So[12].Yes["YOU"] = 7;

            string json = JsonConvert.SerializeObject((ExpandoObject)o);

            Assert.AreEqual("{\"test\":[{\"Name\":{\"Is\":{\"So\":{\"12\":{\"Cool\":{\"YAY\":{\"Oh\":{\"Yeah\":\"MyTest\"}}},\"Yes\":{\"HEY\":4,\"YOU\":7}}}}}}]}", json);
        }
    }
}
