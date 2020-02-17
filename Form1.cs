using System;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace Bisimulation_Desktop
{
    public partial class Form1 : Form
    {
        string filePath { get; set; }
        string filePath2 { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            parseSelection.Text = "Select model to parse";
            parseSelection.Items.Add("Parse first model");
            parseSelection.Items.Add("Parse Second model");
        }

        private void browse_Button_Click(object sender, EventArgs e)
        {
            filePath = getFilePathFromOFD();
            pathLabel.ForeColor = Color.Black;
            pathLabel.Text = filePath;
        }

        private void browse_button2_Click(object sender, EventArgs e)
        {
            filePath2 = getFilePathFromOFD();
            pathLabel2.ForeColor = Color.Black;
            pathLabel2.Text = filePath2;
        }

        private string getFilePathFromOFD()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.InitialDirectory = @System.Environment.CurrentDirectory;
            ofd.Filter = "xml files (*.xml)|*.xml";
            ofd.FilterIndex = 1;
            if (ofd.ShowDialog() == DialogResult.OK)
                if (ofd.CheckPathExists && ofd.CheckFileExists)
                    if (!string.IsNullOrEmpty(ofd.FileName))
                        return ofd.FileName;
            return "";
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
                    file.Close();

                    Channel channelInfo;
                    bool hasSelectLabel = false;

                    Dictionary<string, Channel> channels = new Dictionary<string, Channel>();
                    List<Location> locations = new List<Location>();

                    Dictionary<string, NdLocation> ndLocationList = new Dictionary<string, NdLocation>();
                    NdLocation ndLocations = new NdLocation();

                    Dictionary<string, NdTransition> ndTransitionList = new Dictionary<string, NdTransition>();
                    NdTransition ndTransitions;

                    if (nta != null)
                    {
                        foreach (Template template in nta.Template) // Loop over all templates
                        {
                            // List of all locations in the model
                            locations = AddLocations(locations, template);
                            //***********************************

                            // List of all non-deterministic locations in the models template wise
                            ndLocations = AddNdLocationsByTemplate(template);
                            if (ndLocations.getNdLocations().Count > 0)
                                ndLocationList.Add(template.Name.Text, ndLocations);
                            //***********************************

                            // List of all channels in a custom structure template wise
                            channelInfo = new Channel();
                            ndTransitions = new NdTransition();
                            hasSelectLabel = false;
                            for (int i = 0; i < template.Transition.Count; i++) // Loop over all transitions in each template
                            {
                                for (int j = 0; j < template.Transition[i].Label.Count; j++) // Loop over all labels in each transition
                                {
                                    if (template.Transition[i].Label[j].Kind == "select")
                                        hasSelectLabel = true;
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

                                // Add transitions with select kind lable in ndTransitions
                                if (hasSelectLabel)
                                    ndTransitions.AddNdTransition(template.Transition[i]);

                            }
                            if(ndTransitions.getNdTransitions().Count > 0)
                                ndTransitionList.Add(template.Name.Text, ndTransitions);
                            //**********************************************************
                        }
                    }

                    //if (channels.Count > 0)
                    //{

                    //    foreach (var ch in channels)
                    //    {
                    //        richTextBox1.AppendText("Channel : " + ch.Key + "\n\n");
                    //        richTextBox1.AppendText("TemplateName\t\tIsBroadcaster\t\tSourceId\t\tTargetId\n");
                    //        if (ch.Value.GetChannels().Count > 0)
                    //        {
                    //            foreach (var channelInfo in ch.Value.GetChannels())
                    //            {
                    //                richTextBox1.AppendText(channelInfo.Item1 + "\t\t\t");
                    //                richTextBox1.AppendText(channelInfo.Item2 + "\t\t\t");
                    //                richTextBox1.AppendText(channelInfo.Item3 + "\t\t");
                    //                richTextBox1.AppendText(channelInfo.Item4 + "\n\n");
                    //            }
                    //        }
                    //    }
                    //}
                    //else
                    //    richTextBox1.Text = "No channels found in the model";
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

        private List<Location> AddLocations(List<Location> list, Template template)
        {
            for (int x = 0; x < template.Location.Count; x++)
            {
                list.Add(template.Location[x]);
            }
            return list;
        }

        private NdLocation AddNdLocationsByTemplate(Template template)
        {
            NdLocation ndLocation = new NdLocation();
            int transitionSourceCount;
            foreach(Location location in template.Location)
            {
                transitionSourceCount = 0;
                foreach(Transition transition in template.Transition)
                {
                    if (string.Compare (transition.Source.Ref,location.Id) == 0)
                        transitionSourceCount++;
                }
                if (transitionSourceCount > 1)
                    ndLocation.AddNdLocation(location);
            }
            return ndLocation;
        }

        private void parseSelection_SelectedIndexChange(object sender, EventArgs e)
        {
            if (parseSelection.SelectedIndex == 0)
            {
                if (!string.IsNullOrEmpty(filePath))
                    printXML(filePath);
                else
                {
                    pathLabel.ForeColor = Color.Red;
                    pathLabel.Text = "File not found. Please open a file (.xml)";
                }
            }
            else if (parseSelection.SelectedIndex == 1)
            {
                if (!string.IsNullOrEmpty(filePath2))
                    printXML(filePath2);
                else
                {
                    pathLabel2.ForeColor = Color.Red;
                    pathLabel2.Text = "File not found. Please open a file (.xml)";
                }
            }
        }

        protected void printXML(string path)
        {
            richTextBox1.Clear();
            if (!string.IsNullOrEmpty(path))
            {
                XmlTextReader reader = new XmlTextReader(path);
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
        }
    }
}
