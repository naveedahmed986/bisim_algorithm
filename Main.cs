using System;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bisimulation_Desktop
{
    public partial class Main : Form
    {
        string filePath { get; set; }
        string filePath2 { get; set; }

        public Main()
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
            ofd.Dispose();
            return "";
        }

        private void parseSelection_SelectedIndexChange(object sender, EventArgs e)
        {
            if (parseSelection.SelectedIndex == 0)
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    PrintXML(filePath);
                }
                else
                {
                    pathLabel.ForeColor = Color.Red;
                    pathLabel.Text = "File not found. Please open a file (.xml)";
                }
            }
            else if (parseSelection.SelectedIndex == 1)
            {
                if (!string.IsNullOrEmpty(filePath2))
                {
                    PrintXML(filePath2);
                }
                else
                {
                    pathLabel2.ForeColor = Color.Red;
                    pathLabel2.Text = "File not found. Please open a file (.xml)";
                }
            }
        }

        //Prints the model in xml format
        private void PrintXML(string path)
        {
            SetLoading(true);
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
            SetLoading(false);
        }

        private void modelTransform_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            if (string.IsNullOrEmpty(filePath))
            {
                pathLabel.ForeColor = Color.Red;
                pathLabel.Text = "File not found. Please open a file (.xml)";
            }
            //else if (string.IsNullOrEmpty(filePath2))
            //{
            //    pathLabel2.ForeColor = Color.Red;
            //    pathLabel2.Text = "File not found. Please open a file (.xml)";
            //}
            else
            {
                SetLoading(true);
                Nta model = TransformModels();
                string transformedFilePath = ConvertModel.NtatoXML(model);
                if (!string.IsNullOrEmpty(transformedFilePath))
                {
                    richTextBox1.ForeColor = Color.Black;
                    richTextBox1.Text = "File saved at : " + transformedFilePath;
                    richTextBox1.AppendText("\n\nFile name : bi_model.xml");
                }
                SetLoading(false);
            }
        }
        /*
         * Get all channels based template-wise
         * Function calls to calculate non-deterministic locations and transitions
         */
        private Nta TransformModels()
        {
            try
            {
                Nta model1 = ConvertModel.XMLtoNta(filePath);
                if (!string.IsNullOrEmpty(filePath2)) // merge model2 only if file path is provided
                {
                    Nta model2 = ConvertModel.XMLtoNta(filePath2);
                    model2 = Extension.RenameTemplatesInModel(model1, model2);
                    model2 = Extension.ModifyC_Code(model2);
                    //model1 = Extension.MergeModels(model1, model2);
                    model1 = Extension.MergeModels(model1, model2);
                }

                if (model1 != null)
                {
                    Dictionary<string, NdLocation> ndLocationList = new Dictionary<string, NdLocation>();
                    NdLocation ndLList;

                    Dictionary<string, NdTransition> ndTransitionList = new Dictionary<string, NdTransition>();
                    NdTransition ndTList;

                    LocationInfo.locationIds.Clear();
                    ChannelInfo.channelInfoList1.Clear();
                    ChannelInfo.channelInfoList2.Clear();
                    TemplateInfo.templateType.Clear();

                    foreach (Template template in model1.Template) // Loop over all templates
                    {
                        // List of Ids of all locations in the model
                        LocationInfo.AddLocationsIds(template);
                        //***********************************

                        // List of all channels in a custom structure template wise
                        ChannelInfo.AddChannelInfoByTemplate(template);
                        //***********************************
                    }

                    ChannelInfo.SplitChannelList();

                    foreach (Template template in model1.Template)
                    {
                        ChannelInfo.AddTemplateType(template);

                        ndTList = new NdTransition();
                        ndLList = new NdLocation();

                        ndTList = AddNdTransitionByTemplate(template);
                        if (ndTList.ndTransitions.Count > 0)
                            ndTransitionList.Add(template.Name.Text, ndTList);

                        ndLList = AddNdLocationsByTemplate(template);
                        if (ndLList.ndLocations.Count > 0)
                            ndLocationList.Add(template.Name.Text, ndLList);
                    }

                    // Add auxilary channels for all I/O actions in the model
                    //model1 = Synchronization.SyncIOActions(model1);
                    //***********************************

                    model1 = Synchronization.SyncModels(model1);
                    // Add committed locations for all locations that have more than one out going  transitions
                    //model1 = AddAuxilaryForNdLocation(ndLocationList, model1);
                    //**************************************

                }
                return model1;
            }
            catch (Exception ex)
            {
                richTextBox1.ForeColor = Color.Red;
                richTextBox1.Text = ex.Message;
                richTextBox1.ForeColor = Color.Black;
                return null;
            }
        }

        //Identifies non-deterministic locations and returns a list for a given template
        private NdLocation AddNdLocationsByTemplate(Template template)
        {
            NdLocation list = new NdLocation();
            int transitionSourceCount;
            foreach (Location location in template.Location)
            {
                transitionSourceCount = 0;
                foreach (Transition transition in template.Transition)
                {
                    if (string.Compare(transition.Source.Ref, location.Id) == 0)
                        transitionSourceCount++;
                }
                if (transitionSourceCount > 1)
                    list.ndLocations.Add(location);
            }
            return list;
        }

        //Identifies non-deterministic transitions and returns a list for a given template
        private NdTransition AddNdTransitionByTemplate(Template template)
        {
            NdTransition list = new NdTransition(); 
            foreach (Transition transition in template.Transition)
            {
                foreach (Label label in transition.Label)
                {
                    if (label.Kind == Constant.TransitionLabelKind.Select)
                    {
                        list.ndTransitions.Add(transition);
                    }
                }
            }
            return list;
        }

        //Set cursor to loading while some operation is taking place
        private void SetLoading(bool displayLoader)
        {
            if (displayLoader)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    //picLoader.Visible = true;
                    this.Cursor = Cursors.WaitCursor;
                });
            }
            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    //picLoader.Visible = false;
                    this.Cursor = Cursors.Default;
                });
            }
        }

        /*
         * Get the non-deterministic locations with template names and the processed merged model
         * Retruns the models with modified templates
         */
        private Nta AddAuxilaryForNdLocation(Dictionary<string, NdLocation> ndLocationList, Nta model)
        {
            Template template;
            NdLocation ndLocation;
            string templateName = string.Empty;
            if (ndLocationList != null && ndLocationList.Count > 0)
            {
                foreach (var item in ndLocationList)
                {
                    templateName = item.Key;
                    ndLocation = item.Value;
                    template = (from t in model.Template where t.Name.Text == item.Key select t).First();
                    model.Template.Remove(template);
                    template = AuxilaryForNdLocationPerTemplate(ndLocation, template);
                    model.Template.Add(template);
                }
            }
            return model;
        }

        //Add committed locations for non- deterministic locations for a given template
        private Template AuxilaryForNdLocationPerTemplate(NdLocation ndLocation, Template template)
        {
            string existingTargetid;
            string newId;
            Location targetLocation;
            Location committedLocation;
            Transition transition;
            List<Transition> newTransitions = new List<Transition>();
            List<Location> newLocations = new List<Location>();
            foreach (Location ndLoc in ndLocation.ndLocations)
            {
                for (int i = 0; i < template.Transition.Count; i++)
                {
                    existingTargetid = string.Empty;
                    newId = string.Empty;
                    if (template.Transition[i].Source.Ref == ndLoc.Id)
                    {
                        targetLocation = (from l in template.Location where l.Id == template.Transition[i].Target.Ref select l).First();
                        existingTargetid = template.Transition[i].Target.Ref; //save target of the existing transition

                        // Create new committed location **********
                        newId = LocationInfo.GetNewLocationId(); //get new id from id list for new location
                        committedLocation = new Location(); //initialize new location
                        committedLocation.Id = newId;
                        committedLocation.Committed = "committed";
                        Tuple<string, string> coordinates = LocationPoint.GetCoordinatesForLocationLastNail(ndLoc, targetLocation, template.Transition[i]);
                        committedLocation.X = coordinates.Item1;
                        committedLocation.Y = coordinates.Item2;
                        //*****************************************

                        LocationInfo.locationIds.Add(Int32.Parse(Regex.Match(newId, @"\d+").Value)); // add new id to the location id list

                        template.Transition[i].Target.Ref = newId; //set target of existing transition as the id of new location

                        // Add new transition from Source *********
                        transition = new Transition(); //initialize new transition
                        transition.Source = new Source();
                        transition.Target = new Target();
                        //*****************************************

                        transition.Source.Ref = newId; //set new transition source as the existing source location id
                        transition.Target.Ref = existingTargetid; // set target as the new location id

                        newLocations.Add(committedLocation);
                        newTransitions.Add(transition);
                        //template.Location.Add(committedLocation); //add new location in the template              
                        //template.Transition.Add(transition); //add new transition in the template
                    }
                }
            }
            if (newLocations.Count > 0)
                template.Location.AddRange(newLocations);
            if (newTransitions.Count > 0)
                template.Transition.AddRange(newTransitions);
            return template;
        }
    }
}
