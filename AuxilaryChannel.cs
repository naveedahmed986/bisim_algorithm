using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bisimulation_Desktop
{
    public static class AuxilaryChannel
    {
        public static Nta AddAuxForIO(Nta model)
        {
            Template template;
            Location committedLocation, sourceLoc, targetLoc;
            Transition sourceTransition, newTransition;
            List<string> auxChannelList = new List<string>();
            string channelName = string.Empty;
            string newId = string.Empty;
            if(ChannelInfo.channelInfoList.Count > 0)
            {
                foreach (var ch in ChannelInfo.channelInfoList)
                {
                    template = null;
                    committedLocation = null;
                    sourceLoc = null;
                    targetLoc = null;
                    sourceTransition = null;
                    newTransition = null;
                    channelName = ch.Key;
                    if (ch.Value.Count > 0)
                    {
                        foreach (var info in ch.Value)
                        {
                            template = (from t in model.Template where t.Name.Text.Equals(info.Item1) select t).First();
                            sourceLoc = (from sl in template.Location where sl.Id.Equals(info.Item3) select sl).First();
                            targetLoc = (from tl in template.Location where tl.Id.Equals(info.Item4) select tl).First();
                            //sourceTransition = (from st in template.Transition where (st.Source.Ref.Equals(info.Item3) && st.Target.Ref.Equals(info.Item4)) select st).First();
                            sourceTransition = GetTransitionBySourceAndTarget(template, info.Item3, info.Item4);
                            template.Transition.Remove(sourceTransition);
                            model.Template.Remove(template);

                            newId = LocationInfo.GetNewLocationId(); //get new id from id list for new location
                            committedLocation = new Location(); //initialize new location
                            committedLocation.Id = newId;
                            committedLocation.Committed = "committed";
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

                            template.Location.Add(committedLocation);
                            template.Transition.Add(newTransition);
                            template.Transition.Add(sourceTransition);
                            model.Template.Add(template);
                        }
                    }
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
    }
}
