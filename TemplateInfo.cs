using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bisimulation_Desktop
{
    public class TemplateInfo
    {
        public static Dictionary<string, string> templateType = new Dictionary<string, string>();

        public static Template GetTemplateByName(Nta model, string templateName)
        {
            foreach (Template template in model.Template)
                if (!string.IsNullOrEmpty(templateName) && template.Name.Text.Equals(templateName))
                    return template;
            return null;
        }

        public static void AddTemplateType(Template template)
        {
            bool isSUT = false;
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
                                    label.Text.Substring(label.Text.Length - 1, 1).Equals(Convert.ToString(Constant.ActionType.Receiver)))
                            {
                                isSUT = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (!templateType.ContainsKey(template.Name.Text))
            {
                if (isSUT)
                    templateType.Add(template.Name.Text, Constant.TemplateType.SUT);
                else
                    templateType.Add(template.Name.Text, Constant.TemplateType.Other);
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
    }
}
