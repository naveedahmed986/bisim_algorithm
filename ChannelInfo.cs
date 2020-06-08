using System;
using System.Linq;
using System.Collections.Generic;

namespace Bisimulation_Desktop
{
    public class ChannelInfo
    {
        //Tuple : IsOutput (is true "!" else "?"), transitionSourceId, transitionTargetId, isModified
        public static List<Tuple<bool, string, string, bool>> transitionInfoList;

        public static Dictionary<string,Dictionary<string, List<Tuple<bool, string, string, bool>>>> channelInfoList1 = 
            new Dictionary<string, Dictionary<string, List<Tuple<bool, string, string, bool>>>>();

        public static Dictionary<string, Dictionary<string, List<Tuple<bool, string, string, bool>>>> channelInfoList2 =
            new Dictionary<string, Dictionary<string, List<Tuple<bool, string, string, bool>>>>();

        public static Dictionary<List<Tuple<bool, string, string, bool>>, List<Tuple<bool, string, string, bool>>> synchronizationPairList =
            new Dictionary<List<Tuple<bool, string, string, bool>>, List<Tuple<bool, string, string, bool>>>();

        public static void AddChannelInfoByTemplate(Template template)
        {
            if(template != null && template.Transition != null && template.Transition.Count > 0)
            {
                string channelName = string.Empty;
                bool isEnv = false, envSetBefore = false;
                Dictionary<string, List<Tuple<bool, string, string, bool>>> newItem;
                foreach (Transition transition in template.Transition)
                {
                    transitionInfoList = new List<Tuple<bool, string, string, bool>>();
                    newItem = new Dictionary<string, List<Tuple<bool, string, string, bool>>>();
                    foreach (Label label in transition.Label)
                    {
                        if(label.Kind == Constant.TransitionLabelKind.Synchronization)
                        {
                            
                            // below check will verify if there is any signal sent from the initial location then set ENV flag true
                            //if (template.Init.Ref.Equals(transition.Source.Ref) && IsSender(label.Text) && !envSetBefore)
                            //    isEnv = true;
                            /* below is to explicitly check if there is send and receiving signals (mulitple transitions) 
                            from initial location of a template then dont consider it and set ENV flag false ENV */
                            //if (template.Init.Ref.Equals(transition.Source.Ref) && !IsSender(label.Text))
                            //{
                            //    isEnv = false;
                            //    envSetBefore = true;
                            //}
                            //// if initial state contains self loop and also outgoing with receiver signal then set ENV flag false 
                            //if (template.Init.Ref.Equals(transition.Source.Ref) && template.Init.Ref.Equals(transition.Target.Ref) && IsSender(label.Text))
                            //{
                            //    isEnv = false;
                            //    envSetBefore = true;
                            //}
                            channelName = GetChannelName(label.Text);
                            if (channelInfoList1.ContainsKey(channelName))
                            {
                                if (channelInfoList1[channelName].ContainsKey(template.Name.Text))
                                {
                                    channelInfoList1[channelName][template.Name.Text].Add(new Tuple<bool, string, string, bool>(
                                                IsSender(label.Text),
                                                transition.Source.Ref,
                                                transition.Target.Ref,
                                                false
                                                ));
                                }
                                else
                                {
                                    transitionInfoList.Add(new Tuple<bool, string, string, bool>(
                                        IsSender(label.Text),
                                        transition.Source.Ref,
                                        transition.Target.Ref,
                                        false
                                    ));
                                    channelInfoList1[channelName].Add(template.Name.Text, transitionInfoList);
                                }
                                //channelInfoList1[channelName].Add(new Tuple<bool, string, string, bool>(
                                //template.Name.Text,
                                //IsSender(label.Text),
                                //transition.Source.Ref,
                                //transition.Target.Ref
                                //));
                            }
                            else
                            {
                                transitionInfoList.Add(new Tuple<bool, string, string, bool>(
                                IsSender(label.Text),
                                transition.Source.Ref,
                                transition.Target.Ref,
                                false
                                ));
                                newItem.Add(template.Name.Text, transitionInfoList);
                                channelInfoList1.Add(channelName, newItem);
                            }
                        }
                    }
                }
                //if (isEnv)
                //    TemplateInfo.templateType.Add(template.Name.Text, Constant.TemplateType.ENV);
                //else
                //    TemplateInfo.templateType.Add(template.Name.Text, Constant.TemplateType.Other);
            }
        }

