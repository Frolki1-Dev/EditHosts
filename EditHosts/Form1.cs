using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace EditHosts
{
    public partial class EditHosts : Form
    {
        private string tempFile = Path.GetTempPath() + "EditHosts.tmp";
        private string hostsFile = Environment.SystemDirectory + "\\drivers\\etc\\hosts";
        protected DataSet database = new DataSet();

        public EditHosts()
        {
            InitializeComponent();

            if (!IsRunAsAdministrator())
            {
                var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

                // The following properties run the new process as administrator
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";

                // Start the new process
                try
                {
                    Process.Start(processInfo);
                }
                catch (Exception)
                {
                    // The user did not allow the application to run as administrator
                    MessageBox.Show("Sorry, this application must be run as Administrator.");
                }

                // Shut down the current process
                this.close();
            }

            XmlProcessor xmlProcessor = new XmlProcessor();

            if (!xmlProcessor.buildXmlFile(this.tempFile, this.hostsFile))
            {
                this.close();
            }
        }

        public void close()
        {
            Environment.Exit(0);
        }

        private bool IsRunAsAdministrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);

            return wp.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void EditHosts_Load(object sender, EventArgs e)
        {
            // Load the xml to the data viewer
            this.database.ReadXml(this.tempFile);
            this.dataGridView1.DataSource = this.database.Tables[1];
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            using (XmlTextWriter xmlWrite = new XmlTextWriter(this.tempFile, Encoding.UTF8))
            {
                this.database.WriteXml(xmlWrite);
            }

            using (StreamWriter writer = new StreamWriter(this.hostsFile, false))
            {
                Boolean isComment = false;
                Boolean isHostEntity = false;
                Boolean hostEntitiyEnd = false;

                // Now build the hosts file new
                XmlTextReader xmlReader = new XmlTextReader(this.tempFile);
                while (xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            // Check what type is it
                            if (xmlReader.Name == "Entity")
                            {
                                isComment = false;
                                isHostEntity = true;
                                hostEntitiyEnd = false;
                                break;
                            }

                            if (xmlReader.Name == "Host")
                            {
                                hostEntitiyEnd = true;
                                break;
                            }

                            if (xmlReader.Name == "Comment")
                            {
                                isHostEntity = false;
                                isComment = true;
                                break;
                            }

                            break;
                        case XmlNodeType.Text:
                            if(isComment)
                            {
                                writer.WriteLine(xmlReader.Value);
                            }

                            if(isHostEntity)
                            {
                                if(hostEntitiyEnd)
                                {
                                    writer.WriteLine(xmlReader.Value);
                                } else
                                {
                                    writer.Write(xmlReader.Value + " ");
                                }
                            }

                            break;
                        default:
                            // Do nothing
                            break;
                    }
                }

                writer.Write("# Created by EditHosts (https://github.com/Frolki1-Dev/EditHosts) made by Frank Giger (https://frank-giger.ch) - Version " + System.Reflection.Assembly.GetEntryAssembly().GetName().Version);

                writer.Close();

                MessageBox.Show("Hosts-Datei wurde erfolgreich angepasst!\r\nDanke fürs verwenden. Programm schlisst sich.", "Erfolgreich", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.close();
            }
        }
    }
}
