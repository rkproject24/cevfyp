using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace ClassLibrary
{
    public class xml
    {
        private string xmlFile;
        private string node;

        public xml(string fileName, string group, bool rebuild)
        {
            this.xmlFile = fileName + ".xml";
            this.node = group;

            if (File.Exists(xmlFile) && rebuild)
            {
                File.Delete(xmlFile);
            }
            
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                try
                {
                    xmlDoc.Load(xmlFile);
                }
                catch
                {
                    XmlTextWriter xmlWrite = new XmlTextWriter(xmlFile, System.Text.Encoding.UTF8);
                    xmlWrite.Formatting = Formatting.Indented;
                    xmlWrite.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                    xmlWrite.WriteStartElement(node);
                    xmlWrite.Close();
                    xmlDoc.Load(xmlFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void AddAttribute(string group, string type, string value)
        {
            XmlDocument setting = new XmlDocument();
            setting.Load(this.xmlFile);


            //XmlNode node = setting.SelectSingleNode(group);
            XmlAttribute xmlattribute_add = setting.CreateAttribute(type);
            xmlattribute_add.Value = value;
            //node.Attributes.Append(xmlattribute_add);
            setting.SelectSingleNode(group).Attributes.Append(xmlattribute_add);
            setting.Save(this.xmlFile);
        }


        //public void Add(string list, string type, string value)
        //{
        //    XmlDocument xmlDoc = new XmlDocument();
        //    xmlDoc.Load(this.xmlFile);
        //    XmlElement childNode;


        //    XmlNode root = xmlDoc.DocumentElement;
        //    if ((childNode = (XmlElement)xmlDoc.SelectSingleNode(list)) == null)
        //    {
        //        childNode = xmlDoc.CreateElement(list);
        //    }
           
        //        XmlElement childNode2 = xmlDoc.CreateElement(type);
        //        XmlText textNode = xmlDoc.CreateTextNode(value);
        //        //textNode.Value = value;

        //        root.AppendChild(childNode);
        //        childNode.AppendChild(childNode2);
        //        //childNode.SetAttribute("Name", type[i]);
        //        childNode2.AppendChild(textNode);

        //        //textNode.Value = "replacing hello world";

        //        //XmlAttribute xmlattribute_add = setting.CreateAttribute(type[i]);
        //        //xmlattribute_add.Value = value[i];
        //        //xmlnode_add.Attributes.Append(xmlattribute_add);
            

        //    xmlDoc.Save(this.xmlFile);

        //}

        //by vinci: attribute=NULL if there is no attributes in the node
        public void Add(string list, string []type, string []value, string []attributeName, string[] attributevalues)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(this.xmlFile);
            XmlElement childNode;
            XmlElement root = xmlDoc.DocumentElement;

            //try
            //{
            //    //childNode = (XmlElement)xmlDoc.SelectSingleNode(list);
            //    childNode = (XmlElement)root.SelectSingleNode(list);
            //}
            //catch
            //{
            //    childNode = xmlDoc.CreateElement(list);
            //}

            childNode = xmlDoc.CreateElement(list);

            //by vinci
            for (int i = 0; i < attributeName.Length; i++)
            {
                XmlAttribute attribute = xmlDoc.CreateAttribute(attributeName[i]);
                attribute.Value = attributevalues[i];
                childNode.SetAttributeNode(attribute);
            }

            for (int i = 0; i < type.Length; i++)
            {

                //by vinci:
                try
                {
                    XmlElement subNode = xmlDoc.CreateElement(type[i]);
                    subNode.InnerText = value[i];

                    root.AppendChild(childNode);
                    childNode.AppendChild((XmlNode) subNode);
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }
            
            xmlDoc.Save(this.xmlFile);
        }


        public void Add(string[] type, string[] value)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(this.xmlFile);


            XmlNode childNode = xmlDoc.DocumentElement;

            for (int i = 0; i < type.Length; i++)
            {
                XmlElement childNode2 = xmlDoc.CreateElement(type[i]);
                XmlText textNode = xmlDoc.CreateTextNode(value[i]);
                //textNode.Value = value;

                childNode.AppendChild(childNode2);
                //childNode.SetAttribute("Name", type[i]);
                childNode2.AppendChild(textNode);
            }

            xmlDoc.Save(this.xmlFile);

        }
          

        public string ReadAttribute(string group, string type)
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



         public string Read(string group, string type)
         {
             XmlDocument doc = new XmlDocument();
             doc.Load(this.xmlFile);
             XmlElement root = doc.DocumentElement;
             //XmlNodeList nodes = root.SelectNodes("/"+group);
             //XmlNodeList nodes = root.GetElementsByTagName(group);

                 string value;
                 try
                 {
                     if (root.LocalName == group)
                     {
                        foreach (XmlElement G in root)
                        {
                             if ((G.LocalName) == type)
                             {
                                 value = G.InnerText;
                                 return value;
                             }
                         }
                     }
                     else
                     {
                         foreach (XmlElement G in root)
                         {
                             if (G.LocalName == group)
                             {
                                 foreach (XmlElement T in G)
                                 {
                                     if ((T.LocalName) == type)
                                     {
                                         value = T.InnerText;
                                         return value;
                                     }
                                 }
                             }
                         }
                     }
                 
                 }
                 catch
                 {
                     //MessageBox.Show("Reading Error! Please input both group and type!");
                     return "error";
                 }
                 return "";
          }

         public string Read(string nodeName, string attributeName, string attributeValue, string type)
         {
             XmlDocument doc = new XmlDocument();
             doc.Load(this.xmlFile);
             XmlElement root = doc.DocumentElement;
             //XmlNodeList nodes = root.SelectNodes("/"+group);
             //XmlNodeList nodes = root.GetElementsByTagName(group);

             string value;
             try
             {
                 if (root.LocalName == nodeName)
                 {
                     foreach (XmlElement G in root)
                     {
                         if ((G.LocalName) == type)
                         {
                             value = G.InnerText;
                             return value;
                         }
                     }
                 }
                 else
                 {
                     foreach (XmlElement G in root)
                     {
                         if (G.LocalName == nodeName)
                         {
                             bool matchNode = false;
                             XmlAttributeCollection attributes = G.Attributes;
                             foreach (XmlAttribute attri in attributes)
                             {
                                 if (attri.Name.Equals(attributeName) && attri.InnerText.Equals(attributeValue))
                                     matchNode = true;
                             }

                             if (matchNode)
                             {
                                 foreach (XmlElement T in G)
                                 {
                                     if ((T.LocalName) == type)
                                     {
                                         value = T.InnerText;
                                         return value;
                                     }
                                 }
                             }

                         }
                     }
                 }

             }
             catch(Exception ex)
             {
                 //MessageBox.Show("Reading Error! Please input both group and type!");
                 //return "error";
                 return ex.ToString();
             }
             return "";
         }
        /*
         public string Read(string [] Location)
         {
             XmlDocument doc = new XmlDocument();
             doc.Load(this.xmlFile);
             XmlElement root = doc.DocumentElement;
         
             string value = null;
             try
             {
                 value = XmlNavigator(root, Location);
             }
             catch
             {
                 MessageBox.Show("Reading Error! Please input both group and type!");
                 return "error";
             }
             return "";
         }

         public string XmlNavigator(XmlElement element, string [] check)
         {
             if (location.GetLength == 1)
             {
                 if (element.LocalName == check[0])
                     return element.InnerText;
                 else
                     return "";
             }
             else
             {
 
             }
         }
        */

         public int GetElementNum()
         {
             XmlDocument GetELement = new XmlDocument();
             GetELement.Load(this.xmlFile);
             XmlElement root = GetELement.DocumentElement;

             int EleNum = root.ChildNodes.Count;

             return EleNum;
         }



        public void modify(string group, string type, string newValue)
        {
            XmlDocument modify = new XmlDocument();
            modify.Load(this.xmlFile);
            //XmlAttribute attribute = modify.SelectSingleNode(type).Attributes[type];
            XmlElement root = modify.DocumentElement;

           // foreach (XmlElement 
            
            //attribute.Value = newValue;
            modify.Save(this.xmlFile);
        }

        //by vinci
        public void modifyAttribute(string node, string attributeName, string newValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(this.xmlFile);
            XmlAttribute attribute = xmlDoc.SelectSingleNode(node).Attributes[attributeName];
            //xmlDoc.SelectSingleNode(node).Attributes[attributeName] = newValue;
            attribute.Value = newValue;
            //xmlDoc.RemoveChild(attribute);
            //xmlDoc.AppendChild(attribute);
            xmlDoc.Save(this.xmlFile);
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
