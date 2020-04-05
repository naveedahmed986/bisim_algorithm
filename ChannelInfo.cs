using System;
using System.Linq;
using System.Collections.Generic;

namespace Bisimulation_Desktop
{
    public class ChannelInfo
    {
        //Tuple : templateName, IsOutput (is true "!" else "?"), transitionSourceId, transitionTargetId
        public static List<Tuple<string, bool, string, string>> channelTranList;

        public static Dictionary<string, List<Tuple<string, bool, string, string>>> channelInfoList = 
            new Dictionary<string, List<Tuple<string, bool, string, string>>>();

        public static void AddChannelInfoByTemplate(Template template)
        {
            if(template != null && template.Transition != null && template.Transition.Count > 0)
            {
                string channelName = string.Empty;
                foreach (Transition transition in template.Transition)
                {
                    channelTranList = new List<Tuple<string, bool, string, string>>();
                    foreach (Label label in transition.Label)
                    {
                        if(label.Kind == Constant.TransitionLabelKind.Synchronization)
                        {
                            channelName = GetChannelName(label.Text);
                            if (channelInfoList.ContainsKey(channelName))
                            {
                                channelInfoList[channelName].Add(new Tuple<string, bool, string, string>(
                                template.Name.Text,
                                IsOutputAction(label.Text),
                                transition.Source.Ref,
                                transition.Target.Ref
                                ));
                            }
                            else
                            {
                                channelTranList.Add(new Tuple<string, bool, string, string>(
                                template.Name.Text,
                                IsOutputAction(label.Text),
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

        //Checks if channel is input or output to the SUT (! = Output, ? = Input) 
        private static bool IsOutputAction(string channelName)
        {
            if (channelName.LastIndexOf(Constant.ActionType.Output) == channelName.Length - 1)
                return true;
            return false;
        }

        public static string GetChannelName(string channelName)
        {
            if (!string.IsNullOrEmpty(channelName))
            {
                if(channelName.Contains('[') && channelName.Contains(']'))
                {
                    channelName = channelName.Substring(0, channelName.IndexOf('['));
                }
                else
                    channelName = channelName.Substring(0, channelName.Length - 1);
            }
            return channelName;
        }
    }
}
