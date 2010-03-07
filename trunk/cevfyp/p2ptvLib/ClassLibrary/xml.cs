using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Windows.Forms;


namespace ClassLibrary
{
    public class xml
    {
        private string xmlFile;
        private string node;
        private XmlDocument xmlDoc;

        public xml(string fileName, string group, bool rebuild)
        {
            this.xmlFile = fileName + ".xml";
            this.node = group;

            if (File.Exists(xmlFile) && rebuild)
                File.Delete(xmlFile);

            if (rebuild || !File.Exists(xmlFile))
            {
                try
                {
                    XmlTextWriter xmlWrite = new XmlTextWriter(xmlFile, System.Text.Encoding.UTF8);
                    xmlWrite.Formatting = Formatting.Indented;
                    xmlWrite.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                    xmlWrite.WriteStartElement(node);
                    xmlWrite.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("xml:"+ex.ToString());
                }
            }

            

        }


        public void AddAttribute(string group, string type, string value)
        {
            try
            {
                xmlDoc = new XmlDocument();
                //XmlNode node = setting.SelectSingleNode(group);
                xmlDoc.Load(this.xmlFile);
                XmlAttribute xmlattribute_add = xmlDoc.CreateAttribute(type);
                xmlattribute_add.Value = value;
                //node.Attributes.Append(xmlattribute_add);

                xmlDoc.SelectSingleNode(group).Attributes.Append(xmlattribute_add);
                xmlDoc.Save(this.xmlFile);
            }
            catch(Exception ex)
            {
                MessageBox.Show("AddAttribute:"+ex.ToString());
            }

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
            try
            {
                xmlDoc = new XmlDocument();

                XmlElement childNode;
                xmlDoc.Load(this.xmlFile);
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
                        childNode.AppendChild((XmlNode)subNode);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Add_inside:" + ex.ToString());
                    }

                }

                xmlDoc.Save(this.xmlFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Add_outside:" + ex.ToString());
            }
        }


