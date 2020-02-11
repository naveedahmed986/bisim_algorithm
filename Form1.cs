using System;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace Bisimulation_Desktop
{
    public partial class Form1 : Form
    {
        string filePath { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void parse_Button_Click(object sender, EventArgs e)
        {
            printXML();
        }

        protected void printXML()
        {
            richTextBox1.Clear();
            if (!string.IsNullOrEmpty(filePath))
            {
                XmlTextReader reader = new XmlTextReader(filePath);
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element: // The node is an element.

                            this.richTextBox1.SelectionColor = Color.Blue;

                            this.richTextBox1.AppendText("<");

                            this.richTextBox1.SelectionColor = Color.Brown;

                            this.richTextBox1.AppendText(reader.Name);

                            this.richTextBox1.SelectionColor = Color.Green;

                            while (reader.MoveToNextAttribute())
                                richTextBox1.AppendText(" " + reader.Name + "='" + reader.Value + "'");

                            this.richTextBox1.SelectionColor = Color.Blue;

                            this.richTextBox1.AppendText(">");

                            break;

                        case XmlNodeType.Text: //Display the text in each element.

                            this.richTextBox1.SelectionColor = Color.Black;

                            this.richTextBox1.AppendText(reader.Value);

                            break;

                        case XmlNodeType.EndElement: //Display the end of the element.

                            this.richTextBox1.SelectionColor = Color.Blue;

                            this.richTextBox1.AppendText("</");

                            this.richTextBox1.SelectionColor = Color.Brown;

                            this.richTextBox1.AppendText(reader.Name);

                            this.richTextBox1.SelectionColor = Color.Blue;

                            this.richTextBox1.AppendText(">");

                            this.richTextBox1.AppendText("\n");

                            break;
                    }
                }
            }
            else
            {
                pathLabel.ForeColor = Color.Red;
                pathLabel.Text = "File not found. Please open a file (.xml)";
            }
        }

        private void browse_Button_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.InitialDirectory = @System.Environment.CurrentDirectory;
            ofd.Filter = "xml files (*.xml)|*.xml";
            ofd.FilterIndex = 1;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if(ofd.CheckPathExists && ofd.CheckFileExists)
                {
                    if (!string.IsNullOrEmpty(ofd.FileName))
                    {
                        pathLabel.ForeColor = Color.Black;
                        pathLabel.Text = ofd.FileName;
                        filePath = ofd.FileName;
                    }
                }
            }
        }

        private void model_transform_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    Nta nta;
                    var serializer = new XmlSerializer(typeof(Nta));
                    StreamReader file = new StreamReader(filePath);
                    nta = (Nta)serializer.Deserialize(file);
                    Dictionary<string, Channel> channels = new Dictionary<string, Channel>();
                    if (nta != null)
                    {
                        foreach (Template template in nta.Template) // Loop over all templates
                        {
                            Channel channelInfo = new Channel();
                            for (int i = 0; i < template.Transition.Count; i++) // Loop over all transitions in each template
                            {
                                for (int j = 0; j < template.Transition[i].Label.Count; j++) // Loop over all labels in each transition
                                {
                                    if (template.Transition[i].Label[j].Kind == "synchronisation")
                                    {
                                        string channelName = template.Transition[i].Label[j].Text;
                                        if (!string.IsNullOrEmpty(channelName) && !string.IsNullOrWhiteSpace(channelName))
                                        {
                                            channelName = channelName.Substring(0, channelName.Length - 1);
                                            if (channels.ContainsKey(channelName))
                                            {
                                                channelInfo = channels[channelName];
                                                channelInfo.AddChannelInfo(template.Name.Text, isChannelBroadcaster(template.Transition[i].Label[j].Text),
                                                    template.Transition[i].Source.Ref, template.Transition[i].Target.Ref);
                                            }
                                            else
                                            {
                                                channelInfo.AddChannelInfo(template.Name.Text, isChannelBroadcaster(template.Transition[i].Label[j].Text),
                                                    template.Transition[i].Source.Ref, template.Transition[i].Target.Ref);
                                                channels.Add(channelName, channelInfo);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    file.Close();

                    if (channels.Count > 0)
                    {

                        foreach (var ch in channels)
                        {
                            richTextBox1.AppendText("Channel : " + ch.Key + "\n\n");
                            richTextBox1.AppendText("TemplateName\t\tIsBroadcaster\t\tSourceId\t\tTargetId\n");
                            if (ch.Value.GetChannels().Count > 0)
                            {
                                foreach (var channelInfo in ch.Value.GetChannels())
                                {
                                    richTextBox1.AppendText(channelInfo.Item1 + "\t\t\t");
                                    richTextBox1.AppendText(channelInfo.Item2 + "\t\t\t");
                                    richTextBox1.AppendText(channelInfo.Item3 + "\t\t");
                                    richTextBox1.AppendText(channelInfo.Item4 + "\n\n");
                                }
                            }
                        }
                    }
                    else
                        richTextBox1.Text = "No channels found in the model";
                }
                else
                {
                    pathLabel.ForeColor = Color.Red;
                    pathLabel.Text = "File not found. Please open a file (.xml)";
                }
            }
            catch(Exception ex)
            {
                richTextBox1.ForeColor = Color.Red;
                richTextBox1.Text = ex.Message;
                richTextBox1.ForeColor = Color.Black;
            }
        }

        private bool isChannelBroadcaster(string channelName)
        {
            if (channelName.LastIndexOf('!') == channelName.Length-1)
                return true;
            return false;
        }
    }
}
