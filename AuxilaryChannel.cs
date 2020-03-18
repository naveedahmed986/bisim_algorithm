using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bisimulation_Desktop
{
    public static class AuxilaryChannel
    {
        public static Nta AddAuxForIOAction(Nta model)
        {
            Template template;
            Location committedLocation, sourceLoc, targetLoc;
            Transition sourceTransition, newTransition;
            List<string> auxChannelList = new List<string>();
            Label label;
            string newId = string.Empty;
            string channelDeclaration = string.Empty;
            bool isAuxChannelAdded = false;
            if(ChannelInfo.channelInfoList.Count > 0)
            {
                foreach (var ch in ChannelInfo.channelInfoList)
                {
                    if(string.Equals(ch.Key.Substring(0, 2), Common.Constant.InputChannelPrefix) ||
                        string.Equals(ch.Key.Substring(0, 2), Common.Constant.OutputChannelPrefix))
                    {
                        template = null;
                        committedLocation = null;
                        sourceLoc = null;
                        targetLoc = null;
                        sourceTransition = null;
                        channelDeclaration = channelDeclaration + ch.Key + Common.Constant.AuxilaryChannelPostfix + " ,";
                        if (ch.Value.Count > 0)
                        {
                            foreach (var info in ch.Value)
                            {
                                template = (from t in model.Template where t.Name.Text.Equals(info.Item1) select t).First();
                                sourceLoc = (from sl in template.Location where sl.Id.Equals(info.Item3) select sl).First();
                                targetLoc = (from tl in template.Location where tl.Id.Equals(info.Item4) select tl).First();
                                sourceTransition = GetTransitionBySourceAndTarget(template, info.Item3, info.Item4);

                                template.Transition.Remove(sourceTransition);
                                model.Template.Remove(template);

                                newId = LocationInfo.GetNewLocationId(); //get new id from id list for new location
                                committedLocation = new Location(); //initialize new location
                                committedLocation.Id = newId;
                                committedLocation.Committed = Common.LocationLabelKind.Committed;
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
                                label = new Label();
                                label.Kind = Common.TransitionLabelKind.Synchronization;
                                label.Text = GetAuxChannelName(ch.Key, info.Item2);
                                coordinates = LocationPoint.CalculateCenterPoint(
                                    Int32.Parse(coordinates.Item1),
                                    Int32.Parse(coordinates.Item2),
                                    Int32.Parse(targetLoc.X),
                                    Int32.Parse(targetLoc.Y));
                                label.X = coordinates.Item1;
                                label.Y = coordinates.Item2;
                                newTransition.Label = new List<Label>();
                                newTransition.Label.Add(label);

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
                    channelDeclaration = "\nchan " + channelDeclaration.Substring(0, channelDeclaration.Length - 1) + ";";
                    model.Declaration = string.Concat(model.Declaration, channelDeclaration);
                }
            }
            return model;
        }

        public static Transition GetTransitionBySourceAndTarget(Template template, string sourceId, string targetId)
        {
            Transition transition = null;
            if (template != null)
            {
                foreach(Transition t in template.Transition)
                {
                    if(t.Source.Ref == sourceId && t.Target.Ref == targetId)
                    {
                        transition = t;
                        break;
                    }
                }
            }
            return transition;
        }

        public static string GetAuxChannelName(string channelName, bool isOutput)
        {
            if (!string.IsNullOrEmpty(channelName))
            {
                if (isOutput)
                    channelName = channelName + Common.Constant.AuxilaryChannelPostfix + Convert.ToString(Common.ActionType.Output);
                else
                    channelName = channelName + Common.Constant.AuxilaryChannelPostfix + Convert.ToString(Common.ActionType.Input);
                return channelName;
            }
            return "";
        }
    }
}
