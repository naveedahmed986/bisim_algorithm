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
            parseSelection.Items.Add("Parse First model");
            parseSelection.Items.Add("Parse Second model");
        }

        private void browse_Button_Click(object sender, EventArgs e)
        {
            filePath = GetFilePathFromOFD();
            pathLabel.ForeColor = Color.Black;
            pathLabel.Text = filePath;
        }

        private void browse_button2_Click(object sender, EventArgs e)
        {
            filePath2 = GetFilePathFromOFD();
            pathLabel2.ForeColor = Color.Black;
            pathLabel2.Text = filePath2;
        }

        private string GetFilePathFromOFD()
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
            if (string.IsNullOrEmpty(filePath))
            {
                pathLabel.ForeColor = Color.Red;
                pathLabel.Text = "File not found. Please open a file (.xml)";
            }
            else if (string.IsNullOrEmpty(filePath2))
            {
                pathLabel2.ForeColor = Color.Red;
                pathLabel2.Text = "File not found. Please open a file (.xml)";
            }
            else
                TransformModels();
        }

        private void TransformModels()
        {
            try
            {
                Nta model1 = NtaFromXML(filePath);
                Nta model2 = NtaFromXML(filePath2);

                model2 = RenameTemplatesInModel(model1, model2);
                model1 = MergeModels(model1, model2);
                //********** TODO : Remove ***********************

                //richTextBox1.Clear();
                //richTextBox1.AppendText("\n");
                //for (int x = 0; x < model1.Template.Count; x++)
                //{
                //    richTextBox1.AppendText(model1.Template[x].Name.Text + "\n");
                //}
                //************************************************


                Channel channelInfo;
                bool hasSelectLabel = false;

                Dictionary<string, Channel> channels = new Dictionary<string, Channel>();
                List<Location> locations = new List<Location>();

                Dictionary<string, NdLocation> ndLocationList = new Dictionary<string, NdLocation>();
                NdLocation ndLocations = new NdLocation();

                Dictionary<string, NdTransition> ndTransitionList = new Dictionary<string, NdTransition>();
                NdTransition ndTransitions;

                if (model1 != null)
                {
                    foreach (Template template in model1.Template) // Loop over all templates
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
                                            channelInfo.AddChannelInfo(template.Name.Text, IsChannelBroadcaster(template.Transition[i].Label[j].Text),
                                                template.Transition[i].Source.Ref, template.Transition[i].Target.Ref);
                                        }
                                        else
                                        {
                                            channelInfo.AddChannelInfo(template.Name.Text, IsChannelBroadcaster(template.Transition[i].Label[j].Text),
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
                        if (ndTransitions.getNdTransitions().Count > 0)
                            ndTransitionList.Add(template.Name.Text, ndTransitions);
                        //**********************************************************
                    }
                }

                //********** TODO : Remove ***********************
                //if (channels.Count > 0)
                //{
                //    richTextBox1.Clear();
                //    foreach (var ch in channels)
                //    {
                //        richTextBox1.AppendText("Channel : " + ch.Key + "\n\n");
                //        richTextBox1.AppendText("TemplateName\t\tIsBroadcaster\t\tSourceId\t\tTargetId\n");
                //        if (ch.Value.GetChannels().Count > 0)
                //        {
                //            foreach (var info in ch.Value.GetChannels())
                //            {
                //                richTextBox1.AppendText(info.Item1 + "\t\t\t");
                //                richTextBox1.AppendText(info.Item2 + "\t\t\t");
                //                richTextBox1.AppendText(info.Item3 + "\t\t");
                //                richTextBox1.AppendText(info.Item4 + "\n\n");
                //            }
                //        }
                //    }
                //}
                //else
                //    richTextBox1.Text = "No channels found in the model";
                //*********************************************
            }
            catch (Exception ex)
            {
                richTextBox1.ForeColor = Color.Red;
                richTextBox1.Text = ex.Message;
                richTextBox1.ForeColor = Color.Black;
            }
        }

        private Nta NtaFromXML(string path)
        {
            Nta nta;
            var serializer = new XmlSerializer(typeof(Nta));
            StreamReader file = new StreamReader(path);
            nta = (Nta)serializer.Deserialize(file);
            file.Close();
            return nta;
        }

        private Nta RenameTemplatesInModel(Nta model1, Nta model2)
        {
            int uniqueNamePostfix;
            bool hasPostfix;
            for (int x = 0; x < model2.Template.Count; x++) //For every template in model2 compare name to the templates in model 1
            {
                uniqueNamePostfix = 0;
                hasPostfix = false;
                string newName = "";
                // compare name of each template with model2 and add postfix integer increment to model2 template if names matched
                for (int y = 0; y < model1.Template.Count; y++)
                {
                    if (model2.Template[x].Name.Text.Trim() == model1.Template[y].Name.Text.Trim())
                    {
                        newName = model2.Template[x].Name.Text.Trim();
                        if (hasPostfix)
                            newName = newName.Substring(0, newName.Length - 1) + Convert.ToString(++uniqueNamePostfix);
                        else
                        {
                            newName = newName + "_" +Convert.ToString(++uniqueNamePostfix);
                            hasPostfix = true;
                        }

                        // modify template name in the system as well
                        string properties = @model2.System;
                        properties = properties.Replace((model2.Template[x].Name.Text.Trim()+","), newName + ",");
                        model2.System = @properties;
                        model2.Template[x].Name.Text = newName;

                        // if name is duplicated within model2 templates then add another postfix
                        //for (int z = 0; z < model2.Template.Count; z++)
                        //{
                        //    if (model2.Template[x].Name.Text.Trim() == model2.Template[z].Name.Text.Trim())
                        //    {
                        //        if (hasPostfix)
                        //            newName = newName.Substring(0, newName.Length - 1) + Convert.ToString(++uniqueNamePostfix);
                        //        else
                        //            newName = newName + Convert.ToString(++uniqueNamePostfix);
                        //        model2.Template[z].Name.Text = newName;
                        //    }
                        //}
                    }
                }
            }
            return model2;
        }

        private Nta MergeModels(Nta model1, Nta model2)
        {
            foreach(Template template in model2.Template)
            {
                model1.Template.Add(template);
            }

             //******* Remove system statement from model2 system so that 
            //rest of the text can be added to model1 system ******
            string systemProperties2 = @model2.System;
            int startPosition = systemProperties2.IndexOf("\nsystem");
            int endPosition = systemProperties2.IndexOf(";", startPosition);
            string systemStatement = systemProperties2.Substring(startPosition, (endPosition - startPosition)+1);
            systemProperties2 = systemProperties2.Replace(systemStatement, "");
            //**********************************************************

            //*** Modify model1 system statement and add templates names of model2 
            //and also append rest of the text in system tag of model2 into model1 system tag ***
            string systemProperties1 = @model1.System;
            startPosition = systemProperties1.IndexOf("\nsystem");
            endPosition = systemProperties1.IndexOf(";", startPosition);
            systemStatement = systemProperties1.Substring(startPosition, (endPosition - startPosition)+1);
            systemProperties1 = systemProperties1.Replace(systemStatement, "");
            systemProperties1 = systemProperties1 + "\n\n //Model2 properties \n\n";
            systemProperties1 = systemProperties1 + systemProperties2;

            systemStatement = systemStatement.Replace(";", "");
            foreach (Template template in model2.Template)
                systemStatement = systemStatement + " ," + template.Name.Text.Trim();
            systemStatement = systemStatement + ";";

            systemProperties1 = systemProperties1 + "\n" + systemStatement;
            richTextBox1.AppendText("\n"+ systemProperties1);
           //richTextBox1.AppendText("\n" + systemProperties2);
            return model1;
        }
        
        private bool IsChannelBroadcaster(string channelName)
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
                    PrintXML(filePath);
                else
                {
                    pathLabel.ForeColor = Color.Red;
                    pathLabel.Text = "File not found. Please open a file (.xml)";
                }
            }
            else if (parseSelection.SelectedIndex == 1)
            {
                if (!string.IsNullOrEmpty(filePath2))
                    PrintXML(filePath2);
                else
                {
                    pathLabel2.ForeColor = Color.Red;
                    pathLabel2.Text = "File not found. Please open a file (.xml)";
                }
            }
        }

        private void PrintXML(string path)
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
