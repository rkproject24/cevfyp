using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace ClassLibrary
{
    class xml
    {
        private string xmlFile;
        private string node;

        public xml(string fileName, string group)
        {
            this.xmlFile = fileName;
            this.node = group;
            if (!(File.Exists(xmlFile)))
            {
                XmlDocument setting = new XmlDocument();
                setting.AppendChild(setting.CreateXmlDeclaration("1.0", "UTF-8", ""));
                XmlNode xmlnode_core = setting.CreateNode(XmlNodeType.Element, group, "");
                setting.AppendChild(xmlnode_core);
                setting.Save(fileName);
            }
        }

        public void Add(string group, string type, string value)
        {
            XmlDocument setting = new XmlDocument();
            setting.Load(this.xmlFile);

            /*
            for (int i = 0; i < type.Length; i++)
            {
                XmlAttribute xmlattribute_add = setting.CreateAttribute(type[i]);
                xmlattribute_add.Value = value[i];
                xmlnode_add.Attributes.Append(xmlattribute_add);
            }*/

            //XmlNode node = setting.SelectSingleNode(group);
            XmlAttribute xmlattribute_add = setting.CreateAttribute(type);
            xmlattribute_add.Value = value;
            //node.Attributes.Append(xmlattribute_add);
            setting.SelectSingleNode(group).Attributes.Append(xmlattribute_add);
            setting.Save(this.xmlFile);
        }


        public string Read(string group, string type)
        {
                string value;
                XmlDocument read = new XmlDocument();
                read.Load(this.xmlFile);
                try
                {
                    value = read.SelectSingleNode(group).Attributes[type].Value;
                    return value;
                }
                catch
                {
                    //MessageBox.Show("Reading Error! Please input both group and type!");
                    return "";
                }
         }

        public void modify(string group, string type, string newValue)
        {
            XmlDocument modify = new XmlDocument();
            modify.Load(this.xmlFile);
            XmlAttribute attribute = modify.SelectSingleNode(type).Attributes[type];
            attribute.Value = newValue;
            modify.Save(this.xmlFile);
        }

        public void deleteAttribute(string group, string type)
        {
            XmlDocument delete = new XmlDocument();
            delete.Load(this.xmlFile);
            XmlAttribute attribute = delete.SelectSingleNode(group).Attributes[type];
            attribute.ParentNode.RemoveChild(attribute);
            delete.Save(this.xmlFile);
        }

        public void deleteNode(string group)
        {
            XmlDocument delete = new XmlDocument();
            delete.Load(this.xmlFile);
            XmlNode node = delete.SelectSingleNode(group);
            node.ParentNode.RemoveChild(node);
            delete.Save(this.xmlFile);
        }

      }

    public static class SiteHelper
    {
        public static string Serialize(object o)
        {
            XmlSerializer ser = new XmlSerializer(o.GetType());
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            ser.Serialize(writer, o);
            return sb.ToString();
        }

        public static T Deserialize<T>(string s)
        {
            XmlDocument xdoc = new XmlDocument();

            try
            {
                xdoc.LoadXml(s);
                XmlNodeReader reader = new XmlNodeReader(xdoc.DocumentElement);
                XmlSerializer ser = new XmlSerializer(typeof(T));
                object obj = ser.Deserialize(reader);

                return (T)obj;
            }
            catch
            {
                return default(T);
            }
        }

    }  
        
}
