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
        private string tempFile = Path.GetTempPath() + "EditHosts.tmp";

        public EditHosts()
        {
            InitializeComponent();

            XmlProcessor xmlProcessor = new XmlProcessor();

            string hostsFile = Environment.SystemDirectory + "\\drivers\\etc\\hosts";

            if (!xmlProcessor.buildXmlFile(this.tempFile, hostsFile))
            {
                this.close();
            }

            // Load the xml to the data viewer
            DataSet database = new DataSet();
            database.ReadXml(this.tempFile);
            this.dataGridView1.DataSource = database.Tables[1];
        }

        public void close()
        {
            Environment.Exit(0);
        }

        private void hostsDateiNeuEinlesenToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
