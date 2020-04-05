using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bisimulation_Desktop
{
    public static class AuxilaryChannel
    {
        private static List<string> auxChannelList = new List<string>();
        public static Nta AddAuxForIOAction(Nta model)
        {
            Template template = null; ;
            Location committedLocation = null, sourceLoc = null, targetLoc = null;
            Transition sourceTransition = null, newTransition = null;
            Label syncLabel = null, selectLabel = null;
            string newId = string.Empty, selectLabelValue = string.Empty;
            string channelDeclaration = string.Empty;
            bool isAuxChannelAdded = false;
            if(ChannelInfo.channelInfoList.Count > 0)
            {
                foreach (var ch in ChannelInfo.channelInfoList)
                {
                    if(ch.Key.Length >= 2 && (string.Equals(ch.Key.Substring(0, 2), Constant.Common.InputChannelPrefix) ||
                        string.Equals(ch.Key.Substring(0, 2), Constant.Common.OutputChannelPrefix)) )
                    {
                        //channelDeclaration = channelDeclaration + ch.Key + Constant.Common.AuxilaryChannelPostfix + " ,";
                        if (ch.Value.Count > 0)
                        {
                            foreach (var info in ch.Value)
                            {
                                template = null;
                                sourceLoc = null;
                                targetLoc = null;
                                sourceTransition = null;

                                template = (from t in model.Template where t.Name.Text.Equals(info.Item1) select t).First();
                                sourceLoc = (from sl in template.Location where sl.Id.Equals(info.Item3) select sl).First();
                                targetLoc = (from tl in template.Location where tl.Id.Equals(info.Item4) select tl).First();
                                sourceTransition = TransitionInfo.GetTransitionBySourceAndTarget(template, info.Item3, info.Item4);
                                template.Transition.Remove(sourceTransition);
                                model.Template.Remove(template);

                                newId = LocationInfo.GetNewLocationId(); //get new id from id list for new location
                                committedLocation = new Location(); //initialize new location
                                committedLocation.Id = newId;
                                committedLocation.Committed = Constant.LocationLabelKind.Committed;
                                Tuple<string, string> coordinates = LocationPoint.GetCoordinatesForLocation(sourceLoc, targetLoc, sourceTransition);
                                committedLocation.X = coordinates.Item1;
                                committedLocation.Y = coordinates.Item2;
                                LocationInfo.locationIds.Add(Int32.Parse(Regex.Match(newId, @"\d+").Value)); // add new id to the location id list

                                newTransition = new Transition();
                                newTransition.Source = new Source();
                                newTransition.Target = new Target();

                                sourceTransition.Target.Ref = newId;
                                newTransition.Source.Ref = newId;
                                newTransition.Target.Ref = targetLoc.Id;
                                
                                syncLabel = new Label();
                                syncLabel.Kind = Constant.TransitionLabelKind.Synchronization;
                                syncLabel.Text = GetAuxChannelName(info.Item2, TransitionInfo.GetChannelNameFromTransition(sourceTransition), model.Declaration);
                                coordinates = LocationPoint.CalculateCenterPoint(
                                    Int32.Parse(coordinates.Item1),
                                    Int32.Parse(coordinates.Item2),
                                    Int32.Parse(targetLoc.X),
                                    Int32.Parse(targetLoc.Y));
                                syncLabel.X = coordinates.Item1;
                                syncLabel.Y = coordinates.Item2;

                                newTransition.Label = new List<Label>();
                                newTransition.Label.Add(syncLabel);

                                selectLabelValue = GetLabelForSelectIfExist(sourceTransition);
                                if (!string.IsNullOrEmpty(selectLabelValue))
                                {
                                    selectLabel = new Label();
                                    selectLabel.Kind = Constant.TransitionLabelKind.Select;
                                    selectLabel.Text = selectLabelValue;
                                    selectLabel.X = coordinates.Item1;
                                    selectLabel.Y = Convert.ToString(Int32.Parse(coordinates.Item2) - 15);
                                    newTransition.Label.Add(selectLabel);
                                }
                                
                                template.Location.Add(committedLocation);
                                template.Transition.Add(newTransition);
                                template.Transition.Add(sourceTransition);
                                model.Template.Add(template);
                                isAuxChannelAdded = true;
                            }
                        }
                    }
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

        public static string GetAuxChannelName(bool isOutput, string sourceTChannelName, string gDeclaration)
        {
            if (!string.IsNullOrEmpty(sourceTChannelName))
            {
                string postfix = string.Empty;
                string channelName = ChannelInfo.GetChannelName(sourceTChannelName);
                if (sourceTChannelName.Contains('[') && sourceTChannelName.Contains(']') && 
                    sourceTChannelName.IndexOf(']') > sourceTChannelName.IndexOf('['))
                {
                    postfix = sourceTChannelName.Substring(
                            sourceTChannelName.IndexOf('['),
                            (sourceTChannelName.Length - sourceTChannelName.IndexOf('[')));
                    AddChannelNameToAuxChannelList(channelName, gDeclaration); 
                }
                else
                {
                    postfix = Convert.ToString(sourceTChannelName.Last());
                    if (!auxChannelList.Contains(channelName+Constant.Common.AuxilaryChannelPostfix))
                        auxChannelList.Add(channelName + Constant.Common.AuxilaryChannelPostfix);
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
        
        private static void AddChannelNameToAuxChannelList(string channelName, string gDeclaration)
        {
            if(!string.IsNullOrEmpty(channelName))
            {
                int startIndex = gDeclaration.IndexOf(channelName);
                int endIndex = gDeclaration.IndexOf(']', startIndex);
                string channelInstanceFromDec = gDeclaration.Substring(startIndex,(endIndex-startIndex) + 1);
                channelInstanceFromDec = channelInstanceFromDec.Substring(channelInstanceFromDec.IndexOf('['),
                    (channelInstanceFromDec.IndexOf(']') - channelInstanceFromDec.IndexOf('[')) + 1 
                    );
                channelName = channelName + Constant.Common.AuxilaryChannelPostfix + channelInstanceFromDec;
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
    }
}
