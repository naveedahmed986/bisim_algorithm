using System;
using System.Linq;
using System.Collections.Generic;

namespace Bisimulation_Desktop
{
    public class ChannelInfo
    {
        //templateName, isBroadcaster (is true "!" else "?"), transitionSourceId, transitionTargetId
        public static List<Tuple<string, bool, string, string>> channelTranList;

        public static Dictionary<string, List<Tuple<string, bool, string, string>>> channelInfoList = 
            new Dictionary<string, List<Tuple<string, bool, string, string>>>();

        //public List<Tuple<string, bool, string, string>> GetChannels()
        //{
        //    return list;
        //}

        //public static void AddChannelInfo(string templateName, bool isBroadcaster, string transitionSourceId,string transitionTargetId)
        //{
        //        channelTranList.Add(new Tuple<string, bool, string, string>(
        //           templateName, isBroadcaster, transitionSourceId, transitionTargetId));
        //}

        public static void AddChannelInfo(Template template)
        {
            if(template != null && template.Transition != null && template.Transition.Count > 0)
            {
                string channelName = string.Empty;
                foreach (Transition transition in template.Transition)
                {
                    channelTranList = new List<Tuple<string, bool, string, string>>();
                    foreach (Label label in transition.Label)
                    {
                        if(label.Kind == "synchronisation")
                        {
                            channelName = label.Text.Substring(0, label.Text.Length - 1);
                            if (channelInfoList.ContainsKey(channelName))
                            {
                                channelInfoList[channelName].Add(new Tuple<string, bool, string, string>(
                                template.Name.Text,
                                IsChannelBroadcaster(label.Text),
                                transition.Source.Ref,
                                transition.Target.Ref
                                ));
                            }
                            else
                            {
                                channelTranList.Add(new Tuple<string, bool, string, string>(
                                template.Name.Text,
                                IsChannelBroadcaster(label.Text),
                                transition.Source.Ref,
                                transition.Target.Ref
                                ));
                                channelInfoList.Add(channelName, channelTranList);
                            }
                        }
                    }
                }
            }
        }

        //Checks if channel is input or output to the SUT
        private static bool IsChannelBroadcaster(string channelName)
        {
            if (channelName.LastIndexOf('!') == channelName.Length - 1)
                return true;
            return false;
        }
    }
}
