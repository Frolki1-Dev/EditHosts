using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace EditHosts
{
    class XmlProcessor
    {
        public bool buildXmlFile(string xmlFile, string fileToParse)
        {
            // Check if the file exists
            if (!File.Exists(fileToParse))
            {
                MessageBox.Show("Hosts-Datei nicht gefunden", "Datei nicht gefunden", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Try to load the content of the file
            string hostsContent = File.ReadAllText(fileToParse);

            if (String.IsNullOrEmpty(hostsContent))
            {
                MessageBox.Show("Hosts-Datei ist leer", "Kein Inhalt", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            // Make now a list of string
            List<string> hostsContentLineElements = hostsContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();

            // Build the xml file
            XmlTextWriter xmlTextWriter = new XmlTextWriter(xmlFile, Encoding.UTF8);
            xmlTextWriter.WriteStartDocument(true);
            xmlTextWriter.Formatting = Formatting.Indented;
            xmlTextWriter.Indentation = 2;
            xmlTextWriter.WriteStartElement("Data");

            // Lopp through the lines
            for (int i = 0; i < hostsContentLineElements.Count; i++)
            {
                // Check if the line has a string
                if (String.IsNullOrEmpty(hostsContentLineElements[i]))
                {
                    this.createNodeComment("#", xmlTextWriter);
                    continue;
                }

                // Check if the line is a comment
                if (hostsContentLineElements[i].Substring(0, 1) == "#")
                {
                    // Check if the sting is only the my infromation
                    if (!hostsContentLineElements[i].Contains("EditHosts"))
                    {
                        this.createNodeComment(hostsContentLineElements[i], xmlTextWriter);
                    }
                    continue;
                }

                // Otherwise split now after spaces and tabs
                string[] currentLineContent = hostsContentLineElements[i].Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

                // And now add this to the node
                this.createNodeEntity(currentLineContent[0], currentLineContent[1], xmlTextWriter);
            }

            // Finish the xml file
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndDocument();
            xmlTextWriter.Close();

            // Done
            return true;
        }

        private void createNodeEntity(string ip, string host, XmlTextWriter writer)
        {
            writer.WriteStartElement("Entity");

            writer.WriteStartElement("IP");
            writer.WriteString(ip);
            writer.WriteEndElement();

            writer.WriteStartElement("Host");
            writer.WriteString(host);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        private void createNodeComment(string comment, XmlTextWriter writer)
        {
            writer.WriteStartElement("Comment");

            writer.WriteStartElement("Description");
            writer.WriteString(comment);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}