        public void Add(string[] type, string[] value)
        {
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(this.xmlFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

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
               // xmlDoc = new XmlDocument();
               // xmlDoc.Load(this.xmlFile);
                try
                {
                    value = xmlDoc.SelectSingleNode(group).Attributes[type].Value;
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
             //xmlDoc = new XmlDocument();
             //xmlDoc.Load(this.xmlFile);
             XmlElement root;// = xmlDoc.DocumentElement;
             //XmlNodeList nodes = root.SelectNodes("/"+group);
             //XmlNodeList nodes = root.GetElementsByTagName(group);

                 string value;
                 try
                 {
                     root = xmlDoc.DocumentElement;

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
            // xmlDoc = new XmlDocument();
            // xmlDoc.Load(this.xmlFile);
             XmlElement root;// = xmlDoc.DocumentElement;
             //XmlNodeList nodes = root.SelectNodes("/"+group);
             //XmlNodeList nodes = root.GetElementsByTagName(group);

             string value;
             try
             {
                 root = xmlDoc.DocumentElement;

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

         public string ReadRandom(string nodeName, string attributeName)
         {
             // xmlDoc = new XmlDocument();
             // xmlDoc.Load(this.xmlFile);
             XmlElement root;// = xmlDoc.DocumentElement;
             //XmlNodeList nodes = root.SelectNodes("/"+group);
             //XmlNodeList nodes = root.GetElementsByTagName(group);

             try
             {
                 root = xmlDoc.DocumentElement;

                 XmlNode G = root.ChildNodes[ RandomNumber(0, root.ChildNodes.Count)];
                 XmlAttributeCollection attributes = G.Attributes;
                 foreach (XmlAttribute attri in attributes)
                 {
                     if (attri.Name.Equals(attributeName))
                         return attri.InnerText;
                 }
             }
             catch (Exception ex)
             {
                 //MessageBox.Show("Reading Error! Please input both group and type!");
                 //return "error";
                 return ex.ToString();
             }
             return "";
         }

         public string ReadByIndex(string nodeName, string attributeName,int index)
         {
             // xmlDoc = new XmlDocument();
             // xmlDoc.Load(this.xmlFile);
             XmlElement root;// = xmlDoc.DocumentElement;
             //XmlNodeList nodes = root.SelectNodes("/"+group);
             //XmlNodeList nodes = root.GetElementsByTagName(group);

             try
             {
                 root = xmlDoc.DocumentElement;

                 XmlNode G = root.ChildNodes[index];
                 XmlAttributeCollection attributes = G.Attributes;
                 foreach (XmlAttribute attri in attributes)
                 {
                     if (attri.Name.Equals(attributeName))
                         return attri.InnerText;
                 }
             }
             catch (Exception ex)
             {
                 //MessageBox.Show("Reading Error! Please input both group and type!");
                 //return "error";
                 return "";
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
             try
             {
                 xmlDoc = new XmlDocument();
                 xmlDoc.Load(this.xmlFile);
                 XmlElement root = xmlDoc.DocumentElement;

                 int EleNum = root.ChildNodes.Count;

                 return EleNum;
             }
             catch (Exception ex)
             {
                 MessageBox.Show("GetElementNum:"+ ex.ToString());
                 return -1;
             }
         }



        public void modify(string group, string type, string newValue)
        {
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(this.xmlFile);
                //XmlAttribute attribute = modify.SelectSingleNode(type).Attributes[type];
                XmlElement root = xmlDoc.DocumentElement;

                // foreach (XmlElement 

                //attribute.Value = newValue;
                xmlDoc.Save(this.xmlFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("modify:" + ex.ToString());
                 
            }
        }

        //by vinci
        public void modifyAttribute(string node, string attributeName, string newValue)
        {
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(this.xmlFile);
                XmlAttribute attribute = xmlDoc.SelectSingleNode(node).Attributes[attributeName];
                //xmlDoc.SelectSingleNode(node).Attributes[attributeName] = newValue;
                attribute.Value = newValue;
                //xmlDoc.RemoveChild(attribute);
                //xmlDoc.AppendChild(attribute);
                xmlDoc.Save(this.xmlFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("modifyAttribute:" + ex.ToString());
                 
            }

        }

        public void deleteAttribute(string group, string type)
        {
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(this.xmlFile);
                XmlAttribute attribute = xmlDoc.SelectSingleNode(group).Attributes[type];
                attribute.ParentNode.RemoveChild(attribute);
                xmlDoc.Save(this.xmlFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("deleteAttribute:" + ex.ToString());
            }

        }

        public void deleteNode(string group)
        {
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(this.xmlFile);
                XmlNode node = xmlDoc.SelectSingleNode(group);
                node.ParentNode.RemoveChild(node);
                xmlDoc.Save(this.xmlFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("deleteNode:" + ex.ToString());
            }
        }

        public bool deleteInnerNode(string nodeName, string attributeName, string attributeValue)
        {
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(this.xmlFile);
                XmlElement root = xmlDoc.DocumentElement;
                //XmlNodeList nodes = root.SelectNodes("/"+group);
                //XmlNodeList nodes = root.GetElementsByTagName(group);



                foreach (XmlElement G in root)
                {
                    if ((G.LocalName) == nodeName)
                    {
                        XmlAttributeCollection attributes = G.Attributes;
                        foreach (XmlAttribute attri in attributes)
                        {
                            if (attri.Name.Equals(attributeName) && attri.InnerText.Equals(attributeValue))
                            {
                                root.RemoveChild(G);
                                xmlDoc.Save(this.xmlFile);
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Reading Error! Please input both group and type!");
                //return "error";
                //return ex.ToString();
                return false;
            }
            return false;
        }


        //public void save()
        //{
        //    xmlDoc.Save(this.xmlFile);
        //}

        public bool load()
        {
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(this.xmlFile);
                
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("load:"+ex.ToString());
               return false;
            }
        }

        public bool sortLoad( string nodeString, string sortnode)
        {
            try
            {
                XPathDocument xmldoc = new System.Xml.XPath.XPathDocument(this.xmlFile);
                XPathNavigator nav = xmldoc.CreateNavigator();

                // Compile the XPath query expression to select all the Title elements.
                // The Compile method of the XPathNavigator generates an XPathExpression 
                // object that encapsulates the compiled query.
                System.Xml.XPath.XPathExpression expr = nav.Compile(nodeString);

                // Execute the AddSort method of the XPathExpression object to define the 
                // Title Element as the sort key.
                expr.AddSort(sortnode, System.Xml.XPath.XmlSortOrder.Ascending, System.Xml.XPath.XmlCaseOrder.None, "", System.Xml.XPath.XmlDataType.Number);


                // Create the XPathNodeIterator by executing the Select method of the 
                // XPathNavigator. Notice that the XPathExpression object is supplied as 
                // the query expression parameter.
                XPathNodeIterator iterator = nav.Select(expr);

                string xmlString = "";

                XmlElement root = xmlDoc.DocumentElement;
                root.InnerXml = "";
                // Use the iterator to explore the result set that is generated.
                while (iterator.MoveNext())
                {
                    //System.Diagnostics.Debug.WriteLine(iterator.Current.InnerXml);
                    xmlString += iterator.Current.OuterXml;
                }
                root.InnerXml = xmlString;

                xmlDoc.Save(this.xmlFile);
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("load:"+ex.ToString());
               return false;
            }
        }

        public bool hasNode()
        {
            try
            {
                XmlElement root = xmlDoc.DocumentElement;
                return root.HasChildNodes;
            }
            catch
            {
                return false;
            }
        }
        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
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
