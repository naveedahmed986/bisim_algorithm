using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bisimulation_Desktop
{
    public static class Synchronization
    {
        private static List<string> auxChannelList = new List<string>();
        private static Nta nta = null;
        public static Nta SyncIOActions(Nta model)
        {

            Template template = null, template2 = null;
            //Location committedLocation = null, sourceLoc = null, targetLoc = null;
            //Transition sourceTransition = null, newTransition = null;
            //Label syncLabel = null, selectLabel = null;
            string newId = string.Empty, selectLabelValue = string.Empty;
            string channelDeclaration = string.Empty, auxChannelName = string.Empty;
            bool isAuxChannelAdded = false, syncBefore = false, t1HasGuard = false, t2HasGuard = false;
            nta = model;
            List<Transition> tList = new List<Transition>();
            List<string> modifiedChannels = new List<string>();
            List<Tuple<string, bool, string, string>> transitionList = new List<Tuple<string, bool, string, string>>();
            if (ChannelInfo.channelInfoList1.Count > 0 && ChannelInfo.channelInfoList2.Count > 0)
            {
                foreach (var ch in ChannelInfo.channelInfoList1) // ch.Key contains channel name
                {
                    if (ChannelInfo.IsIOChannel(ch.Key))
                    {
                        if (ChannelInfo.channelInfoList2.ContainsKey(ch.Key + Constant.Common.VariablePostfix))
                        {
                            var templs2 = ChannelInfo.channelInfoList2[ch.Key + Constant.Common.VariablePostfix]; // ch2.Key = ch.Key + Constant.Common.VariablePostfix

                            foreach (var templ in ch.Value) //templ.Key contains template name
                            {
                                foreach (var templ2 in templs2)
                                {
                                    if (templ.Value.Count == 1 && templ2.Value.Count == 1 && templ.Value[0].Item1 == templ2.Value[0].Item1) // if both templates contain exactly one transition with that channel and both are either sender or receiver
                                    {
                                        auxChannelName = GetAuxiliaryChannelName(ch.Key.Substring(0, 2));
                                        if ((TemplateInfo.templateType[templ.Key]).Equals(Constant.TemplateType.ENV)) // if primary template is of type ENV
                                            syncBefore = true;
                                        var val1 = templ.Value[0];
                                        var val2 = templ2.Value[0];

                                        template = (from t in model.Template where t.Name.Text.Equals(templ.Key) select t).First();
                                        model.Template.Remove(template);
                                        template = AddAuxChannelInTemplate(template, val1.Item2, val1.Item3, syncBefore, string.Concat(auxChannelName, Constant.ActionType.Sender));
                                        model.Template.Add(template);
                                        syncBefore = false;

                                        if ((TemplateInfo.templateType[templ.Key]).Equals(Constant.TemplateType.ENV)) // if primary template is of type ENV
                                            syncBefore = true;
                                        template = (from t in model.Template where t.Name.Text.Equals(templ2.Key) select t).First();
                                        model.Template.Remove(template);
                                        template = AddAuxChannelInTemplate(template, val2.Item2, val2.Item3, syncBefore, string.Concat(auxChannelName, Constant.ActionType.Receiver));
                                        model.Template.Add(template);
                                        syncBefore = false;

                                        auxChannelName = string.Empty;
                                        isAuxChannelAdded = true;
                                    }

                                    if (templ.Value.Count > 1 && templ2.Value.Count > 1)
                                    {
                                        foreach (var val1 in templ.Value)
                                        {
                                            if (!val1.Item2.Equals(val1.Item3)) // skip self loop transition
                                            {
                                                t1HasGuard = false;
                                                template = (from t in model.Template where t.Name.Text.Equals(templ.Key) select t).First();
                                                tList = GetPrevTransition(template, val1.Item2);
                                                foreach (Transition t in tList)
                                                    if (ContainsTransitionKind(t, Constant.TransitionLabelKind.Guard))
                                                        t1HasGuard = true;
                                                foreach (var val2 in templ2.Value)
                                                {
                                                    if (!val2.Item2.Equals(val2.Item3))
                                                    {
                                                        t2HasGuard = false;
                                                        template2 = (from t in model.Template where t.Name.Text.Equals(templ2.Key) select t).First();
                                                        tList = GetPrevTransition(template2, val2.Item2);
                                                        foreach (Transition t in tList)
                                                            if (ContainsTransitionKind(t, Constant.TransitionLabelKind.Guard))
                                                                t2HasGuard = true;
                                                        if (t1HasGuard == t2HasGuard && val1.Item1 == val2.Item1)
                                                        {
                                                            auxChannelName = GetAuxiliaryChannelName(ch.Key.Substring(0, 2));
                                                            if ((TemplateInfo.templateType[templ.Key]).Equals(Constant.TemplateType.ENV))
                                                                syncBefore = true;
                                                            model.Template.Remove(template);
                                                            template = AddAuxChannelInTemplate(template, val1.Item2, val1.Item3, syncBefore, string.Concat(auxChannelName, Constant.ActionType.Sender));
                                                            model.Template.Add(template);
                                                            syncBefore = false;

                                                            if ((TemplateInfo.templateType[templ2.Key]).Equals(Constant.TemplateType.ENV))
                                                                syncBefore = true;
                                                            model.Template.Remove(template2);
                                                            template2 = AddAuxChannelInTemplate(template2, val2.Item2, val2.Item3, syncBefore, string.Concat(auxChannelName, Constant.ActionType.Receiver));
                                                            model.Template.Add(template2);
                                                            syncBefore = false;

                                                            auxChannelName = string.Empty;
                                                            isAuxChannelAdded = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
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
                    if(!string.IsNullOrEmpty(channelDeclaration))
                        channelDeclaration = "\nchan " + channelDeclaration.Substring(0, channelDeclaration.Length - 1) + ";";
                    model.Declaration = string.Concat(model.Declaration, channelDeclaration);
                }
            }
            return model;
        }

        private static Template AddAuxChannelInTemplate(Template template, string sourceId, string targetId, bool syncBefore, string auxChannelName)
        {     
            Location sourceLoc = (from sl in template.Location where sl.Id.Equals(sourceId) select sl).First();
            Location targetLoc = (from tl in template.Location where tl.Id.Equals(targetId) select tl).First();
            Transition sourceTransition = TransitionInfo.GetTransitionBySourceAndTargetId(template, sourceId, targetId);
            template.Transition.Remove(sourceTransition);

            string newId = LocationInfo.GetNewLocationId(); //get new id from id list for new location
            LocationInfo.locationIds.Add(Int32.Parse(Regex.Match(newId, @"\d+").Value)); // add new id to the location id list

            Location committedLocation = new Location(); //create new location
            committedLocation.Id = newId;
            committedLocation.Committed = Constant.LocationLabelKind.Committed;
            
            Label syncLabel = new Label(); // create synchronization channel Label
            syncLabel.Kind = Constant.TransitionLabelKind.Synchronization;
            syncLabel.Text = auxChannelName;

            Transition newTransition = new Transition();  // create new transition
            newTransition.Source = new Source();
            newTransition.Target = new Target();

            Tuple<string, string> coordinates;
            /*
             * SRC-------new transition---->COMMITTED_LOC-----original transition----->TARGET_LOC
             * configure new transition (split the original transition and add committed location in between,  
             * new transition is between sourceLoc and committedLoc and original transition is between committedLoc and targetLoc)
             */
            if (syncBefore) 
            {
                sourceTransition.Source.Ref = newId;
                newTransition.Source.Ref = sourceLoc.Id;
                newTransition.Target.Ref = newId;

                coordinates = LocationPoint.GetCoordinatesForLocationFirstNail(sourceLoc, targetLoc, sourceTransition);
                committedLocation.X = coordinates.Item1;
                committedLocation.Y = coordinates.Item2;

                coordinates = LocationPoint.CalculateCenterPoint(
                Int32.Parse(sourceLoc.X),
                Int32.Parse(sourceLoc.Y),
                Int32.Parse(coordinates.Item1),
                Int32.Parse(coordinates.Item2)); // calculate center point location for synchronization label
                syncLabel.X = coordinates.Item1;
                syncLabel.Y = coordinates.Item2;
            }
            /*
             * SRC-------original transition---->COMMITTED_LOC-----new transition----->TARGET_LOC
             * configure new transition (split the original transition and add committed location in between,
             *  original transition is sourceLoc and committedLoc and new transition is between committedLoc and targetLoc)
             */
            else
            {
                sourceTransition.Target.Ref = newId;
                newTransition.Source.Ref = newId;
                newTransition.Target.Ref = targetLoc.Id;

                coordinates = LocationPoint.GetCoordinatesForLocationLastNail(sourceLoc, targetLoc, sourceTransition);
                committedLocation.X = coordinates.Item1;
                committedLocation.Y = coordinates.Item2;

                coordinates = LocationPoint.CalculateCenterPoint(
                Int32.Parse(coordinates.Item1),
                Int32.Parse(coordinates.Item2),
                Int32.Parse(targetLoc.X),
                Int32.Parse(targetLoc.Y)); // calculate center point location for synchronization label
                syncLabel.X = coordinates.Item1;
                syncLabel.Y = coordinates.Item2;
            }

            //string sourceChannelName = TransitionInfo.GetChannelNameFromTransition(sourceTransition);
            //if (isSecondary) // means that original channel name is modified and contains _ and needs to be removed before assigning to aux channel
            //sourceChannelName = sourceChannelName.Substring(0, sourceChannelName.Length - 1);
            //string auxChannelName = GetAuxChannelName(sourceChannelName); // get name for auxiliary channel
            //AddChannelNameToAuxChannelList(auxChannelName, nta.Declaration + template.Declaration); // add auxiliary channel name to the list
            //auxChannelName + (isSender ? Constant.ActionType.Sender : Constant.ActionType.Receiver);

            newTransition.Label = new List<Label>();
            newTransition.Label.Add(syncLabel); // add auxiliary channel label to new transition

            template.Location.Add(committedLocation); // add committed location to the template
            template.Transition.Add(newTransition); // add new transition to the template
            template.Transition.Add(sourceTransition); // add modified source transition back in the template

            return template;
        }

        private static string GetAuxiliaryChannelName(string actionType)
        {
            string channelName = Constant.Common.AuxilaryChannelPostfix;
                if (!string.IsNullOrEmpty(actionType))
                {
                    if (actionType.Equals(Constant.Common.InputChannelPrefix))
                        channelName = string.Concat(Constant.Common.InputChannelPrefix, channelName);
                    else if(actionType.Equals(Constant.Common.OutputChannelPrefix))
                        channelName = string.Concat(Constant.Common.OutputChannelPrefix, channelName);
                }
            int newId = GetAuxChannelId();
            channelName = string.Concat(channelName, Convert.ToString(newId));
            auxChannelList.Add(channelName);
            return channelName;
        }

        public static int GetAuxChannelId()
        {
            List<int> idList = new List<int>();
            if(auxChannelList.Count > 0)
                for (int i = 0; i < auxChannelList.Count; i++)
                    idList.Add(LocationInfo.GetNumberFromString(auxChannelList[i]));
            return (idList.Count > 0 ? (idList.Max() + 1) : 0);
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
                if (channelName.Contains('[') && channelName.Contains(']'))
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

        private static List<Transition> GetFirstChannelTransition(Template template)
        {
            List<Transition> tList = new List<Transition>();
            List<string> targetIdList = new List<string>();
            if (template != null)
            {
                foreach (Transition t in template.Transition) // if transition(s) from initial location contains synchronization
                    if (IsTransitionInit(template, t.Source.Ref))
                        if (ContainsTransitionKind(t, Constant.TransitionLabelKind.Synchronization))
                            tList.Add(t);
                        else
                            targetIdList.Add(t.Target.Ref);
                if(tList.Count <= 0)
                    foreach(string id in targetIdList)
                    {
                        foreach(Transition t in template.Transition)
                        {
                            if (t.Source.Ref.Equals(id))
                            {
                                if (ContainsTransitionKind(t, Constant.TransitionLabelKind.Synchronization))
                                    tList.Add(t);
                            }
                        }
                    }
            }
            return tList;
        }

        private static bool IsTransitionInit(Template template, string id)
        {
            if (template != null && !string.IsNullOrEmpty(id))
                if (template.Init.Ref.Equals(id))
                    return true;
            return false;
        }

        private static bool ContainsTransitionKind(Transition transition, string kind)
        {
            if(transition != null && !string.IsNullOrEmpty(kind))
                foreach (Label l in transition.Label)
                    if (l.Kind.Equals(kind))
                        return true;
            return false;
        }

        private static Label GetLabelForTransitionKind(Transition transition, string kind)
        {
            if (transition != null && !string.IsNullOrEmpty(kind))
                foreach (Label l in transition.Label)
                    if (l.Kind.Equals(kind))
                        return l;
            return null;
        }

        private static List<Transition> GetPrevTransition(Template template, string id)
        {
            List<Transition> tList = new List<Transition>();
            if (template != null && !string.IsNullOrEmpty(id))
                foreach(Transition t in template.Transition)
                    if (t.Target.Ref.Equals(id))
                        tList.Add(t);
            return tList;
        }

        public static Nta SynchronizeModels(Nta model1)
        {
            Location newLoc1 = null, newLoc2 = null, newLoc3 = null, sourceLoc=null, targetLoc=null;
            List<Location> locList1 = new List<Location>();
            List<Transition> tranList1 = new List<Transition>();
            Transition newTransition1 = null, newTransition2 = null, newTransition3 = null;
            string newId1 = string.Empty, newId2 = string.Empty, newId3 = string.Empty, guardVar = string.Empty,
                syncDeclarations= string.Empty, X=string.Empty, Y=string.Empty;
            nta=model1;
            Label label1 = null, label2 = null, syncLabel=null;
            Transition srcTransition2 = null;
            Template tgtTemplate2 = null;
            int chanIndex = 0, fIndex=0, gIndex=0;
            bool isSUT = false;
            Tuple<string, string> coordinates=new Tuple<string, string>(string.Empty, string.Empty);
            if (model1.Template.Count <= 0)
                return model1;
            for(int teIndex =0; teIndex < nta.Template.Count; teIndex++)
            {
                tgtTemplate2 = TemplateInfo.GetTemplateByName(model1, string.Concat(model1.Template[teIndex].Name.Text, Constant.Common.TemplateRenamePostfix));
                if (tgtTemplate2 != null)
                {
                    model1.Template.Remove(tgtTemplate2);
                    if (model1.Template[teIndex].Transition.Count > 0)
                    {
                        for (int trIndex = 0; trIndex < nta.Template[teIndex].Transition.Count; trIndex++)
                        {
                            if ((label1 = TransitionInfo.TransitionHasChannel(model1.Template[teIndex].Transition[trIndex])) != null)
                            {
                                srcTransition2 = TransitionInfo.GetTransitionBySourceAndTargetId(tgtTemplate2, model1.Template[teIndex].Transition[trIndex].Source.Ref, model1.Template[teIndex].Transition[trIndex].Target.Ref);
                                if (srcTransition2 != null)
                                {
                                    if ((label2 = TransitionInfo.TransitionHasChannel(srcTransition2)) != null)
                                    {
                                        chanIndex++;
                                        if ((TemplateInfo.templateType[model1.Template[teIndex].Name.Text]).Equals(Constant.TemplateType.SUT))
                                        {
                                            gIndex++;
                                            guardVar = "g" + gIndex;
                                        }
                                        else
                                        {
                                            fIndex++;
                                            guardVar = "f" + fIndex;
                                            isSUT = false;
                                        }

                                        tgtTemplate2.Transition.Remove(srcTransition2);
                                        
                                        // First part of the pattern
                                        newId1 = LocationInfo.GetNewLocationId();
                                        LocationInfo.locationIds.Add(Int32.Parse(Regex.Match(newId1, @"\d+").Value));

                                        newLoc1 = new Location();
                                        newLoc1.Id = newId1;
                                        newLoc1.Urgent = Constant.LocationLabelKind.Urgent;

                                        sourceLoc = LocationInfo.GetLocationById(model1.Template[teIndex],
                                            model1.Template[teIndex].Transition[trIndex].Source.Ref);
                                        targetLoc = LocationInfo.GetLocationById(model1.Template[teIndex],
                                            model1.Template[teIndex].Transition[trIndex].Target.Ref);
                                        if(sourceLoc != null && targetLoc != null)
                                        {
                                            coordinates = LocationPoint.GetCoordinatesForLocationLastNail(sourceLoc, targetLoc, 
                                                model1.Template[teIndex].Transition[trIndex]);
                                            newLoc1.X = coordinates.Item1;
                                            newLoc1.Y = coordinates.Item2;

                                            coordinates = LocationPoint.CalculateCenterPoint(
                                                Int32.Parse(sourceLoc.X),
                                                Int32.Parse(sourceLoc.Y),
                                                Int32.Parse(coordinates.Item1),
                                                Int32.Parse(coordinates.Item2)); // calculate center point location for synchronization label
                                            X = coordinates.Item1;
                                            Y = coordinates.Item2;
                                        }
                                        
                                        newTransition1 = new Transition();
                                        newTransition1.Source = new Source();
                                        newTransition1.Target = new Target();

                                        newTransition1.Source.Ref = newId1;
                                        newTransition1.Target.Ref = model1.Template[teIndex].Transition[trIndex].Target.Ref;

                                        syncLabel = new Label();
                                        syncLabel.Kind = Constant.TransitionLabelKind.Synchronization;
                                        syncLabel.Text = Constant.Common.AuxilaryChannelPostfix + chanIndex + Constant.ActionType.Sender;
                                        syncLabel.X = X;
                                        syncLabel.Y = Y;

                                        newTransition1.Label = new List<Label>();
                                        newTransition1.Label.Add(syncLabel);

                                        model1.Template[teIndex].Transition[trIndex].Target.Ref = newId1;

                                        syncLabel = GetLabelForTransitionKind(model1.Template[teIndex].Transition[trIndex], Constant.TransitionLabelKind.Update);
                                        if (syncLabel == null)
                                        {
                                            syncLabel = new Label();
                                            syncLabel.Kind = Constant.TransitionLabelKind.Update;
                                            syncLabel.Text = guardVar + "=1";
                                            X = coordinates.Item1;
                                            Y = coordinates.Item2;
                                            model1.Template[teIndex].Transition[trIndex].Label.Add(syncLabel);
                                        }
                                        else
                                        {
                                            model1.Template[teIndex].Transition[trIndex].Label.Remove(syncLabel);
                                            syncLabel.Text = syncLabel.Text + ", " + guardVar + "=1";
                                            model1.Template[teIndex].Transition[trIndex].Label.Add(syncLabel);
                                        }

                                        if (!isSUT) // if template is not controller then add flag to the pattern
                                        {
                                            syncLabel = GetLabelForTransitionKind(model1.Template[teIndex].Transition[trIndex], Constant.TransitionLabelKind.Guard);
                                            if (syncLabel == null)
                                            {
                                                syncLabel = new Label();
                                                syncLabel.Kind = Constant.TransitionLabelKind.Guard;
                                                syncLabel.Text = "flag==false";
                                                syncLabel.X = X;
                                                syncLabel.Y = Y;
                                                model1.Template[teIndex].Transition[trIndex].Label.Add(syncLabel);
                                            }
                                            else
                                            {
                                                model1.Template[teIndex].Transition[trIndex].Label.Remove(syncLabel);
                                                syncLabel.Text = syncLabel.Text + " && flag==false";
                                                model1.Template[teIndex].Transition[trIndex].Label.Add(syncLabel);
                                            }
                                            syncLabel = GetLabelForTransitionKind(model1.Template[teIndex].Transition[trIndex], Constant.TransitionLabelKind.Update);
                                            model1.Template[teIndex].Transition[trIndex].Label.Remove(syncLabel);
                                            syncLabel.Text = syncLabel.Text + ", flag=true";
                                            model1.Template[teIndex].Transition[trIndex].Label.Add(syncLabel);
                                        }

                                        locList1.Add(newLoc1);
                                        tranList1.Add(newTransition1);
                                        
                                        // Second part of the pattern
                                        newId2 = LocationInfo.GetNewLocationId();
                                        LocationInfo.locationIds.Add(Int32.Parse(Regex.Match(newId2, @"\d+").Value));
                                        newId3 = LocationInfo.GetNewLocationId();
                                        LocationInfo.locationIds.Add(Int32.Parse(Regex.Match(newId1, @"\d+").Value));

                                        newLoc2 = new Location();
                                        newLoc2.Id = newId2;
                                        newLoc2.Urgent = Constant.LocationLabelKind.Urgent;

                                        newTransition2 = new Transition();
                                        newTransition2.Source = new Source();
                                        newTransition2.Target = new Target();
                                        newTransition2.Source.Ref = srcTransition2.Source.Ref;
                                        newTransition2.Target.Ref = newId2;
                                        newTransition2.Label = new List<Label>();

                                        sourceLoc = LocationInfo.GetLocationById(tgtTemplate2, srcTransition2.Source.Ref);
                                        targetLoc = LocationInfo.GetLocationById(tgtTemplate2, srcTransition2.Target.Ref);
                                        if (sourceLoc != null && targetLoc != null)
                                        {
                                            coordinates = LocationPoint.GetCoordinatesForLocationLastNail(sourceLoc, targetLoc, srcTransition2);
                                            newLoc2.X = coordinates.Item1;
                                            newLoc2.Y = coordinates.Item2;

                                            coordinates = LocationPoint.CalculateCenterPoint(
                                                Int32.Parse(sourceLoc.X),
                                                Int32.Parse(sourceLoc.Y),
                                                Int32.Parse(coordinates.Item1),
                                                Int32.Parse(coordinates.Item2)); // calculate center point location for synchronization label
                                            X = coordinates.Item1;
                                            Y = coordinates.Item2;
                                        }

                                        syncLabel = new Label();
                                        syncLabel.Kind = Constant.TransitionLabelKind.Guard;
                                        syncLabel.Text = guardVar + "==1";
                                        syncLabel.X = X;
                                        syncLabel.Y = Y;
                                        newTransition2.Label.Add(syncLabel);

                                        syncLabel = new Label();
                                        syncLabel.Kind = Constant.TransitionLabelKind.Update;
                                        syncLabel.Text = guardVar + "=0";
                                        syncLabel.X = X;
                                        syncLabel.Y = Y;
                                        newTransition2.Label.Add(syncLabel);

                                        newLoc3 = new Location();
                                        newLoc3.Id = newId3;
                                        newLoc3.Committed = Constant.LocationLabelKind.Committed;

                                        newTransition3 = new Transition();
                                        newTransition3.Source = new Source();
                                        newTransition3.Target = new Target();
                                        newTransition3.Source.Ref = newId3;
                                        newTransition3.Target.Ref = srcTransition2.Target.Ref;
                                        newTransition3.Label = new List<Label>();

                                        //sourceLoc = LocationInfo.GetLocationById(tgtTemplate2, newTransition3.Source.Ref);
                                        targetLoc = LocationInfo.GetLocationById(tgtTemplate2, srcTransition2.Target.Ref);
                                        if (newLoc2 != null && targetLoc != null)
                                        {
                                            coordinates = LocationPoint.GetCoordinatesForLocationLastNail(newLoc2, targetLoc, newTransition3);
                                            newLoc3.X = coordinates.Item1;
                                            newLoc3.Y = coordinates.Item2;

                                            coordinates = LocationPoint.CalculateCenterPoint(
                                                Int32.Parse(newLoc2.X),
                                                Int32.Parse(newLoc2.Y),
                                                Int32.Parse(coordinates.Item1),
                                                Int32.Parse(coordinates.Item2)); // calculate center point location for synchronization label
                                            X = coordinates.Item1;
                                            Y = coordinates.Item2;
                                        }

                                        syncLabel = new Label();
                                        syncLabel.Kind = Constant.TransitionLabelKind.Synchronization;
                                        syncLabel.Text = Constant.Common.AuxilaryChannelPostfix + chanIndex + Constant.ActionType.Receiver;
                                        syncLabel.X = X;
                                        syncLabel.Y = Y;
                                        newTransition3.Label.Add(syncLabel);
                                        if (!isSUT)
                                        {
                                            syncLabel = new Label();
                                            syncLabel.Kind = Constant.TransitionLabelKind.Update;
                                            syncLabel.Text = "flag=false";
                                            syncLabel.X = X;
                                            syncLabel.Y = Y;
                                            newTransition3.Label.Add(syncLabel);
                                        }
                                        
                                        srcTransition2.Source.Ref = newId2;
                                        srcTransition2.Target.Ref = newId3;

                                        tgtTemplate2.Location.Add(newLoc2);
                                        tgtTemplate2.Location.Add(newLoc3);
                                        tgtTemplate2.Transition.Add(newTransition2);
                                        tgtTemplate2.Transition.Add(newTransition3);
                                        tgtTemplate2.Transition.Add(srcTransition2);
                                    }
                                }
                            }
                        }
                        if (locList1.Count > 0 && tranList1.Count > 0)
                        {
                            model1.Template[teIndex].Location.AddRange(locList1);
                            model1.Template[teIndex].Transition.AddRange(tranList1);
                            locList1.Clear();
                            tranList1.Clear();
                        }
                    }
                    model1.Template.Add(tgtTemplate2);
                }
            }
            if (chanIndex > 0)
            {
                syncDeclarations = "\nchan ";
                for (int i = 0; i < chanIndex; i++)
                    syncDeclarations = syncDeclarations + Constant.Common.AuxilaryChannelPostfix + (i+1) + ", ";
                syncDeclarations = syncDeclarations.Substring(0, syncDeclarations.Length - 2) + ";\n";
            }
            if(gIndex > 0)
            {
                syncDeclarations = syncDeclarations + "int ";
                for(int i = 0; i < gIndex; i++)
                    syncDeclarations = syncDeclarations + "g" + (i+1) + ", ";
                syncDeclarations = syncDeclarations.Substring(0, syncDeclarations.Length - 2) + ";\n";
            }
            if (fIndex > 0)
            {
                syncDeclarations = syncDeclarations + "int ";
                for (int i = 0; i < fIndex; i++)
                    syncDeclarations = syncDeclarations + "f" + (i + 1) + ", ";
                syncDeclarations = syncDeclarations.Substring(0, syncDeclarations.Length - 2) + ";\n";
                syncDeclarations = syncDeclarations + "bool flag = false;\n";
            }
            model1.Declaration = model1.Declaration + syncDeclarations;
            return model1;
        }
    }
}