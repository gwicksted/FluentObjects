using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FluentObjects
{
    public partial class FluentObject : IXmlSerializable 
    {
        public XmlSchema GetSchema()
        {
            return null; // sufficient
        }

        // TODO: handle arrays
        public string ToXmlString(string name)
        {
            StringBuilder stringBuilder = new StringBuilder();

            // TODO: specify encoding as UTF-8

            using (TextWriter textWriter = new StringWriter(stringBuilder))
            {
                using (TextWriter indentedTextWriter = new IndentedTextWriter(textWriter, "  "))
                {
                    using (XmlWriter writer = new XmlTextWriter(indentedTextWriter))
                    {
                        writer.WriteStartDocument();

                        WriteXmlElement(writer, this, name);

                        writer.WriteEndDocument();
                        writer.Flush();
                    }
                }
            }

            return stringBuilder.ToString();
        }

        // TODO: handle arrays
        public void WriteXml(XmlWriter writer)
        {
            WriteXmlElement(writer, this, null);
        }

        private void WriteXmlElement(XmlWriter writer, FluentObject elementFo, string name)
        {
            if (name != null)
            {
                writer.WriteStartElement(name);
            }

            // TODO: should test to see if is native datatype (struct) vs object -- if object, it is element
            var xmlAttributes = (from a in elementFo._attributes where !(a.Value is FluentObject) select new KeyValuePair<string, string>(a.Key, a.Value.ToString()));
            WriteXmlAttributes(writer, xmlAttributes);
            // TODO: selectmany
            // TODO: dictionary keys may not be valid xml attribute names
            xmlAttributes = (from a in elementFo._dictionaryElements where !(a.Value is FluentObject) select new KeyValuePair<string, string>(a.Key.ToString(), a.Value.ToString()));
            WriteXmlAttributes(writer, xmlAttributes);

            if (name != null)
            {
                //writer.WriteEndElement();
            }

            bool hasElements = false;

            var xmlElements = (from a in elementFo._attributes where a.Value is FluentObject select a);
            foreach (var xmlElement in xmlElements)
            {
                // TODO: fix
                hasElements = true;
                WriteXmlElement(writer, xmlElement.Value, xmlElement.Key);
            }
            // TODO: selectmany
            xmlElements = (from a in elementFo._dictionaryElements where a.Value is FluentObject && a.Key is int select new KeyValuePair<string, dynamic>(name ?? "unnamed", a.Value));
            foreach (var xmlElement in xmlElements)
            {
                // TODO: if dictionary["abcd"], use key and value? (some dictionary keys may not be valid xml
                hasElements = true; // TODO: fix
                WriteXmlElement(writer, xmlElement.Value, xmlElement.Key);
            }
            // TODO: selectmany
            xmlElements = (from a in elementFo._dictionaryElements where a.Value is FluentObject && !(a.Key is int) select new KeyValuePair<string, dynamic>(a.Key.ToString(), a.Value));
            foreach (var xmlElement in xmlElements)
            {
                // TODO: if dictionary["abcd"], use key and value? (some dictionary keys may not be valid xml
                hasElements = true; // TODO: fix
                WriteXmlElement(writer, xmlElement.Value, xmlElement.Key);
            }

            if (name != null)
            {
                if (hasElements)
                {
                    writer.WriteFullEndElement();
                }
                else
                {
                    writer.WriteEndElement();
                }
            }
        }

        private static void WriteXmlAttributes(XmlWriter writer, IEnumerable<KeyValuePair<string, string>> attributes)
        {
            foreach (var attribute in attributes)
            {
                writer.WriteAttributeString(attribute.Key, attribute.Value);
            }
        }

        public void ReadXml(XmlReader reader)
        {
            bool first = true;
            //reader.ReadStartElement();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                // first name will always be FluentObject if WriteXml was used
                if (first && reader.Name == "FluentObject")
                {
                    first = false;
                }
                else
                {
                    // TODO: check for existence, convert to array
                    _dictionaryElements.Add(reader.Name, new FluentObject());
                }

                if (reader.HasAttributes)
                {
                    while (reader.MoveToNextAttribute())
                    {
                        _attributes.Add(reader.Name, reader.Value);
                    }
                }

                reader.MoveToContent();
                //_dictionaryElements.Add(entity.Key, entity);

                if (!reader.Read())
                {
                    break;
                }
            }

            //reader.ReadEndElement();
        }
    }
}
