using System.CodeDom.Compiler;
using System.Collections.Generic;
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
            const string typicalXmlTabulation = "  ";

            StringBuilder stringBuilder = new StringBuilder();

            // TODO: specify encoding as UTF-8

            using (TextWriter textWriter = new StringWriter(stringBuilder))
            {
                using (TextWriter indentedTextWriter = new IndentedTextWriter(textWriter, typicalXmlTabulation))
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

        public void WriteXml(XmlWriter writer)
        {
            WriteXmlElement(writer, this, null);
        }

        private IEnumerable<KeyValuePair<string, string>> GetAttributes(FluentObject o)
        {
            // TODO: dictionary keys may not be valid xml attribute names
            // TODO: should test to see if is native datatype (struct) vs object -- if object, it is element
            return new[]
                {
                    from a in o._attributes
                    where !(a.Value is FluentObject)
                    select new KeyValuePair<string, string>(a.Key, a.Value.ToString()),

                    from a in o._dictionaryElements
                    where !(a.Value is FluentObject)
                    select new KeyValuePair<string, string>(a.Key.ToString(), a.Value.ToString())
                }.SelectMany(x => x);
        }

        private IEnumerable<KeyValuePair<string, dynamic>> GetElements(FluentObject o, string name)
        {
            return new[]
                {
                    from a in o._attributes 
                    where a.Value is FluentObject 
                    select a,
                    
                    from a in o._dictionaryElements
                    where a.Value is FluentObject && a.Key is int
                    select new KeyValuePair<string, dynamic>(name ?? "unnamed", a.Value),
                    
                    from a in o._dictionaryElements
                    where a.Value is FluentObject && !(a.Key is int)
                    select new KeyValuePair<string, dynamic>(a.Key.ToString(), a.Value)
                }.SelectMany(x => x);
        }

        private void WriteXmlElement(XmlWriter writer, FluentObject elementFo, string name)
        {
            if (name != null)
            {
                writer.WriteStartElement(name);
            }

            WriteXmlAttributes(writer, GetAttributes(elementFo));
            
            bool hasElements = false;

            var elements = GetElements(elementFo, name);
            foreach (var element in elements)
            {
                hasElements = true;
                WriteXmlElement(writer, element.Value, element.Key);
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
            Stack<FluentObject> stack = new Stack<FluentObject>();
            FluentObject current = this;

            bool first = true;

            while (true)
            {
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    current = stack.Pop();
                    if (!reader.Read())
                    {
                        break;
                    }
                }
                else
                {
                    var processed = new FluentObject();

                    if (first && reader.Name == "FluentObject")
                    {
                        first = false;
                        processed = this;
                    }
                    else
                    {
                        string name = reader.Name;

                        // Parent shares same name, access parent attribute's dictionary to create an array with the same name
                        if (stack.Peek()._attributes.ContainsKey(name))
                        {
                            var referenced = stack.Peek()._attributes[name];

                            int i = 0;
                            
                            while (referenced._dictionaryElements.ContainsKey(i))
                            {
                                i++;
                            }

                            referenced._dictionaryElements.Add(i, processed);
                        }
                        else
                        {
                            current._attributes.Add(name, processed);
                        }

                    }
                    
                    if (reader.HasAttributes)
                    {
                        while (reader.MoveToNextAttribute())
                        {
                            processed._attributes.Add(reader.Name, reader.Value);
                        }
                    }


                    reader.MoveToContent();
                    int depth = reader.Depth;

                    if (!reader.Read())
                    {
                        break;
                    }

                    if (reader.Depth > depth)
                    {
                        stack.Push(current);
                        current = processed;

                        reader.MoveToContent();
                    }
                }
            }
        }
    }
}
