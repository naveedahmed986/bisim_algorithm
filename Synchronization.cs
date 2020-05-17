using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Bisimulation_Desktop
{
    public static class Synchronization
    {
        private static List<string> auxChannelList = new List<string>();
        private static Nta nta = null;
        public static Nta SyncIOActions(Nta model)
        {

            Template template = null;
            //Location committedLocation = null, sourceLoc = null, targetLoc = null;
            //Transition sourceTransition = null, newTransition = null;
            //Label syncLabel = null, selectLabel = null;
            string newId = string.Empty, selectLabelValue = string.Empty;
            string channelDeclaration = string.Empty;
            bool isAuxChannelAdded = false, syncBefore = false;
            nta = model;
            List<string> modifiedChannels = new List<string>();
            List<Tuple<string, bool, string, string>> transitionList = new List<Tuple<string, bool, string, string>>();
            if (ChannelInfo.channelInfoList1.Count > 0)
            {
                foreach (var ch in ChannelInfo.channelInfoList1) // ch.Key contains channel name
                {
                    if (ChannelInfo.channelInfoList2.ContainsKey(ch.Key + Constant.Common.VariablePostfix))
                    {
                        var templs2 = ChannelInfo.channelInfoList2[ch.Key + Constant.Common.VariablePostfix]; // ch2.Key = ch.Key + Constant.Common.VariablePostfix

                        foreach (var templ in ch.Value) //templ.Key contains template name
                        {
                            foreach (var templ2 in templs2)
                            {
                                if (templ.Value.Count == 1 && templ2.Value.Count == 1 && templ.Value[0].Item1 == templ2.Value[0].Item1)
                                {
                                    if ((TemplateInfo.templateType[templ.Key]).Equals(Constant.TemplateType.ENV)) // if primary template is of type ENV
                                        syncBefore = true;
                                    var val1 = templ.Value[0];
                                    var val2 = templ2.Value[0];

                                    template = (from t in model.Template where t.Name.Text.Equals(templ.Key) select t).First();
                                    model.Template.Remove(template);
                                    template = AddAuxChannelInTemplate(template, val1.Item2, val1.Item3, val1.Item1, syncBefore, false);
                                    model.Template.Add(template);
                                    syncBefore = false;

                                    if ((TemplateInfo.templateType[templ.Key]).Equals(Constant.TemplateType.ENV)) // if primary template is of type ENV
                                        syncBefore = true;
                                    template = (from t in model.Template where t.Name.Text.Equals(templ2.Key) select t).First();
                                    model.Template.Remove(template);
                                    template = AddAuxChannelInTemplate(template, val2.Item2, val2.Item3, !val2.Item1, syncBefore, true);
                                    model.Template.Add(template);
                                    syncBefore = false;

                                    isAuxChannelAdded = true;
                                }
                            }
                        }
                    }


                    //if (ch.Key.Length >= 2 && (string.Equals(ch.Key.Substring(0, 2), Constant.Common.InputChannelPrefix) ||
                    //    string.Equals(ch.Key.Substring(0, 2), Constant.Common.OutputChannelPrefix)) &&
                    //    !modifiedChannels.Contains(ch.Key))
                    //{
                    //    foreach (var item in ch.Value)
                    //    {
                    //        if (item.Item2) // Signal is sender i.e. <channel_name>!
                    //        {
                    //            /* if there exist another env using same input channel
                    //            (since original channel name is modified in second model,
                    //            so the input channel will contain postfix "_" i.e. <channel_name_>) */
                    //            if (ChannelInfo.channelInfoList2.ContainsKey(ch.Key + Constant.Common.VariablePostfix))
                    //            {
                    //                if ((TemplateInfo.templateType[item.Item1]).Equals(Constant.TemplateType.ENV)) // if primary template is of type ENV
                    //                    syncBefore = true;
                    //                template = (from t in model.Template where t.Name.Text.Equals(item.Item1) select t).First();
                    //                model.Template.Remove(template);
                    //                template = AddAuxChannelInTemplate(template, item.Item3, item.Item4, item.Item2, syncBefore, false);
                    //                model.Template.Add(template);
                    //                syncBefore = false;
                    //                var secondary_ch = (from c in ChannelInfo.channelInfoList1
                    //                                    where c.Key.Equals(ch.Key + Constant.Common.VariablePostfix)
                    //                                    select c).First();
                    //                foreach (var item_ in secondary_ch.Value)
                    //                {
                    //                    if (item_.Item2) // Signal is sender in second ENV i.e. <channel_name>!
                    //                    {
                    //                        if ((TemplateInfo.templateType[item_.Item1]).Equals(Constant.TemplateType.ENV)) // if secondary template is of type ENV
                    //                            syncBefore = true;
                    //                        template = (from t in model.Template where t.Name.Text.Equals(item_.Item1) select t).First();
                    //                        model.Template.Remove(template);
                    //                        template = AddAuxChannelInTemplate(template, item_.Item3, item_.Item4, !item_.Item2, syncBefore, true);
                    //                        model.Template.Add(template);
                    //                        syncBefore = false;
                    //                    }
                    //                }
                    //                isAuxChannelAdded = true;
                    //                modifiedChannels.Add(ch.Key + Constant.Common.VariablePostfix);
                    //            }
                    //        }
                    //    }
                    //}

                    //if (ch.Key.Length >= 2 && (string.Equals(ch.Key.Substring(0, 2), Constant.Common.InputChannelPrefix) ||
                    //    string.Equals(ch.Key.Substring(0, 2), Constant.Common.OutputChannelPrefix)))
                    //{
                    //    //channelDeclaration = channelDeclaration + ch.Key + Constant.Common.AuxilaryChannelPostfix + " ,";
                    //    if (ch.Value.Count > 0)
                    //    {
                    //        foreach (var info in ch.Value)
                    //        {
                    //            template = null;
                    //            sourceLoc = null;
                    //            targetLoc = null;
                    //            sourceTransition = null;

                    //            template = (from t in model.Template where t.Name.Text.Equals(info.Item1) select t).First();
                    //            sourceLoc = (from sl in template.Location where sl.Id.Equals(info.Item3) select sl).First();
                    //            targetLoc = (from tl in template.Location where tl.Id.Equals(info.Item4) select tl).First();
                    //            sourceTransition = TransitionInfo.GetTransitionBySourceAndTarget(template, info.Item3, info.Item4);
                    //            template.Transition.Remove(sourceTransition);
                    //            model.Template.Remove(template);

                    //            newId = LocationInfo.GetNewLocationId(); //get new id from id list for new location
                    //            committedLocation = new Location(); //initialize new location
                    //            committedLocation.Id = newId;
                    //            committedLocation.Committed = Constant.LocationLabelKind.Committed;
                    //            Tuple<string, string> coordinates = LocationPoint.GetCoordinatesForLocation(sourceLoc, targetLoc, sourceTransition);
                    //            committedLocation.X = coordinates.Item1;
                    //            committedLocation.Y = coordinates.Item2;
                    //            LocationInfo.locationIds.Add(Int32.Parse(Regex.Match(newId, @"\d+").Value)); // add new id to the location id list

                    //            newTransition = new Transition();
                    //            newTransition.Source = new Source();
                    //            newTransition.Target = new Target();

                    //            sourceTransition.Target.Ref = newId;
                    //            newTransition.Source.Ref = newId;
                    //            newTransition.Target.Ref = targetLoc.Id;

                    //            syncLabel = new Label();
                    //            syncLabel.Kind = Constant.TransitionLabelKind.Synchronization;
                    //            string channelTName = TransitionInfo.GetChannelNameFromTransition(sourceTransition);
                    //            syncLabel.Text = GetAuxChannelName(info.Item2, channelTName, model.Declaration);
                    //            coordinates = LocationPoint.CalculateCenterPoint(
                    //                Int32.Parse(coordinates.Item1),
                    //                Int32.Parse(coordinates.Item2),
                    //                Int32.Parse(targetLoc.X),
                    //                Int32.Parse(targetLoc.Y));
                    //            syncLabel.X = coordinates.Item1;
                    //            syncLabel.Y = coordinates.Item2;

                    //            newTransition.Label = new List<Label>();
                    //            newTransition.Label.Add(syncLabel);

                    //            selectLabelValue = GetLabelForSelectIfExist(sourceTransition);
                    //            if (!string.IsNullOrEmpty(selectLabelValue))
                    //            {
                    //                selectLabel = new Label();
                    //                selectLabel.Kind = Constant.TransitionLabelKind.Select;
                    //                selectLabel.Text = selectLabelValue;
                    //                selectLabel.X = coordinates.Item1;
                    //                selectLabel.Y = Convert.ToString(Int32.Parse(coordinates.Item2) - 15);
                    //                newTransition.Label.Add(selectLabel);
                    //            }

                    //            template.Location.Add(committedLocation);
                    //            template.Transition.Add(newTransition);
                    //            template.Transition.Add(sourceTransition);
                    //            model.Template.Add(template);
                    //            isAuxChannelAdded = true;
                    //        }
                    //    }
                    //}
                }
                if (isAuxChannelAdded)
                {
                    foreach (string name in auxChannelList)
                        channelDeclaration = channelDeclaration+ " "+ name + ",";
                    channelDeclaration = "\nchan " + channelDeclaration.Substring(0, channelDeclaration.Length - 1) + ";";
                    model.Declaration = string.Concat(model.Declaration, channelDeclaration);
                }
            }
            return model;
        }

        private static Template AddAuxChannelInTemplate(Template template, string sourceId, string targetId, bool isSender, bool syncBefore, bool isSecondary)
        {     
            Location sourceLoc = (from sl in template.Location where sl.Id.Equals(sourceId) select sl).First();
            Location targetLoc = (from tl in template.Location where tl.Id.Equals(targetId) select tl).First();
            Transition sourceTransition = TransitionInfo.GetTransitionBySourceAndTarget(template, sourceId, targetId);
            template.Transition.Remove(sourceTransition);

            string newId = LocationInfo.GetNewLocationId(); //get new id from id list for new location
            Location committedLocation = new Location(); //create new location
            committedLocation.Id = newId;
            committedLocation.Committed = Constant.LocationLabelKind.Committed;
            Tuple<string, string> coordinates = LocationPoint.GetCoordinatesForLocation(sourceLoc, targetLoc, sourceTransition);
            committedLocation.X = coordinates.Item1;
            committedLocation.Y = coordinates.Item2;
            LocationInfo.locationIds.Add(Int32.Parse(Regex.Match(newId, @"\d+").Value)); // add new id to the location id list

            Transition newTransition = new Transition();  // create new transition
            newTransition.Source = new Source();
            newTransition.Target = new Target();

            if(syncBefore) // configure new transition (add aux channel before IO Action)
            {
                sourceTransition.Source.Ref = newId;
                newTransition.Source.Ref = sourceLoc.Id;
                newTransition.Target.Ref = newId;
            }
            else // configure new transition (add aux channel after IO Action)
            {
                sourceTransition.Target.Ref = newId;
                newTransition.Source.Ref = newId;
                newTransition.Target.Ref = targetLoc.Id;
            }

            Label syncLabel = new Label(); // create synchronization channel Label
            syncLabel.Kind = Constant.TransitionLabelKind.Synchronization;
            string sourceChannelName = TransitionInfo.GetChannelNameFromTransition(sourceTransition);
            if (isSecondary) // means that original channel name is modified and contains _ and needs to be removed before assigning to aux channel
                sourceChannelName = sourceChannelName.Substring(0, sourceChannelName.Length - 1);
            string auxChannelName = GetAuxChannelName(sourceChannelName); // get name for auxiliary channel
            AddChannelNameToAuxChannelList(auxChannelName, nta.Declaration + template.Declaration); // add auxiliary channel name to the list
            syncLabel.Text = auxChannelName + (isSender ? Constant.ActionType.Sender : Constant.ActionType.Receiver);
            coordinates = LocationPoint.CalculateCenterPoint(
                Int32.Parse(coordinates.Item1),
                Int32.Parse(coordinates.Item2),
                Int32.Parse(targetLoc.X),
                Int32.Parse(targetLoc.Y)); // calculate center point location for synchronization label
            syncLabel.X = coordinates.Item1;
            syncLabel.Y = coordinates.Item2;

            newTransition.Label = new List<Label>();
            newTransition.Label.Add(syncLabel); // add auxiliary channel label to new transition

            template.Location.Add(committedLocation); // add committed location to the template
            template.Transition.Add(newTransition); // add new transition to the template
            template.Transition.Add(sourceTransition); // add modified source transition back in the template

            return template;
        }

        public static string GetAuxChannelName(string sourceChannelName)
        {
            if (!string.IsNullOrEmpty(sourceChannelName))
            {
                string postfix = string.Empty;
                string channelName = ChannelInfo.GetChannelName(sourceChannelName);
                if (sourceChannelName.Contains('[') && sourceChannelName.Contains(']') && 
                    sourceChannelName.IndexOf(']') > sourceChannelName.IndexOf('['))
                {
                    postfix = sourceChannelName.Substring(
                            sourceChannelName.IndexOf('['),
                            (sourceChannelName.Length - sourceChannelName.IndexOf('[')));
                    //AddChannelNameToAuxChannelList(channelName, gDeclaration);
                }
                else
                {
                    //postfix = Convert.ToString(sourceChannelName.Last());
                    //if (!auxChannelList.Contains(channelName+Constant.Common.AuxilaryChannelPostfix))
                        //auxChannelList.Add(channelName + Constant.Common.AuxilaryChannelPostfix);
                }
                channelName = channelName  + Constant.Common.AuxilaryChannelPostfix + postfix;
                //if (isOutput)
                //    channelName = string.Concat(channelName, Convert.ToString(Constant.ActionType.Output));
                //else
                //    channelName = string.Concat(channelName + Convert.ToString(Constant.ActionType.Input));
                return channelName;
            }
            return "";
        }
        
        private static void AddChannelNameToAuxChannelList(string channelName, string declaration)
        {
            if(!string.IsNullOrEmpty(channelName))
            {
                string channelInstanceFromDec = string.Empty;
                if (channelName.Contains('[') && channelName.Contains('['))
                {
                    int startIndex = declaration.IndexOf(channelName);
                    int endIndex = declaration.IndexOf(']', startIndex);
                    channelInstanceFromDec = declaration.Substring(startIndex, (endIndex - startIndex) + 1);
                    channelInstanceFromDec = channelInstanceFromDec.Substring(channelInstanceFromDec.IndexOf('['),
                        (channelInstanceFromDec.IndexOf(']') - channelInstanceFromDec.IndexOf('[')) + 1
                        );
                }
                channelName = channelName + channelInstanceFromDec;
                if(!auxChannelList.Contains(channelName))
                    auxChannelList.Add(channelName);
            }
        }

        private static string GetLabelForSelectIfExist(Transition transition)
        {
            foreach(Label l in transition.Label)
            {
                if(l.Kind.Equals(Constant.TransitionLabelKind.Select))
                    return l.Text;
            }
            return string.Empty;
        }

        private static List<Tuple<string, bool, string, string>> GetChannelListBySignalType(List<Tuple<string, bool, string, string>> items, bool isSender)
        {
            List<Tuple<string, bool, string, string>> list = new List<Tuple<string, bool, string, string>>();
            if (isSender)
                foreach (var item in items)
                {
                    if (item.Item2)
                    {
                        list.Add(item);
                    }
                }
            else
                foreach (var item in items)
                {
                    if (!item.Item2)
                    {
                        list.Add(item);
                    }
                }
            return list;
        }
    }
}
