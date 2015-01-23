using System.IO;
using System.Text;
using System.Xml.Serialization;
using FluentObjects;
using NUnit.Framework;

namespace FluentObjectsTests
{
    [TestFixture]
    public class TestFluentObjectXmlSerializer
    {
        [Test]
        public void TestBasicXml()
        {
            dynamic o = new FluentObject();

            o.test = "asdf";
            
            XmlSerializer x = new XmlSerializer(typeof(FluentObject));

            string output;

            using (MemoryStream stream = new MemoryStream())
            {
                x.Serialize(stream, o);
                output = GetUtf8FromMemoryStream(stream);
            }

            Assert.AreEqual("<?xml version=\"1.0\"?>\r\n<FluentObject test=\"asdf\" />", output);
        }

        [Test]
        public void TestXmlObjects()
        {
            dynamic o = new FluentObject();

            o.test.on.you = "true";

            XmlSerializer x = new XmlSerializer(typeof(FluentObject));

            string output;

            using (MemoryStream stream = new MemoryStream())
            {
                x.Serialize(stream, o);
                output = GetUtf8FromMemoryStream(stream);
            }

            Assert.AreEqual("<?xml version=\"1.0\"?>\r\n<FluentObject>\r\n  <test>\r\n    <on you=\"true\" />\r\n  </test>\r\n</FluentObject>", output);
        }

        [Test]
        public void TestXmlArrays()
        {
            dynamic o = new FluentObject();

            o.test[0].element = "true";
            o.test[1].element = "false";

            Assert.AreEqual("true", o.test[0].element);
            Assert.AreEqual("false", o.test[1].element);

            XmlSerializer x = new XmlSerializer(typeof(FluentObject));

            string output;

            using (MemoryStream stream = new MemoryStream())
            {
                x.Serialize(stream, o);
                output = GetUtf8FromMemoryStream(stream);
            }

            const string xml = "<?xml version=\"1.0\"?>\r\n<FluentObject>\r\n  <test>\r\n    <test element=\"true\" />\r\n    <test element=\"false\" />\r\n  </test>\r\n</FluentObject>";
            Assert.AreEqual(xml, output);
            byte[] buffer = Encoding.UTF8.GetBytes(xml);
            using (Stream stream = new MemoryStream(buffer))
            {
                dynamic result = x.Deserialize(stream);

                // TODO: not quite equal (utf-16 decl)
                //string resultXml = ((FluentObject)result).ToXmlString("FluentObject");
                //Assert.AreEqual(xml, resultXml);

                Assert.AreEqual("true", result.test[0].element);
                Assert.AreEqual("false", result.test[1].element);
            }
        }

        [Test]
        public void TestXmlDictionary()
        {
            dynamic o = new FluentObject();

            o.test["abc"].element = "true";
            o.test["def"].element = "false";

            XmlSerializer x = new XmlSerializer(typeof(FluentObject));

            string output;

            using (MemoryStream stream = new MemoryStream())
            {
                x.Serialize(stream, o);
                output = GetUtf8FromMemoryStream(stream);
            }

            // TODO: fix because dictionary keys may not be valid xml element names and because this will not deserialize to a dictionary
            Assert.AreEqual("<?xml version=\"1.0\"?>\r\n<FluentObject>\r\n  <test>\r\n    <abc element=\"true\" />\r\n    <def element=\"false\" />\r\n  </test>\r\n</FluentObject>", output);
        }

        // TODO: more tests to assert dictionary/array re-serialize back to dictionary/array (I assume they will not)

        [Test]
        public void TestReadXmlString()
        {
            XmlSerializer x = new XmlSerializer(typeof(FluentObject));
            dynamic o;

            const string original = "<?xml version=\"1.0\"?>\r\n<FluentObject test=\"asdf\" />";
            byte[] buffer = Encoding.UTF8.GetBytes(original);
            using (Stream stream = new MemoryStream(buffer))
            {
                o = x.Deserialize(stream);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                FluentObject fo = (FluentObject)o;
                x.Serialize(stream, fo);
                string reserialized = GetUtf8FromMemoryStream(stream);
                Assert.AreEqual(original, reserialized);
            }
        }

        [Test]
        public void TestToXmlString()
        {
            dynamic o = new FluentObject();

            o.test = "asdf";

            string output = ((FluentObject)o).ToXmlString("CustomName");
            
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?><CustomName test=\"asdf\" />", output);
        }

        private string GetUtf8FromMemoryStream(MemoryStream stream)
        {
            return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Position);
        }
    }
}