        //Checks if channel is input or output to the SUT (! = Sender, ? = Receiver) 
        public static bool IsSender(string channelName)
        {
            if (!string.IsNullOrEmpty(channelName) && channelName.LastIndexOf(Constant.ActionType.Sender) == channelName.Length - 1)
                return true;
            return false;
        }

        public static bool IsInputChannel(string channelName)
        {
            if (!string.IsNullOrEmpty(channelName) && channelName.Substring(0,2).Equals(Constant.Common.InputChannelPrefix))
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

        public static void SplitChannelList()
        {
            foreach (var channel in channelInfoList1)
                foreach (var channel2 in channelInfoList1)
                    if (channel2.Key.Equals(channel.Key + Constant.Common.VariablePostfix))
                        channelInfoList2.Add(channel2.Key, channel2.Value);
            foreach (var ch in channelInfoList2)
                if (channelInfoList1.ContainsKey(ch.Key))
                    channelInfoList1.Remove(ch.Key);
        }

        public static void AddTemplateType(Template template)
        {
            bool isEnv = false;
            foreach (Transition transition in template.Transition)
            {
                if (transition.Label.Count > 0)
                {
                    foreach (Label label in transition.Label)
                    {
                        if (label.Kind.Equals(Constant.TransitionLabelKind.Synchronization))
                        {
                            if (!string.IsNullOrEmpty(label.Text) &&
                                label.Text.Substring(0, 2).Equals(Constant.Common.InputChannelPrefix) &&
                                    label.Text.Substring(label.Text.Length - 1, 1).Equals(Convert.ToString(Constant.ActionType.Sender)))
                            {
                                isEnv = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (!TemplateInfo.templateType.ContainsKey(template.Name.Text))
            { 
                if (isEnv)
                    TemplateInfo.templateType.Add(template.Name.Text, Constant.TemplateType.ENV);
                else
                    TemplateInfo.templateType.Add(template.Name.Text, Constant.TemplateType.Other);
            }

            //bool isSender = false;
            //foreach (var channel in ChannelInfo.channelInfoList1)
            //{
            //    foreach (var template in channel.Value)
            //    {
            //        if (channel.Key.Substring(0, 2).Equals(Constant.Common.InputChannelPrefix))
            //        {
            //            foreach (var item in template.Value)
            //            {
            //                if (item.Item1)
            //                    isSender = true;
            //            }
            //            if (!TemplateInfo.templateType.ContainsKey(template.Key))
            //            {
            //                if (isSender)
            //                    TemplateInfo.templateType.Add(template.Key, Constant.TemplateType.ENV);
            //                else
            //                    TemplateInfo.templateType.Add(template.Key, Constant.TemplateType.Other);
            //            }
            //        }
            //        if (channel.Key.Substring(0, 2).Equals(Constant.Common.OutputChannelPrefix))
            //        {
            //            foreach (var item in template.Value)
            //            {
            //                if (item.Item1)
            //                    isSender = true;
            //            }
            //            if (!TemplateInfo.templateType.ContainsKey(template.Key))
            //            {
            //                if (isSender)
            //                    TemplateInfo.templateType.Add(template.Key, Constant.TemplateType.Other);
            //                else
            //                    TemplateInfo.templateType.Add(template.Key, Constant.TemplateType.ENV);
            //            }
            //        }
            //        isSender = false;
            //    }
            //}
        }

        //public static void SynchronizationPairs(Nta model)
        //{
        //    foreach(var channel in channelInfoList1)
        //    {
        //        if(channel.Value.Count > 1)
        //        {
        //            string templateName = string.Empty;
        //            for(int i = 0; i < channel.Value.Count; i++)
        //            {
        //                for (int j = 0; j < channel.Value.Count; j++)
        //                {
        //                    if(i != j && channel.Value[i].Item1 == channel.Value[j].Item1)
        //                    {

        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
