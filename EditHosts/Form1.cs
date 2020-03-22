using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace EditHosts
{
    public partial class EditHosts : Form
    {
        public EditHosts()
        {
            InitializeComponent();

            if(!this.loadHostsFile())
            {
                MessageBox.Show("Die Hosts-Datei konnte nicht geladen werden!", "Fehler beim Auslesen", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.close();
            }
        }

        protected bool loadHostsFile()
        {
            string hostsFile = Environment.SystemDirectory + "\\drivers\\etc\\hosts";
            string tempFile = Path.GetTempPath() + "EditHosts.tmp";

            // Check if the file exists
            if (!File.Exists(hostsFile))
            {
                return false;
            }

            // Try to load the content of the file
            string hostsContent = File.ReadAllText(hostsFile);

            if(String.IsNullOrEmpty(hostsContent))
            {
                return false;
            }

            // Make now a list of string
            List<string> hostsContentLineElements = hostsContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();

            // Build the xml file
            XmlTextWriter xmlTextWriter = new XmlTextWriter(tempFile, Encoding.UTF8);
            xmlTextWriter.WriteStartDocument(true);
            xmlTextWriter.Formatting = Formatting.Indented;
            xmlTextWriter.Indentation = 2;
            xmlTextWriter.WriteStartElement("Data");

            // Lopp through the lines
            for(int i = 0; i < hostsContentLineElements.Count; i++)
            {
                // Check if the line has a string
                if(String.IsNullOrEmpty(hostsContentLineElements[i]))
                {
                    this.createNodeComment("", xmlTextWriter);
                    continue;
                }

                // Check if the line is a comment
                if(hostsContentLineElements[i].Substring(0,1) == "#")
                {
                    this.createNodeComment(hostsContentLineElements[i], xmlTextWriter);
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

        public void close()
        {
            Environment.Exit(0);
        }
    }
}
